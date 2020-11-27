using HtmlAgilityPack;
using Kaisa.GScraper.Exceptions;
using Kaisa.GScraper.Navigator;
using Kaisa.GScraper.Packets;
using Kaisa.GScraper.SQLite;
using OpenQA.Selenium;
using ScrapySharp.Extensions;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Kaisa.GScraper.Scrapers {
    public class ScraperGnula {
        private readonly PageLoader loader;

        public ScraperGnula() {
            loader = new PageLoader();
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

        /// <summary>
        /// Scrapes the main page of a website to find a link to every episode, and then scrapes every episode link to find all
        /// links to videos. It ignores any episodes marked as false in chosenEpisodes, and will throw exceptions if the number of
        /// seasons or episodes provided does not match the number of seasons or episodes found. After it scrapes all links in an
        /// episode, it returns the index of the episode and season scraped.
        /// </summary>
        /// <param name="url">The url of the main page of the series in gnula.cc</param>
        /// <param name="chosenEpisodes">An array containing the episodes that should be downloaded (true) or ignored (false)
        /// in each season.</param>
        public async IAsyncEnumerable<(int season, int episode)> ScrapeSeries(string url, bool[][] chosenEpisodes = null) {
            var conds = ExpectedConditions.ElementExists(By.Id("episodes"));
            HtmlNode page = loader.LoadDynamicWebpage(url, conds);
            if (page == null) throw new PageNotFoundException();

            string internalName = url.Split("/")[url.EndsWith("/") ? ^2 : ^1].ToLower();
            string seriesName = page.CssSelect(".data h1").First().InnerHtml ?? "series_name_not_found";
            // create directories for the images of the series
            string seriesImgFolder = $@"img\{internalName}";
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + seriesImgFolder);

            string seriesImgUrl = page.CssSelect(".poster img").First()?.Attributes["src"]?.Value;
            string seriesImgPath = $@"{seriesImgFolder}\poster.{seriesImgUrl.Split('.')[^1]}";
            DownloadImage(seriesImgUrl, AppDomain.CurrentDomain.BaseDirectory + seriesImgPath);

            Database.InsertSeries(internalName, seriesName, seriesImgPath);
            Console.WriteLine($"Scraping series: {seriesName}");

            var html_seasons = page.CssSelect("#episodes .episodios");

            if (chosenEpisodes != null && chosenEpisodes.Length != html_seasons.Count()) {
                Console.WriteLine("The number of seasons submitted to scrape does not match the number of seasons found. Operation aborted.");
                throw new BadRequestException();
            }
            else {
                Console.WriteLine($"Seasons found: {html_seasons.Count()}");
            }

            for (int sIndex = 0; sIndex < html_seasons.Count(); sIndex++) {
                int seasonNumber = sIndex + 1;
                var html_episodes = html_seasons.ElementAt(sIndex).CssSelect("li");
                if (chosenEpisodes != null && chosenEpisodes[sIndex].Length != html_episodes.Count()) {
                    Console.WriteLine($"The number of episodes submitted for season {seasonNumber} does not match the number of episodes found. Operation aborted");
                    throw new BadRequestException();
                }
                else {
                    Console.WriteLine($"Episodes found for season {seasonNumber}: {html_episodes.Count()}");
                }

                for (int eIndex = 0; eIndex < html_episodes.Count(); eIndex++) {
                    if (chosenEpisodes[sIndex][eIndex] == false) continue;
                    yield return (sIndex, eIndex);
                    string episodePageLink = null;
                    string episodeFullId = null;
                    await Task.Run(() => {
                        var episode = html_episodes.ElementAt(eIndex);
                        int episodeNumber = eIndex + 1;
                        episodeFullId = Database.BuildEpisodeId(internalName, seasonNumber, episodeNumber);

                        string episodeImgUrl = episode.CssSelect(".imagen img").First()?.Attributes["src"]?.Value;
                        string episodeImgPath = $@"{seriesImgFolder}\{episodeFullId}.{episodeImgUrl.Split('.')[^1]}";

                        DownloadImage(episodeImgUrl, AppDomain.CurrentDomain.BaseDirectory + episodeImgPath);

                        string episodeName = episode.CssSelect(".episodiotitle a").First()?.InnerHtml;
                        episodePageLink = episode.CssSelect(".episodiotitle a").First()?.Attributes["href"]?.Value;
                        string episodeDate = episode.CssSelect(".date").First()?.InnerHtml;


                        Console.WriteLine($"{seasonNumber}x{episodeNumber}: {episodeName} ({episodePageLink}) [{episodeDate}]");
                        Database.InsertEpisode(episodeFullId, internalName, seasonNumber, episodeNumber, episodeName, episodeDate, episodeImgPath);
                        ScrapeEpisodePage(episodePageLink, episodeFullId);
                    });

                    /*await foreach (string link in ScrapeEpisodePage(episodePageLink, episodeFullId)) {
                        yield return (sIndex, eIndex, link);
                    }*/
                }
            }
        }

        private string ConvertUrlToInternalName(string seriesUrl) {
            return seriesUrl.Split("/")[seriesUrl.EndsWith("/") ? ^2 : ^1].ToLower();
        }

        private void DownloadImage(string url, string savePath) {
            using (WebClient client = new WebClient()) {
                Console.WriteLine($"Downloading image in {url}...");
                try {
                    client.DownloadFile(new Uri(url), savePath);
                    Console.WriteLine($"Image saved in {savePath}");
                }
                catch (WebException ex) {
                    Console.WriteLine($"Image couldn't be saved. {ex.Message}");
                }
            }
        }
        private void ScrapeEpisodePage(string episodeUrl, string episodeFullId) {
            var conds = ExpectedConditions.ElementExists(By.Id("videos"));
            HtmlNode page = loader.LoadDynamicWebpage(episodeUrl, conds);
            if (page == null) throw new PageNotFoundException();

            string url = null;
            var watchOnlineTable = page.CssSelect("#videos table > tbody > tr");
            foreach (var row in watchOnlineTable) {
                ParseLinkFromRow(row, out url, out string hostName, out string quality, out string language);
                Console.WriteLine($"Type: {LinkType.watchOnline} - {hostName} / {quality} / {language}: {url}");
                Database.InsertLink(url, episodeFullId, (int)LinkType.watchOnline, hostName, quality, language);
            }

            var downloadTable = page.CssSelect("#download table > tbody > tr");
            foreach (var row in downloadTable) {
                ParseLinkFromRow(row, out url, out string hostName, out string quality, out string language);
                Console.WriteLine($"Type: {LinkType.download} - {hostName} / {quality} / {language}: {url}");
                Database.InsertLink(url, episodeFullId, (int)LinkType.download, hostName, quality, language);
            }
        }

        /*private async IAsyncEnumerable<string> ScrapeEpisodePage (string episodeUrl, string episodeFullId) {
            var conds = ExpectedConditions.ElementExists(By.Id("videos"));
            HtmlNode page = loader.LoadDynamicWebpage(episodeUrl, conds);
            if (page == null) throw new PageNotFoundException();

            string url = null;
            var watchOnlineTable = page.CssSelect("#videos table > tbody > tr");
            foreach (var row in watchOnlineTable) {
                await Task.Run(() => {
                    ParseLinkFromRow(row, out url, out string hostName, out string quality, out string language);
                    Console.WriteLine($"Type: {LinkType.watchOnline} - {hostName} / {quality} / {language}: {url}");
                    Database.InsertLink(url, episodeFullId, (int)LinkType.watchOnline, hostName, quality, language);
                });
                yield return url;
            }

            var downloadTable = page.CssSelect("#download table > tbody > tr");
            foreach (var row in downloadTable) {
                await Task.Run(() => {
                    ParseLinkFromRow(row, out url, out string hostName, out string quality, out string language);
                    Console.WriteLine($"Type: {LinkType.download} - {hostName} / {quality} / {language}: {url}");
                    Database.InsertLink(url, episodeFullId, (int)LinkType.download, hostName, quality, language);
                });
                yield return url;
            }
        }*/

        private void ParseLinkFromRow(HtmlNode row, out string url, out string hostName, out string quality, out string language) {
            var columns = row.CssSelect("td");

            url = RefineLink(columns.ElementAt(0).CssSelect("a").First()?.Attributes["href"]?.Value);
            hostName = columns.ElementAt(1).CssSelect("a").First()?.InnerHtml;
            quality = columns.ElementAt(2).CssSelect(".quality")?.First().InnerHtml;
            language = CalcLanguage(columns.ElementAt(3).CssSelect("img").First()?.Attributes["src"]?.Value);
        }

        private string CalcLanguage(string imgUrl) {
            if (imgUrl.EndsWith("/flags/es.png")) return "Spanish";
            if (imgUrl.EndsWith("/flags/mx.png")) return "Latin Spanish";
            if (imgUrl.EndsWith("/flags/en.png")) return "English";
            if (imgUrl.EndsWith("/flags/jp.png")) return "English (Spanish subs)"; // Yes, jp.png is the name of the Spanish/English icon.
            return "unknown";
        }

        private string RefineLink(string rawLink) {
            if (rawLink.Contains("powvideo.net")) {
                // input: https://powvideo.net/embed-tysob516aw4x.html
                // output: https://powvideo.net/tysob516aw4x
                string videoId = rawLink.Replace("https://powvideo.net/embed-", "").Replace(".html", "");
                return $"https://powvideo.net/{videoId}";
            }
            return rawLink;
        }
    }
}
