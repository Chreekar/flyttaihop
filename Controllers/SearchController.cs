using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Flyttaihop.Configuration;
using Flyttaihop.Framework.Extensions;
using Flyttaihop.Framework.Interfaces;
using Flyttaihop.Framework.Models;
using Flyttaihop.Framework.Parsers;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Flyttaihop.Controllers
{
    [Route("api/[controller]")]
    public class SearchController : Controller
    {
        private readonly IOptions<ApplicationOptions> _applicationOptions;
        private readonly ILogger<SearchController> _logger;
        private readonly ICriteriaRepository _criteriaRepository;
        private readonly HemnetParser _hemnetParser;

        public SearchController(IOptions<ApplicationOptions> applicationOptions, ILogger<SearchController> logger, ICriteriaRepository criteriaRepository, HemnetParser hemnetParser)
        {
            _applicationOptions = applicationOptions;
            _logger = logger;
            _criteriaRepository = criteriaRepository;
            _hemnetParser = hemnetParser;
        }

        [HttpGet()]
        ///<summary>Gör en sökning mot Hemnet och Google Maps Directions med de sparade kriterierna</summary>
        public async Task<List<SearchResult>> Search()
        {
            var savedCriteria = _criteriaRepository.GetSavedCriteria();

            string googleApiKey = _applicationOptions.Value.GoogleApiKey;

            if (string.IsNullOrWhiteSpace(googleApiKey))
            {
                _logger.LogError("GoogleApiKey not specified, skipping duration calculations");
            }

            var result = new List<SearchResult>();

            //TODO: Cacha hemnetsökningen i tio minuter (i minnet), Google-uppslaget i eget ef-context (egen sqlite-databas). Kan jag konfigurera den contexten som en cacheprovider och låta den vara giltig i en vecka?

            using (var hemnetClient = new HttpClient())
            {
                var doc = new HtmlDocument();

                hemnetClient.BaseAddress = new Uri("http://www.hemnet.se");
                string hemnetUrl = "/bostader?item_types%5B%5D=bostadsratt&upcoming=1&price_max=4000000&rooms_min=2.5&living_area_min=65&location_ids%5B%5D=17744";
                if (savedCriteria.Keywords.Any())
                {
                    hemnetUrl += "&keywords=" + savedCriteria.Keywords.Select(x => x.Text).JoinUrlEncoded(",");
                }
                var hemnetResult = await hemnetClient.GetAsync(hemnetUrl);
                doc.LoadHtml(await hemnetResult.Content.ReadAsStringAsync());

                //TODO: Kör loopen asynkront så att vi väntar in alla varv efteråt innan vi returnerar listan  
                //kanske så här: http://stackoverflow.com/questions/23137393/parallel-foreach-and-async-await
                foreach (var itemContainerNode in doc.GetElementbyId("search-results").Elements("li"))
                {
                    var itemNode = itemContainerNode.Elements("div").Single();

                    var item = await GetItem(itemNode, savedCriteria, googleApiKey);

                    if (item != null)
                    {
                        result.Add(item);
                    }
                }
            }

            return result;
        }

        private async Task<SearchResult> GetItem(HtmlNode itemNode, Criteria savedCriteria, string googleApiKey)
        {
            var item = _hemnetParser.ParseNode(itemNode);

            if (item == null)
            {
                return null;
            }

            item.Durations = new List<Duration>();

            //Räkna ut tidsåtgång
            if (savedCriteria.DurationCriterias.Any() && !string.IsNullOrWhiteSpace(googleApiKey))
            {
                foreach (var durationCriteria in savedCriteria.DurationCriterias)
                {
                    using (var googleClient = new HttpClient())
                    {
                        //Dokumentation: https://developers.google.com/maps/documentation/directions/intro

                        googleClient.BaseAddress = new Uri("https://maps.googleapis.com");
                        string itemAddress = item.Address + "," + item.City;
                        string requestUri = string.Format("/maps/api/directions/json?origin={0}&destination={1}&key={2}", itemAddress, durationCriteria.Target, googleApiKey);
                        if (durationCriteria.Type == Duration.TraversalType.Commuting)
                        {
                            requestUri += "&mode=transit&departure_time=" + "TODO: Unix epoch timestamp för nästa tisdag kl 17";
                        }
                        else if (durationCriteria.Type == Duration.TraversalType.Walking)
                        {
                            requestUri += "&mode=walking";
                        }
                        else if (durationCriteria.Type == Duration.TraversalType.Biking)
                        {
                            requestUri += "&mode=bicycling";
                        }

                        var googleResult = await googleClient.GetAsync(requestUri);
                        var googleResultString = await googleResult.Content.ReadAsStringAsync();

                        var googleJson = JObject.Parse(googleResultString);

                        if ((string)googleJson.SelectToken("geocoded_waypoints[0].geocoder_status") != "OK" ||
                            (string)googleJson.SelectToken("geocoded_waypoints[1].geocoder_status") != "OK")
                        {
                            //Inkluderar de objekt vi inte lyckas slå upp för säkerhets skull
                            return item;
                        }

                        int distanceMeters = (int)googleJson.SelectToken("routes[0].legs[0].distance.value");
                        float distanceKilometers = distanceMeters / 1000f;

                        int durationSeconds = (int)googleJson.SelectToken("routes[0].legs[0].duration.value");
                        int durationMinutes = durationSeconds / 60;

                        if (durationMinutes > durationCriteria.Minutes)
                        {
                            //Exkluderar de objekt som ligger för långt bort
                            return null;
                        }

                        item.Durations.Add(new Duration
                        {
                            Minutes = durationMinutes,
                            Type = durationCriteria.Type,
                            Target = durationCriteria.Target
                        });
                    }
                }
            }

            return item;
        }
    }
}