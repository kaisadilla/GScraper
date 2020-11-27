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
            DataContext = this;
        }
        public string ImgUrl { get; set; }
        public string SeriesName { get; set; }

        private string url;
        private bool[][] chosenEpisodes;
        private bool cancelationToken = false;

        public void Initialize (string url, bool[][] chosenEpisodes) {
            this.url = url;
            this.chosenEpisodes = chosenEpisodes;
            CreateSeasonBar(chosenEpisodes.Length);
            CreateEpisodeBar(chosenEpisodes[0].Length);
        }

        private void arrow_goBack_Click(object sender, RoutedEventArgs e) {
            var window = (MainWindow)Window.GetWindow(this);
            window.DisplayFrame.Navigate(window.page_options);
        }

        private void button_scrape_Click (object sender, RoutedEventArgs e) {
            DownloadSeries();
        }

        private void button_cancel_Click (object sender, RoutedEventArgs e) {
            button_cancel.IsEnabled = false;
            cancelationToken = true;
        }

        private async void DownloadSeries () {
            SetScrapeInProgress(true);

            int currentSeason = -1;
            await foreach ((int season, int episode) e in BindingObjects.Scraper.ScrapeSeries(url, chosenEpisodes)) {
                if (cancelationToken) {
                    cancelationToken = false;
                    break;
                }
                label_season.Content = $"Scraping season {e.season + 1} / {chosenEpisodes.Length}";
                label_episode.Content = $"Scraping episoded {e.episode + 1} / {chosenEpisodes[e.season].Length}";
                //label_link.Content = $"Adding link: {e.link}";
                if (e.season != currentSeason) {
                    currentSeason = e.season;
                    CreateEpisodeBar(chosenEpisodes[currentSeason].Length);
                }
                SetSeasonCount(e.season);
                SetEpisodeCount(e.episode);
            }

            SetScrapeInProgress(false);
        }

        private void SetScrapeInProgress (bool inProgress) {
            arrow_goBack.IsEnabled = !inProgress;
            button_scrape.IsEnabled = !inProgress;
            button_cancel.IsEnabled = inProgress;
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
