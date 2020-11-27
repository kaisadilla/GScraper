using System;
using System.Collections.Generic;
using System.Text;

namespace Kaisa.GScraper.SQLite.Models {
    public struct Episode {
        public readonly string fullId;
        public readonly string series;
        public readonly int seasonIndex;
        public readonly int episodeIndex;
        public readonly string name;
        public readonly string date;
        public readonly string imgPath;
        public readonly float rating;

        public Episode (string fullId, string series, int seasonIndex, int episodeIndex, string name, string date, string imgPath, float rating) {
            this.fullId = fullId;
            this.series = series;
            this.seasonIndex = seasonIndex;
            this.episodeIndex = episodeIndex;
            this.name = name;
            this.date = date;
            this.imgPath = imgPath;
            this.rating = rating;
        }
    }
}
