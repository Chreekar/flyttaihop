using System;
using System.Linq;
using Flyttaihop.Framework.Models;
using HtmlAgilityPack;

namespace Flyttaihop.Framework.Parsers
{
    public class HemnetParser
    {
        public SearchResult ParseNode(HtmlNode itemNode)
        {
            try
            {
                return new SearchResult
                {
                    Area = itemNode
                                .Elements("ul")
                                .Where(n => n.GetAttributeValue("class", "").Contains("location-type"))
                                .Single()
                                .Elements("li")
                                .Where(n => n.GetAttributeValue("class", "").Contains("area"))
                                .Single()
                                .InnerText
                                .Trim(),
                    City = itemNode
                                .Elements("ul")
                                .Where(n => n.GetAttributeValue("class", "").Contains("location-type"))
                                .Single()
                                .Elements("li")
                                .Where(n => n.GetAttributeValue("class", "").Contains("city"))
                                .Single()
                                .InnerText
                                .Trim(),
                    Address = itemNode
                                .Elements("ul")
                                .Where(n => n.GetAttributeValue("class", "").Contains("location-type"))
                                .Single()
                                .Elements("li")
                                .Where(n => n.GetAttributeValue("class", "").Contains("address"))
                                .Single()
                                .InnerText
                                .Trim(),
                    Price = itemNode
                                .Elements("ul")
                                .Where(n => n.GetAttributeValue("class", "").Contains("prices"))
                                .Single()
                                .Elements("li")
                                .Where(n => n.GetAttributeValue("class", "").Contains("price"))
                                .Single()
                                .InnerText
                                .Trim(),
                    Fee = itemNode
                                .Elements("ul")
                                .Where(n => n.GetAttributeValue("class", "").Contains("prices"))
                                .Single()
                                .Elements("li")
                                .Where(n => n.GetAttributeValue("class", "").Contains("fee"))
                                .Single()
                                .InnerText
                                .Trim(),
                    Size = itemNode
                                .Elements("ul")
                                .Where(n => n.GetAttributeValue("class", "").Contains("size"))
                                .Single()
                                .Elements("li")
                                .Where(n => n.GetAttributeValue("class", "").Contains("living-area"))
                                .Single()
                                .InnerText
                                .Split(new string[] { "&nbsp;" }, StringSplitOptions.RemoveEmptyEntries)[0]
                                .Trim(),
                    Rooms = itemNode
                                .Elements("ul")
                                .Where(n => n.GetAttributeValue("class", "").Contains("size"))
                                .Single()
                                .Elements("li")
                                .Where(n => n.GetAttributeValue("class", "").Contains("living-area"))
                                .Single()
                                .InnerText
                                .Split(new string[] { "&nbsp;" }, StringSplitOptions.RemoveEmptyEntries)[1]
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
    }
}