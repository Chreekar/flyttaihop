using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Flyttaihop.Framework.Extensions;
using Flyttaihop.Framework.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Caching.Distributed;

namespace Flyttaihop.Framework.Parsers
{
    public class HemnetParser
    {
        private readonly IDistributedCache _cache;

        public HemnetParser(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<HtmlDocument> GetDocument(Criteria criteria, int page)
        {
            string url = GetUrl(criteria, page);

            string htmlContent = await _cache.GetStringAsync(url);

            if (htmlContent == null)
            {
                using (var hemnetClient = new HttpClient())
                {
                    hemnetClient.BaseAddress = new Uri("http://www.hemnet.se");
                    var result = await hemnetClient.GetAsync(url);
                    htmlContent = await result.Content.ReadAsStringAsync();
                    await _cache.SetStringAsync(url, htmlContent, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2) });
                }
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);
            return doc;
        }

        public SearchResult ParseNode(HtmlNode itemNode)
        {
            try
            {
                var detailsNode = itemNode.Elements("div").Where(n => n.GetAttributeValue("class", "").Contains("listing-post__details")).Single();

                string addressString = detailsNode
                                .Elements("ul")
                                .Where(n => n.GetAttributeValue("class", "").Contains("location-type"))
                                .Single()
                                .Elements("li")
                                .Where(n => n.GetAttributeValue("class", "").Contains("address"))
                                .Single()
                                .InnerText
                                .TrimEnd(' ', '\n');

                return new SearchResult
                {
                    Area = detailsNode
                                .Elements("ul")
                                .Where(n => n.GetAttributeValue("class", "").Contains("location-type"))
                                .Single()
                                .Elements("li")
                                .Where(n => n.GetAttributeValue("class", "").Contains("area"))
                                .Single()
                                .InnerText
                                .Trim(),
                    City = detailsNode
                                .Elements("ul")
                                .Where(n => n.GetAttributeValue("class", "").Contains("location-type"))
                                .Single()
                                .Elements("li")
                                .Where(n => n.GetAttributeValue("class", "").Contains("city"))
                                .Single()
                                .InnerText
                                .Trim(),
                    Address = addressString.Contains("\n") ?
                                addressString.Split('\n').Last().Trim() :
                                addressString.Trim(),
                    Price = detailsNode
                                .Elements("ul")
                                .Where(n => n.GetAttributeValue("class", "").Contains("prices"))
                                .Single()
                                .Elements("li")
                                .Where(n => n.GetAttributeValue("class", "").Contains("price"))
                                .Single()
                                .InnerText
                                .Trim(),
                    Fee = detailsNode
                                .Elements("ul")
                                .Where(n => n.GetAttributeValue("class", "").Contains("prices"))
                                .Single()
                                .Elements("li")
                                .Where(n => n.GetAttributeValue("class", "").Contains("fee"))
                                .Single()
                                .InnerText
                                .Trim(),
                    Size = detailsNode
                                .Elements("ul")
                                .Where(n => n.GetAttributeValue("class", "").Contains("size"))
                                .Single()
                                .Elements("li")
                                .Where(n => n.GetAttributeValue("class", "").Contains("living-area"))
                                .Single()
                                .InnerText
                                .Trim(),
                    Rooms = detailsNode
                                .Elements("ul")
                                .Where(n => n.GetAttributeValue("class", "").Contains("size"))
                                .Single()
                                .Elements("li")
                                .Where(n => n.GetAttributeValue("class", "").Contains("rooms"))
                                .Single()
                                .InnerText.Replace("&nbsp;", "")
                                .Trim(),
                    ImageUrl = itemNode
                                .Elements("div")
                                .Where(n => n.GetAttributeValue("class", "").Contains("image"))
                                .Single()
                                .Elements("img")
                                .Single()
                                .GetAttributeValue("data-src", ""),
                    Url = "http://www.hemnet.se" + itemNode
                                .Elements("a")
                                .Where(n => n.GetAttributeValue("class", "").Contains("item-link-container"))
                                .Single()
                                .GetAttributeValue("href", "")
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        #region Helpers

        private string GetUrl(Criteria criteria, int page)
        {
            string url = "/bostader?item_types%5B%5D=bostadsratt&upcoming=1&price_max=4000000&rooms_min=2&living_area_min=60&location_ids%5B%5D=17744&page=" + page;

            if (criteria.Keywords.Any())
            {
                url += "&keywords=" + criteria.Keywords.Select(x => x.Text).JoinUrlEncoded(",");
            }

            return url;
        }

        #endregion
    }
}