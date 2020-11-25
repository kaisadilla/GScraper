using System;
using System.Collections.Generic;
using System.Text;

namespace Kaisa.GScraper.Packets {
    public struct SeriesStructure {
        public readonly string internalName;
        public readonly string name;
        public readonly string imgUrl;
        public readonly int[] episodes;

        public SeriesStructure(string internalName, string name, string imgUrl, int[] episodes) {
            this.internalName = internalName;
            this.name = name;
            this.imgUrl = imgUrl;
            this.episodes = episodes;
        }
    }
}
