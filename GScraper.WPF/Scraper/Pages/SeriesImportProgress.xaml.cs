using Kaisa.GScraper.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
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

namespace Kaisa.GScraper.Scraper.Pages {
    /// <summary>
    /// Interaction logic for SeriesImportProgress.xaml
    /// </summary>
    public partial class SeriesImportProgress : Page {
        private static Brush PENDING_COLOR = new SolidColorBrush(Color.FromRgb(30, 30, 30));
        private static Brush CURRENT_COLOR = new SolidColorBrush(Color.FromRgb(245, 191, 5));
        private static Brush COMPLETED_COLOR = new SolidColorBrush(Color.FromRgb(220, 120, 35));
        private static Brush BORDER_COLOR = new SolidColorBrush(Colors.Transparent);

        public SeriesImportProgress() {
            InitializeComponent();
        }

        private string url;
        private bool[][] chosenEpisodes;

        public void Initialize (string url, bool[][] chosenEpisodes) {
            this.url = url;
            this.chosenEpisodes = chosenEpisodes;
            CreateSeasonBar(chosenEpisodes.Length);
        }

        private void button_scrape_Click (object sender, RoutedEventArgs e) {
            DownloadSeries();
        }

        private void button_cancel_Click (object sender, RoutedEventArgs e) {
            // cancel
        }

        private async void DownloadSeries () {
            button_scrape.IsEnabled = false;
            button_cancel.IsEnabled = true;
            int currentSeason = -1;

            await foreach ((int season, int episode) e in BindingObjects.Scraper.ScrapeSeries(url, chosenEpisodes)) {
                if (e.season != currentSeason) {
                    currentSeason = e.season;
                    CreateEpisodeBar(chosenEpisodes[currentSeason].Length);
                }
                SetSeasonCount(e.season);
                SetEpisodeCount(e.episode);
            }
            button_cancel.IsEnabled = true;
            button_scrape.IsEnabled = true;
        }

        private void CreateSeasonBar (int amount) {
            CreateLoadBar(seasonBar, amount);
        }

        private void CreateEpisodeBar (int amount) {
            CreateLoadBar(episodeBar, amount);
        }

        private void CreateLoadBar (Grid bar, int amount) {
            bar.ColumnDefinitions.Clear();
            bar.Children.Clear();
            for (int i = 0; i < amount; i++) {
                var cd = new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) };
                bar.ColumnDefinitions.Add(cd);
                var rect = new Rectangle {
                    Fill = PENDING_COLOR,
                    Stroke = BORDER_COLOR,
                };
                rect.SetValue(Grid.ColumnProperty, i);
                bar.Children.Add(rect);
            }
        }

        private void SetSeasonCount (int currentIndex) {
            for (int i = 0; i < seasonBar.Children.Count; i++) {
                (seasonBar.Children[i] as Rectangle).Fill = i < currentIndex ? COMPLETED_COLOR : i > currentIndex ? PENDING_COLOR : CURRENT_COLOR;
            }
        }

        private void SetEpisodeCount(int currentIndex) {
            for (int i = 0; i < episodeBar.Children.Count; i++) {
                (episodeBar.Children[i] as Rectangle).Fill = i < currentIndex ? COMPLETED_COLOR : i > currentIndex ? PENDING_COLOR : CURRENT_COLOR;
            }
        }
    }
}
