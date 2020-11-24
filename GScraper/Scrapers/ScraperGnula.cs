using HtmlAgilityPack;
using Kaisa.GScraper.SQLite;
using Kaisa.GScraper.Navigator;
using OpenQA.Selenium;
using ScrapySharp.Extensions;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Kaisa.GScraper.Packets;
using System.Threading.Tasks;

namespace Kaisa.GScraper.Scrapers {
    public class ScraperGnula {
        private readonly PageLoader loader;

        public ScraperGnula () {
            loader = new PageLoader();
        }

        public void ScrapeSeries (string seriesUrl) {
            //ScrapeSeriesMainPage();
        }

        public GnulaSeriesResult[] ScrapeSearchResult (string searchQuery) {
            searchQuery = searchQuery.ToLower().Replace(' ', '+');
            var conds = ExpectedConditions.ElementExists(By.ClassName("result-item"));
            HtmlNode page = loader.LoadDynamicWebpage($"https://www.gnula.cc/?s={searchQuery}", conds);

            var html_results = page.CssSelect(".result-item");
            var results = new List<GnulaSeriesResult>();

            foreach (var res in html_results) {
                string resName = res.CssSelect(".title a").First()?.InnerHtml;
                string resImageUrl = res.CssSelect(".image img").First()?.Attributes["src"]?.Value;
                string resPageUrl = res.CssSelect(".title a").First()?.Attributes["href"]?.Value;
                string resYear = res.CssSelect(".year").First()?.InnerHtml;
                results.Add(new GnulaSeriesResult(resName, resImageUrl, resYear, resPageUrl));
            }

            return results.ToArray();
        }
    }
}
