using Kaisa.GScraper.Scrapers;
using Kaisa.GScraper.SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kaisa.GScraper.WPF {
    public static class BindingObjects {
        public static ScraperGnula Scraper { get; private set; }

        public static void Initialize () {
            Database.Initialize();
        }

        public static void InitializeScraper () {
            Scraper = new ScraperGnula();
        }
    }
}
