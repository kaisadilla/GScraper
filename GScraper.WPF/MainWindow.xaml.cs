using Kaisa.GScraper.Scraper.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kaisa.GScraper.WPF {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            BindingObjects.Initialize();
            BindingObjects.InitializeScraper();
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

        private void ParseQuery (string query) {
            if (query.Contains("https://") || query.Contains("http://")) {
                // go to page directly
            }
            else if (!string.IsNullOrEmpty(query)) {
                SearchAndDisplay(query);
            }
        }

        private void SearchAndDisplay (string query) {
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
    }
}
