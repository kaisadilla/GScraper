using HtmlAgilityPack;
using Kaisa.GScraper.Exceptions;
using Kaisa.GScraper.Navigator;
using Kaisa.GScraper.Packets;
using OpenQA.Selenium;
using ScrapySharp.Extensions;
using SeleniumExtras.WaitHelpers;
using System.Collections.Generic;
using System.Linq;

namespace Kaisa.GScraper.Scrapers {
    public class ScraperGnula {
        private readonly PageLoader loader;

        public ScraperGnula() {
            loader = new PageLoader();
        }

        public void ScrapeSeries(string seriesUrl) {
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
            //return results.OrderBy(res => res.name).ToArray();
        }

        public SeriesStructure ScrapeSeriesStructure (string seriesUrl) {
            var conds = ExpectedConditions.ElementExists(By.Id("episodes"));
            HtmlNode page = loader.LoadDynamicWebpage(seriesUrl, conds);
            if (page == null) throw new PageNotFoundException();

            string internalName = ConvertUrlToInternalName(seriesUrl);
            string seriesName = page.CssSelect(".data h1")?.First()?.InnerHtml;
            string seriesImgUrl = page.CssSelect(".poster img")?.First()?.Attributes["src"]?.Value;

            var html_seasons = page.CssSelect("#episodes .episodios");
            int[] episodes = new int[html_seasons.Count()];

            for (int sIndex = 0; sIndex < html_seasons.Count(); sIndex++) {
                episodes[sIndex] = html_seasons.ElementAt(sIndex).CssSelect("li").Count();
            }

            return new SeriesStructure(internalName, seriesName, seriesImgUrl, episodes);
        }

        private string ConvertUrlToInternalName(string seriesUrl) {
            return seriesUrl.Split("/")[seriesUrl.EndsWith("/") ? ^2 : ^1].ToLower();
        }
    }
}
