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
                NavigateToPage("test.url");
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
                resultEntry.SetStandardSize();

                list_searchResults.Items.Add(resultEntry);
            }
            loading_searchingQuery.Visibility = Visibility.Hidden;
        }

        private void NavigateToPage(string url) {
            var window = (MainWindow)Window.GetWindow(this);
            var page_seriesImport = new SeriesImportOptions();
            page_seriesImport.CreateSeasonPanels(new int[] { 22, 22, 22, 20, 17, 12, 15, 26, 8, 7 });
            //page_seriesImport.CreateSeasonPanels(new int[] { 12, 22, 42 });
            window.DisplayFrame.Navigate(page_seriesImport);
        }
    }
}
