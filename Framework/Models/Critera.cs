using System.Collections.Generic;

namespace Flyttaihop.Framework.Models
{
    public class Criteria
    {
        public int Id { get; set; }

        public List<Keyword> Keywords { get; set; }

        public List<Duration> DurationCriterias { get; set; }
    }
}