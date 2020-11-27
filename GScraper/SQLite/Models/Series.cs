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
    }
}
