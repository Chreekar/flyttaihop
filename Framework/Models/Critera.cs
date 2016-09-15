using System.Collections.Generic;

namespace Flyttaihop.Framework.Models
{
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