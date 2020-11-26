#define NO_CONNECTION
#undef NO_CONNECTION

using Kaisa.GScraper.Exceptions;
using Kaisa.GScraper.Packets;
using Kaisa.GScraper.Scraper.UserControls;
using Kaisa.GScraper.WPF;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kaisa.GScraper.Scraper.Pages {
    /// <summary>
    /// Interaction logic for SeriesSearch.xaml
    /// </summary>
    public partial class SeriesSearch : Page {
        public SeriesSearch() {
            InitializeComponent();
        }

        private void button_search_Click(object sender, RoutedEventArgs e) {
            ParseQuery(SearchQuery);
        }

        private void textBox_searchQuery_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter || e.Key == Key.Return) {
                button_search_Click(sender, e);
            }
        }

        private string SearchQuery {
            get => textBox_searchQuery.Text;
            set => textBox_searchQuery.Text = value;
        }

        private void ParseQuery(string query) {
            if (query.Contains("https://") || query.Contains("http://")) {
#if NO_CONNECTION
                NavigateSimulated();
#else
                NavigateToPage("test.url");
#endif
            }
            else if (!string.IsNullOrEmpty(query)) {
                SearchAndDisplay(query);
            }
        }

        private void SearchAndDisplay(string query) {
            list_searchResults.Items.Clear();
            loading_searchingQuery.Visibility = Visibility.Visible;
            var results = BindingObjects.Scraper.ScrapeSearchResult(query);

            foreach (var r in results) {
                var resultEntry = new WebSearchResult {
                    SeriesName = r.name,
                    ImgUrl = r.imageUrl,
                    Year = r.year
                };
                resultEntry.PreviewMouseDown += (sender, e) => NavigateToPage(r.pageUrl);

                list_searchResults.Items.Add(resultEntry);
            }
            loading_searchingQuery.Visibility = Visibility.Hidden;
        }

        private void NavigateToPage (string url) {
            try {
                var seriesStructure = BindingObjects.Scraper.ScrapeSeriesStructure(url);
                var window = (MainWindow)Window.GetWindow(this);
                window.page_options = new SeriesImportOptions {
                    SeriesUrl = url,
                    ImgUrl = seriesStructure.imgUrl,
                    SeriesName = seriesStructure.name,
                    InternalName = seriesStructure.internalName
                };
                window.page_options.CreateSeasonPanels(seriesStructure.episodes);
                window.DisplayFrame.Navigate(window.page_options);
            }
            catch (PageNotFoundException ex) {

            }
        }

        private void NavigateSimulated () {
            var window = (MainWindow)Window.GetWindow(this);
            window.page_options = new SeriesImportOptions {
                ImgUrl = @"C:\Users\DAW2\poster.jpg",
                SeriesName = "Test (2007)",
                InternalName = "id"
            };
            window.page_options.CreateSeasonPanels(new int[] { 22, 20, 18, 19, 20, 20, 23 });
            window.DisplayFrame.Navigate(window.page_options);
        }
    }
}
