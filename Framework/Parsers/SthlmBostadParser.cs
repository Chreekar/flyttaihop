using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Flyttaihop.Framework.Extensions;
using Flyttaihop.Framework.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Flyttaihop.Framework.Parsers
{
    public class SthlmBostadParser
    {
        private readonly IDistributedCache _cache;

        public SthlmBostadParser(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<object> GetData()
        {
            string jsonContent = await _cache.GetStringAsync("sthlmBostad");

            if (jsonContent == null)
            {
                using (var sthlmBostadClient = new HttpClient())
                {
                    sthlmBostadClient.BaseAddress = new Uri("https://bostad.stockholm.se");
                    var result = await sthlmBostadClient.GetAsync("/Lista/AllaAnnonser");
                    jsonContent = await result.Content.ReadAsStringAsync();
                    await _cache.SetStringAsync("sthlmBostad", jsonContent, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2) });
                }
            }

            return JsonConvert.DeserializeObject(jsonContent);
        }
    }
}