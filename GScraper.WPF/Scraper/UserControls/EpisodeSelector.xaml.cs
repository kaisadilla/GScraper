using Kaisa.GScraper.Scraper.Packets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kaisa.GScraper.Scraper.UserControls {
    /// <summary>
    /// Interaction logic for EpisodeSelector.xaml
    /// </summary>
    public partial class EpisodeSelector : UserControl {
        private const int ROW_HEIGHT = 35;
        private const int EPISODES_PER_ROW = 5;
        private static Color TOGGLE_COLOR_ACTIVE = (Color)ColorConverter.ConvertFromString("#FFDC7823");

        public EpisodeSelector() {
            InitializeComponent();
            DataContext = this;
        }

        public string Legend { get; set; }


        private void toggle_season_Click(object sender, RoutedEventArgs e) {
            ToggleAll(toggle_season.IsChecked == true);
        }

        private List<ToggleButton> episodeToggles;

        /// <summary>
        /// Returns true if all the episodes of the control are toggled on.
        /// </summary>
        private bool AreAllEpisodesToggled => episodeToggles.TrueForAll(e => e.IsChecked == true);

        /// <summary>
        /// Adds a toggle for each episode, with its text being the number of that episode,
        /// and resizes the control to fit all episodes.
        /// </summary>
        /// <param name="legend">The name of the season.</param>
        /// <param name="episodes">An array of ints containing all the episodes.</param>
        public void AddEpisodes (string legend, int episodeCount) {
            list_episodes.Children.Clear();
            toggle_season.IsChecked = true;
            episodeToggles = new List<ToggleButton>();
            Legend = legend;
            int rows = (int)Math.Ceiling(episodeCount / (double)EPISODES_PER_ROW);
            Height = ROW_HEIGHT + (ROW_HEIGHT * rows);

            for (int i = 0; i < episodeCount; i++) {
                var eToggle = new ToggleButton {
                    Width = ROW_HEIGHT - 1,
                    Height = ROW_HEIGHT - 1,
                    BorderThickness = new Thickness(0),
                    Margin = new Thickness(1, 1, 0, 0),
                    //Background = new SolidColorBrush(TOGGLE_COLOR_ACTIVE),
                    FontSize = 14d * (96d / 72d),
                    Content = $"{i + 1}",
                    IsChecked = true
                };
                eToggle.Style = FindResource("GScraper.Series.ToggleButton") as Style;
                //eToggle.Click += (sender, e) => toggle_season.IsChecked = false; // why can't I untoggle the master toggle? Curious.
                list_episodes.Children.Add(eToggle);
                episodeToggles.Add(eToggle);
            }
        }

        /// <summary>
        /// Returns an array of booleans, each representing if the episode at that index was chosen for download.
        /// </summary>
        /// <returns></returns>
        public bool[] GetChosenEpisodes () {
            bool[] chosenEpisodes = new bool[episodeToggles.Count];

            for (int i = 0; i < episodeToggles.Count; i++) {
                chosenEpisodes[i] = episodeToggles[i].IsChecked == true;
            }

            return chosenEpisodes;
        }

        /// <summary>
        /// Toggles all the episodes in the control on or off.
        /// </summary>
        /// <param name="toggled"></param>
        private void ToggleAll (bool toggled) {
            foreach (var eToggle in episodeToggles) {
                eToggle.IsChecked = toggled;
            }
        }
    }
}
