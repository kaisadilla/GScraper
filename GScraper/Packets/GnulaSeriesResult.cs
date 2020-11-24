namespace Kaisa.GScraper.Packets {
    public struct GnulaSeriesResult {
        public readonly string name;
        public readonly string imageUrl;
        public readonly string year;
        public readonly string pageUrl;

        public GnulaSeriesResult(string name, string imageUrl, string year, string pageUrl) {
            this.name = name;
            this.imageUrl = imageUrl;
            this.year = year;
            this.pageUrl = pageUrl;
        }
    }
}
