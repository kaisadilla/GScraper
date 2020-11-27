using System;
using System.Collections.Generic;
using System.Text;

namespace Kaisa.GScraper.SQLite.Models {
    public struct Link {
        public readonly string url;
        public readonly string episode;
        public readonly LinkType linkType;
        public readonly string hostName;
        public readonly string quality;
        public readonly string language;
        public readonly float favorite;
        public readonly int badLink;

        public Link (string url, string episode, LinkType linkType, string hostName, string quality, string language, int favorite, int badLink) {
            this.url = url;
            this.episode = episode;
            this.linkType = linkType;
            this.hostName = hostName;
            this.quality = quality;
            this.language = language;
            this.favorite = favorite;
            this.badLink = badLink;
        }
    }
}
