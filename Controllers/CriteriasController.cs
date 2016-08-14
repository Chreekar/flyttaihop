using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Flyttaihop.Controllers
{
    [Route("api/[controller]")]
    public class CriteriasController : Controller
    {
        //TODO: Debug, byt ut mot databas
        private Criteria savedCriteria = new Criteria
        {
            Keywords = new List<string>(),
            DistanceCriterias = new List<Criteria.DistanceCriteria>()
        };

        [HttpGet()]
        ///<summary>Hämta tidigare sparad criteria</summary>
        public Criteria Get()
        {
            return savedCriteria;
        }

        [HttpPost()]
        ///<summary>Spara ny criteria (skriver över existerande om redan finns)</summary>
        public Criteria Post([FromBody]Criteria request)
        {
            savedCriteria = request;
            return savedCriteria;
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
    }
}