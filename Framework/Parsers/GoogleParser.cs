using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Flyttaihop.Framework.Models;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json.Linq;

namespace Flyttaihop.Framework.Parsers
{
    public class GoogleParser
    {
        private readonly IDistributedCache _cache;

        public GoogleParser(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<SearchResult> ParseItem(Criteria criteria, SearchResult hemnetItem, string apiKey)
        {
            hemnetItem.Durations = new List<DurationAndDistance>();

            //Räkna ut tidsåtgång (api-dokumentation på https://developers.google.com/maps/documentation/directions/intro)
            if (criteria.DurationCriterias.Any() && !string.IsNullOrWhiteSpace(apiKey))
            {
                foreach (var durationCriteria in criteria.DurationCriterias)
                {
                    string url = GetUrl(durationCriteria, hemnetItem, apiKey);

                    string jsonContent = await _cache.GetStringAsync(url);

                    if (jsonContent == null)
                    {
                        using (var googleClient = new HttpClient())
                        {
                            googleClient.BaseAddress = new Uri("https://maps.googleapis.com");
                            var result = await googleClient.GetAsync(url);
                            jsonContent = await result.Content.ReadAsStringAsync();
                            //TODO: Cacha i sqlite istället (egen fil cache.db) så att den sparas fastän applikationen startar om. Jag kanske ska göra en egen CacheProvider för det ändamålet? 
                            await _cache.SetStringAsync(url, jsonContent, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1) });
                        }
                    }

                    var doc = JObject.Parse(jsonContent);

                    if ((string)doc.SelectToken("geocoded_waypoints[0].geocoder_status") != "OK" ||
                        (string)doc.SelectToken("geocoded_waypoints[1].geocoder_status") != "OK" ||
                        (string)doc.SelectToken("status") != "OK")
                    {
                        //Inkluderar de objekt vi inte lyckas slå upp för säkerhets skull
                        return hemnetItem;
                    }

                    int distanceMeters = (int)doc.SelectToken("routes[0].legs[0].distance.value");
                    decimal distanceKilometers = Math.Round(distanceMeters / (decimal)1000, 1);

                    int durationSeconds = (int)doc.SelectToken("routes[0].legs[0].duration.value");
                    int durationMinutes = durationSeconds / 60;

                    if (durationMinutes > durationCriteria.Minutes)
                    {
                        //Exkluderar de objekt som ligger för långt bort
                        return null;
                    }

                    hemnetItem.Durations.Add(new DurationAndDistance
                    {
                        Minutes = durationMinutes,
                        Type = durationCriteria.Type,
                        Target = durationCriteria.Target,
                        Kilometers = distanceKilometers
                    });
                }
            }

            return hemnetItem;
        }

        #region Helpers

        private string GetUrl(Duration durationCriteria, SearchResult hemnetItem, string apiKey)
        {
            string itemAddress = hemnetItem.Address + "," + hemnetItem.City;

            string url = string.Format("/maps/api/directions/json?origin={0}&destination={1}&key={2}", itemAddress, durationCriteria.Target, apiKey);

            if (durationCriteria.Type == Duration.TraversalType.Commuting)
            {
                url += "&mode=transit&departure_time=" + "TODO: Unix epoch timestamp för nästa tisdag kl 17";
            }
            else if (durationCriteria.Type == Duration.TraversalType.Walking)
            {
                url += "&mode=walking";
            }
            else if (durationCriteria.Type == Duration.TraversalType.Biking)
            {
                url += "&mode=bicycling";
            }

            return url;
        }

        #endregion
    }
}