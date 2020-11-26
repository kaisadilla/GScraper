using Kaisa.GScraper.Scrapers;
using Kaisa.GScraper.SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kaisa.GScraper.WPF {
    public static class BindingObjects {
        public static ScraperGnula Scraper { get; private set; }

        public static void Initialize () {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/series.sqlite")) {
                Database.CreateDatabase(); // already initializes the db.
            }
            else {
                Database.Initialize();
            }
        }

        public static void InitializeScraper () {
            Scraper = new ScraperGnula();
        }
    }
}
