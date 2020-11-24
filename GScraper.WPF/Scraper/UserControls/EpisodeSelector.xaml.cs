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
        private const int EPISODES_PER_ROW = 5;

        public EpisodeSelector() {
            InitializeComponent();
            DataContext = this;
        }

        public string Legend { get; set; }


        private void toggle_season_Click(object sender, RoutedEventArgs e) {
            ToggleAll(toggle_season.IsChecked == true);
        }

        private List<ToggleButton> episodeToggles;

        //private bool AllEpisodesToggled => episodeToggles.TrueForAll(e => e.IsChecked == true);
        private bool AllEpisodesToggled {
            get {
                foreach (var e in episodeToggles) {
                    System.Diagnostics.Debug.WriteLine($"Episode {e} is toggled? {e.IsChecked}");
                    if (e.IsChecked == false) {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Adds a toggle for each episode, with its text being the number of that episode,
        /// and resizes the control to fit all episodes.
        /// </summary>
        /// <param name="legend">The name of the season.</param>
        /// <param name="episodes">An array of ints containing all the episodes.</param>
        public void AddEpisodes (string legend, int episodeCount) {
            list_episodes.Children.Clear();
            episodeToggles = new List<ToggleButton>();
            Legend = legend;
            int rows = (int)Math.Ceiling(episodeCount / (double)EPISODES_PER_ROW);
            Height = 40 + (40 * rows);

            for (int i = 0; i < episodeCount; i++) {
                var eToggle = new ToggleButton {
                    Width = 39,
                    Height = 39,
                    BorderThickness = new Thickness(0),
                    Margin = new Thickness(1, 1, 0, 0),
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDC7823")),
                    FontSize = 14d * (96d / 72d),
                    Content = $"{i + 1}",
                };
                eToggle.Click += (sender, e) => { toggle_season.IsChecked = false; System.Diagnostics.Debug.WriteLine(toggle_season.IsChecked); };
                list_episodes.Children.Add(eToggle);
                episodeToggles.Add(eToggle);
            }
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
