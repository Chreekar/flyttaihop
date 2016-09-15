using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Flyttaihop.Framework.Extensions;
using Flyttaihop.Framework.Interfaces;
using Flyttaihop.Framework.Models;
using Flyttaihop.Framework.Parsers;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;

namespace Flyttaihop.Controllers
{
    [Route("api/[controller]")]
    public class SearchController : Controller
    {
        private readonly ICriteriaRepository _criteriaRepository;
        private readonly HemnetParser _hemnetParser;

        public SearchController(ICriteriaRepository criteriaRepository, HemnetParser hemnetParser)
        {
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

                foreach (var itemContainerNode in doc.GetElementbyId("search-results").Elements("li"))
                {
                    var itemNode = itemContainerNode.Elements("div").Single();

                    var sr = _hemnetParser.ParseNode(itemNode);

                    if (sr == null)
                    {
                        continue;
                    }

                    result.Add(sr);
                }
            }

            return result;
        }
    }
}