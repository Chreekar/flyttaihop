using System.Collections.Generic;

namespace Flyttaihop.Framework.Models
{
    public class Criteria
    {
        public IEnumerable<string> Keywords { get; set; }

        public IEnumerable<Duration> DurationCriterias { get; set; }
    }
}