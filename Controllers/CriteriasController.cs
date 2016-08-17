using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;

namespace Flyttaihop.Controllers
{
    [Route("api/[controller]")]
    public class CriteriasController : Controller
    {
        //TODO: Byt ut mot sessionsvariabel (och läs in från databas framöver)

        private Criteria savedCriteria = new Criteria
        {
            Keywords = new List<string>(),
            DistanceCriterias = new List<Criteria.DistanceCriteria>()
        };

        [HttpGet()]
        ///<summary>Hämta tidigare sparad criteria</summary>
        public Criteria Load()
        {
            return savedCriteria;
        }

        [HttpPost()]
        ///<summary>Spara ny criteria (skriver över existerande om redan finns)</summary>
        public Criteria Save([FromBody]Criteria request)
        {
            savedCriteria = request;
            return savedCriteria;
        }

        [HttpGet("search")]
        ///<summary>Gör en sökning mot Hemnet med de sparade kriterierna</summary>
        public List<SearchResult> Search()
        {
            //TODO: Bryt ut denna till searchcontroller?

            //TODO: använd denna för att räkna ut avstånd: https://developers.google.com/maps/documentation/javascript/directions

            var result = new List<SearchResult>();

            using (var client = new HttpClient())
            {
                var doc = new HtmlDocument();

                client.BaseAddress = new Uri("http://www.hemnet.se");
                var getTask = client.GetAsync("/bostader?item_types%5B%5D=bostadsratt&upcoming=1&price_max=4000000&rooms_min=2.5&living_area_min=65&location_ids%5B%5D=17744");
                getTask.Wait();
                var res = getTask.Result;
                var readTask = res.Content.ReadAsStringAsync();
                readTask.Wait();

                //TODO: Bryt ut Wait()-grejerna ovan till async-metod som kör "await client.GetAsync..." osv

                doc.LoadHtml(readTask.Result);

                foreach (var itemContainerNode in doc.GetElementbyId("search-results").Elements("li"))
                {
                    var itemNode = itemContainerNode.Elements("div").Single();

                    //TODO: Bryt ut parsningen nedan till egen klass

                    try
                    {
                        result.Add(new SearchResult
                        {
                            Area = itemNode
                                .Elements("ul")
                                .Where(n => n.GetAttributeValue("class", "").Contains("location-type"))
                                .Single()
                                .Elements("li")
                                .Where(n => n.GetAttributeValue("class", "").Contains("area"))
                                .Single()
                                .InnerText
                                .Trim(),
                            City = itemNode
                                .Elements("ul")
                                .Where(n => n.GetAttributeValue("class", "").Contains("location-type"))
                                .Single()
                                .Elements("li")
                                .Where(n => n.GetAttributeValue("class", "").Contains("city"))
                                .Single()
                                .InnerText
                                .Trim(),
                            Address = itemNode
                                .Elements("ul")
                                .Where(n => n.GetAttributeValue("class", "").Contains("location-type"))
                                .Single()
                                .Elements("li")
                                .Where(n => n.GetAttributeValue("class", "").Contains("address"))
                                .Single()
                                .InnerText
                                .Trim(),
                            Price = itemNode
                                .Elements("ul")
                                .Where(n => n.GetAttributeValue("class", "").Contains("prices"))
                                .Single()
                                .Elements("li")
                                .Where(n => n.GetAttributeValue("class", "").Contains("price"))
                                .Single()
                                .InnerText
                                .Trim(),
                            Fee = itemNode
                                .Elements("ul")
                                .Where(n => n.GetAttributeValue("class", "").Contains("prices"))
                                .Single()
                                .Elements("li")
                                .Where(n => n.GetAttributeValue("class", "").Contains("fee"))
                                .Single()
                                .InnerText
                                .Trim(),
                            Size = itemNode
                                .Elements("ul")
                                .Where(n => n.GetAttributeValue("class", "").Contains("size"))
                                .Single()
                                .Elements("li")
                                .Where(n => n.GetAttributeValue("class", "").Contains("living-area"))
                                .Single()
                                .InnerText
                                .Split(new string[] { "&nbsp;" }, StringSplitOptions.RemoveEmptyEntries)[0]
                                .Trim(),
                            Rooms = itemNode
                                .Elements("ul")
                                .Where(n => n.GetAttributeValue("class", "").Contains("size"))
                                .Single()
                                .Elements("li")
                                .Where(n => n.GetAttributeValue("class", "").Contains("living-area"))
                                .Single()
                                .InnerText
                                .Split(new string[] { "&nbsp;" }, StringSplitOptions.RemoveEmptyEntries)[1]
                                .Trim(),
                            ImageUrl = itemNode
                                .Elements("div")
                                .Where(n => n.GetAttributeValue("class", "").Contains("image"))
                                .Single()
                                .Elements("img")
                                .Single()
                                .GetAttributeValue("data-src", ""),
                            Url = "http://www.hemnet.se" + itemNode
                                .Elements("a")
                                .Where(n => n.GetAttributeValue("class", "").Contains("item-link-container"))
                                .Single()
                                .GetAttributeValue("href", "")
                        });
                    }
                    catch (Exception) { }
                }
            }

            return result;
        }

        public class Criteria
        {
            public IEnumerable<string> Keywords { get; set; }

            public IEnumerable<DistanceCriteria> DistanceCriterias { get; set; }

            public class DistanceCriteria
            {
                public int MaxMinutes { get; set; }

                public DistanceType Type { get; set; }

                public string Target { get; set; }

                public enum DistanceType
                {
                    Walking,
                    Biking,
                    Commuting
                }
            }
        }

        public class SearchResult
        {
            public string Area { get; set; }

            public string City { get; set; }

            public string Address { get; set; }

            public string Price { get; set; }

            public string Fee { get; set; }

            public string Size { get; set; }

            public string Rooms { get; set; }

            public string ImageUrl { get; set; }

            public string Url { get; set; }
        }
    }
}