using System;
using System.Collections.Generic;
using System.Text;

namespace Kaisa.GScraper.Scraper.Packets {
    public struct SeriesEpisode {
        public readonly string episodeName;
        public readonly string episodeUrl;

        public SeriesEpisode(string episodeName, string episodeUrl) {
            this.episodeName = episodeName;
            this.episodeUrl = episodeUrl;
        }
    }
}
