using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Flyttaihop.Framework.Extensions
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object item)
        {
            session.SetString(key, JsonConvert.SerializeObject(item));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var storedString = session.GetString(key);

            return storedString == null ? default(T) : JsonConvert.DeserializeObject<T>(storedString);
        }
    }
}