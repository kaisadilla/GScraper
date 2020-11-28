using System;
using System.Collections.Generic;
using System.Text;

namespace Kaisa.GScraper.SQLite.Models {
    public struct Series {
        public readonly string internalName;
        public readonly string displayName;
        public readonly string imgPath;
        public readonly bool favorite;
        public readonly float rating;

        public Series(string internalName, string displayName, string imgPath, bool favorite, float rating) {
            this.internalName = internalName;
            this.displayName = displayName;
            this.imgPath = imgPath;
            this.favorite = favorite;
            this.rating = rating;
        }
    }
}
