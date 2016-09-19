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
using Microsoft.Extensions.Options;

namespace Flyttaihop.Controllers
{
    [Route("api/[controller]")]
    public class SearchController : Controller
    {
        private readonly IOptions<ApplicationOptions> _applicationOptions;
        private readonly ICriteriaRepository _criteriaRepository;
        private readonly HemnetParser _hemnetParser;

        public SearchController(IOptions<ApplicationOptions> applicationOptions, ICriteriaRepository criteriaRepository, HemnetParser hemnetParser)
        {
            _applicationOptions = applicationOptions;
            _criteriaRepository = criteriaRepository;
            _hemnetParser = hemnetParser;
        }

        [HttpGet()]
        ///<summary>Gör en sökning mot Hemnet och Google Maps Directions med de sparade kriterierna</summary>
        public async Task<List<SearchResult>> Search()
        {
            var savedCriteria = _criteriaRepository.GetSavedCriteria();

            var result = new List<SearchResult>();

            using (var hemnetClient = new HttpClient())
            {
                var doc = new HtmlDocument();

                hemnetClient.BaseAddress = new Uri("http://www.hemnet.se");
                string hemnetUrl = "/bostader?item_types%5B%5D=bostadsratt&upcoming=1&price_max=4000000&rooms_min=2.5&living_area_min=65&location_ids%5B%5D=17744";
                if (savedCriteria.Keywords.Any())
                {
                    hemnetUrl += "&keywords=" + savedCriteria.Keywords.JoinUrlEncoded(",");
                }
                var hemnetResult = await hemnetClient.GetAsync(hemnetUrl);
                doc.LoadHtml(await hemnetResult.Content.ReadAsStringAsync());

                bool hasRequestedGoogleDistanceOneTime = false;

                foreach (var itemContainerNode in doc.GetElementbyId("search-results").Elements("li"))
                {
                    var itemNode = itemContainerNode.Elements("div").Single();

                    var sr = _hemnetParser.ParseNode(itemNode);

                    if (sr == null)
                    {
                        continue;
                    }

                    if (!hasRequestedGoogleDistanceOneTime) //Debug: bara första objektet
                    {
                        //Räkna ut avstånd
                        if (savedCriteria.DistanceCriterias.Any())
                        {
                            string googleApiKey = _applicationOptions.Value.GoogleApiKey;

                            if (!string.IsNullOrWhiteSpace(googleApiKey))
                            {
                                foreach (var distanceCriteria in savedCriteria.DistanceCriterias)
                                {
                                    using (var googleClient = new HttpClient())
                                    {
                                        //Dokumentation: https://developers.google.com/maps/documentation/directions/intro

                                        googleClient.BaseAddress = new Uri("https://maps.googleapis.com");
                                        string itemAddress = sr.Address + "," + sr.City;
                                        string requestUri = string.Format("/maps/api/directions/json?origin={0}&destination={1}&key={2}", itemAddress, distanceCriteria.Target, googleApiKey);
                                        if (distanceCriteria.Type == Criteria.DistanceCriteria.DistanceType.Commuting)
                                        {
                                            requestUri += "&mode=transit&departure_time=" + "TODO: Unix epoch timestamp för nästa tisdag kl 17";
                                        }
                                        else if (distanceCriteria.Type == Criteria.DistanceCriteria.DistanceType.Walking)
                                        {
                                            requestUri += "&mode=walking";
                                        }
                                        else if (distanceCriteria.Type == Criteria.DistanceCriteria.DistanceType.Biking)
                                        {
                                            requestUri += "&mode=bicycling";
                                        }

                                        var googleResult = await googleClient.GetAsync(requestUri);

                                        var parseThisToJson = await googleResult.Content.ReadAsStringAsync();

                                        //TODO: Fortsätt
                                    }
                                }
                            }
                            hasRequestedGoogleDistanceOneTime = true;
                        }
                    }

                    result.Add(sr);
                }
            }

            return result;
        }
    }
}