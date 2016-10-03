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
using Microsoft.Extensions.Caching.Distributed;
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
        private readonly IDistributedCache _cache;
        private readonly ICriteriaRepository _criteriaRepository;
        private readonly HemnetParser _hemnetParser;

        public SearchController(IOptions<ApplicationOptions> applicationOptions, ILogger<SearchController> logger, IDistributedCache cache, ICriteriaRepository criteriaRepository, HemnetParser hemnetParser)
        {
            _applicationOptions = applicationOptions;
            _logger = logger;
            _cache = cache;
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

            var hemnetDoc = new HtmlDocument();

            string hemnetUrl = "/bostader?item_types%5B%5D=bostadsratt&upcoming=1&price_max=4000000&rooms_min=2.5&living_area_min=65&location_ids%5B%5D=17744";
            if (savedCriteria.Keywords.Any())
            {
                hemnetUrl += "&keywords=" + savedCriteria.Keywords.Select(x => x.Text).JoinUrlEncoded(",");
            }

            string hemnetHtml = await _cache.GetStringAsync(hemnetUrl);

            if (hemnetHtml == null)
            {
                using (var hemnetClient = new HttpClient())
                {
                    hemnetClient.BaseAddress = new Uri("http://www.hemnet.se");
                    var hemnetResult = await hemnetClient.GetAsync(hemnetUrl);
                    hemnetHtml = await hemnetResult.Content.ReadAsStringAsync();
                    await _cache.SetStringAsync(hemnetUrl, hemnetHtml, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2) });
                }
            }

            hemnetDoc.LoadHtml(hemnetHtml);

            //TODO: Kör loopen asynkront så att vi väntar in alla varv efteråt innan vi returnerar listan  
            //kanske så här: http://stackoverflow.com/questions/23137393/parallel-foreach-and-async-await
            foreach (var itemContainerNode in hemnetDoc.GetElementbyId("search-results").Elements("li"))
            {
                var itemNode = itemContainerNode.Elements("div").Single();

                var item = _hemnetParser.ParseNode(itemNode);

                if (item != null)
                {
                    item = await ParseItem(item, savedCriteria, googleApiKey);

                    if (item != null)
                    {
                        result.Add(item);
                    }
                }
            }

            return result;
        }

        //TODO: Bryt ut denna till en egen klass _googleParser
        private async Task<SearchResult> ParseItem(SearchResult hemnetItem, Criteria savedCriteria, string googleApiKey)
        {
            hemnetItem.Durations = new List<Duration>();

            //Räkna ut tidsåtgång
            if (savedCriteria.DurationCriterias.Any() && !string.IsNullOrWhiteSpace(googleApiKey))
            {
                foreach (var durationCriteria in savedCriteria.DurationCriterias)
                {
                    using (var googleClient = new HttpClient())
                    {
                        //Dokumentation: https://developers.google.com/maps/documentation/directions/intro

                        googleClient.BaseAddress = new Uri("https://maps.googleapis.com");
                        string itemAddress = hemnetItem.Address + "," + hemnetItem.City;
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

                        //TODO: Cacha Google-uppslaget i eget ef-context (egen sqlite-databas). Kan jag konfigurera den contexten som en cacheprovider och låta den vara giltig i en vecka?

                        var googleResult = await googleClient.GetAsync(requestUri);
                        var googleResultString = await googleResult.Content.ReadAsStringAsync();

                        var googleJson = JObject.Parse(googleResultString);

                        if ((string)googleJson.SelectToken("geocoded_waypoints[0].geocoder_status") != "OK" ||
                            (string)googleJson.SelectToken("geocoded_waypoints[1].geocoder_status") != "OK")
                        {
                            //Inkluderar de objekt vi inte lyckas slå upp för säkerhets skull
                            return hemnetItem;
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

                        hemnetItem.Durations.Add(new Duration
                        {
                            Minutes = durationMinutes,
                            Type = durationCriteria.Type,
                            Target = durationCriteria.Target
                        });
                    }
                }
            }

            return hemnetItem;
        }
    }
}