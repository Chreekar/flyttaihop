using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flyttaihop.Configuration;
using Flyttaihop.Framework.Interfaces;
using Flyttaihop.Framework.Models;
using Flyttaihop.Framework.Parsers;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Flyttaihop.Controllers
{
    [Route("api/[controller]")]
    public class SearchController : Controller
    {
        private readonly IOptions<ApplicationOptions> _applicationOptions;
        private readonly ILogger<SearchController> _logger;
        private readonly ICriteriaRepository _criteriaRepository;
        private readonly HemnetParser _hemnetParser;
        private readonly GoogleParser _googleParser;

        public SearchController(IOptions<ApplicationOptions> applicationOptions, ILogger<SearchController> logger, ICriteriaRepository criteriaRepository, HemnetParser hemnetParser, GoogleParser googleParser)
        {
            _applicationOptions = applicationOptions;
            _logger = logger;
            _criteriaRepository = criteriaRepository;
            _hemnetParser = hemnetParser;
            _googleParser = googleParser;
        }

        [HttpGet()]
        ///<summary>Gör en sökning mot Hemnet och Google Maps Directions med de sparade kriterierna</summary>
        public async Task<List<SearchResult>> Search()
        {
            var criteria = _criteriaRepository.GetSavedCriteria();

            string googleApiKey = _applicationOptions.Value.GoogleApiKey;

            if (string.IsNullOrWhiteSpace(googleApiKey))
            {
                _logger.LogError("GoogleApiKey not specified, skipping duration calculations");
            }

            var hemnetDoc = await _hemnetParser.GetDocument(criteria);

            var itemContainerNodes = hemnetDoc.GetElementbyId("search-results").Elements("li");

            var result = await Task.WhenAll(itemContainerNodes.Select(itemContainerNode => ProcessNode(criteria, itemContainerNode, googleApiKey)));

            return result.Where(r => r != null).ToList();
        }

        private async Task<SearchResult> ProcessNode(Criteria criteria, HtmlNode itemContainerNode, string googleApiKey)
        {
            var itemNode = itemContainerNode.Elements("div").Single();

            var item = _hemnetParser.ParseNode(itemNode);

            if (item != null)
            {
                item = await _googleParser.ParseItem(criteria, item, googleApiKey);
            }

            return item;
        }
    }
}