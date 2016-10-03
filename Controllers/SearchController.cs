using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flyttaihop.Configuration;
using Flyttaihop.Framework.Interfaces;
using Flyttaihop.Framework.Models;
using Flyttaihop.Framework.Parsers;
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

            var result = new List<SearchResult>();

            var hemnetDoc = await _hemnetParser.GetDocument(criteria);

            //TODO: Kör loopen asynkront så att vi väntar in alla varv efteråt innan vi returnerar listan  
            //kanske så här: http://stackoverflow.com/questions/23137393/parallel-foreach-and-async-await
            foreach (var itemContainerNode in hemnetDoc.GetElementbyId("search-results").Elements("li"))
            {
                var itemNode = itemContainerNode.Elements("div").Single();

                var item = _hemnetParser.ParseNode(itemNode);

                if (item != null)
                {
                    item = await _googleParser.ParseItem(criteria, item, googleApiKey);

                    if (item != null)
                    {
                        result.Add(item);
                    }
                }
            }

            return result;
        }

    }
}