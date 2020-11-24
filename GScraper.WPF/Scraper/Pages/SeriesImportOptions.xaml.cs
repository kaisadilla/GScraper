using Kaisa.GScraper.Scraper.Packets;
using Kaisa.GScraper.Scraper.UserControls;
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
        }

        /// <summary>
        /// Builds the episodes' panels for each season. The name of each season will be "Season {index + 1}".
        /// The name of each episode will be the number of that episode.
        /// </summary>
        /// <param name="episodesPerSeason">An array of integers, representing the amount of episodes in each seasons.</param>
        public void CreateSeasonPanels (int[] episodesPerSeason) {
            for (int i = 0; i < episodesPerSeason.Length; i++) {
                var eSelector = new EpisodeSelector {
                    Width = 200,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(0, 0, 10, 10),
                };
                eSelector.AddEpisodes($"Season {i + 1}", episodesPerSeason[i]);
                list_seasons.Children.Add(eSelector);
            }
        }
    }
}
