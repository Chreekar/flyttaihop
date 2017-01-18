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
    public class RentalController : Controller
    {
        private readonly IOptions<ApplicationOptions> _applicationOptions;
        private readonly ILogger<RentalController> _logger;
        private readonly SthlmBostadParser _sthlmBostadParser;
        private readonly GoogleParser _googleParser;

        public RentalController(IOptions<ApplicationOptions> applicationOptions, ILogger<RentalController> logger, SthlmBostadParser sthlmBostadParser, GoogleParser googleParser)
        {
            _applicationOptions = applicationOptions;
            _logger = logger;
            _sthlmBostadParser = sthlmBostadParser;
            _googleParser = googleParser;
        }

        [HttpGet()]
        ///<summary>Gör en sökning mot Stockholms bostadsförmedling och Google Maps Directions</summary>
        public async Task<object> Rental()
        {
            string googleApiKey = _applicationOptions.Value.GoogleApiKey;

            if (string.IsNullOrWhiteSpace(googleApiKey))
            {
                _logger.LogError("GoogleApiKey not specified, skipping duration calculations");
            }

            //TODO: Gör en klass istället för object
            
            object data = await _sthlmBostadParser.GetData();

            //TODO: Slå upp avstånd med GoogleParser

            return data;
        }
    }

}