using System.Collections.Generic;
using System.Text.Encodings.Web;

namespace Flyttaihop.Framework.Extensions
{
    public static class ListExtensions
    {
        public static string JoinUrlEncoded(this IEnumerable<string> list, string separator)
        {
            return UrlEncoder.Default.Encode(string.Join(separator, list));
        }
    }
}