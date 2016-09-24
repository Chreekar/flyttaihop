namespace Flyttaihop.Framework.Models
{
    public class Duration
    {
        public int Minutes { get; set; }

        public TraversalType Type { get; set; }

        public string Target { get; set; }

        public enum TraversalType
        {
            Walking,
            Biking,
            Commuting
        }
    }
}