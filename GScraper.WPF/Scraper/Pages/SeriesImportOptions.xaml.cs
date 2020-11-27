using Kaisa.GScraper.Scraper.Packets;
using Kaisa.GScraper.Scraper.UserControls;
using Kaisa.GScraper.SQLite;
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
    /// Interaction logic for SeriesImportOptions.xaml
    /// </summary>
    public partial class SeriesImportOptions : Page {
        public SeriesImportOptions() {
            InitializeComponent();
            DataContext = this;
        }

        public string SeriesUrl { get; set; }
        public string ImgUrl { get; set; }
        public string InternalName { get; set; }
        public string SeriesName { get; set; }

        private void arrow_goBack_Click(object sender, RoutedEventArgs e) {
            var window = (MainWindow)Window.GetWindow(this);
            window.DisplayFrame.Navigate(window.page_search);
        }

        private void button_import_Click(object sender, RoutedEventArgs e) {
            OpenDownloadPage();
        }

        private void button_unmarkDownloaded_Click(object sender, RoutedEventArgs e) {
            ToggleOffAlreadyScrapedEpisodes();
        }

        private List<EpisodeSelector> seasons;

        /// <summary>
        /// Builds the episodes' panels for each season. The name of each season will be "Season {index + 1}".
        /// The name of each episode will be the number of that episode.
        /// </summary>
        /// <param name="episodesPerSeason">An array of integers, representing the amount of episodes in each seasons.</param>
        public void CreateSeasonPanels (int[] episodesPerSeason) {
            seasons = new List<EpisodeSelector>();
            for (int i = 0; i < episodesPerSeason.Length; i++) {
                var eSelector = new EpisodeSelector {
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(0, 0, 10, 10),
                };
                eSelector.AddEpisodes($"Season {i + 1}", episodesPerSeason[i]);
                list_seasons.Children.Add(eSelector);
                seasons.Add(eSelector);
            }
        }

        private void ToggleOffAlreadyScrapedEpisodes() {
            for (int s = 0; s < seasons.Count; s++) {
                for (int e = 0; e < seasons[s].EpisodeCount; e++) {
                    var dbEpisode = Database.RetrieveEpisodeById(Database.BuildEpisodeId(InternalName, s + 1, e + 1));
                    if (dbEpisode != null) seasons[s].ToggleEpisode(e, false);
                }
            }
        }

        private bool[][] GetChosenEpisodes () {
            bool[][] chosenEpisodes = new bool[seasons.Count][];

            for (int i = 0; i < seasons.Count; i++) {
                chosenEpisodes[i] = seasons[i].GetChosenEpisodes();
            }

            return chosenEpisodes;
        }

        private void OpenDownloadPage () {
            var window = (MainWindow)Window.GetWindow(this);
            window.page_download = new SeriesImportProgress {
                SeriesName = SeriesName,
                ImgUrl = ImgUrl,
            };
            window.page_download.Initialize(SeriesUrl, GetChosenEpisodes());
            window.DisplayFrame.Navigate(window.page_download);
        }
    }
}
