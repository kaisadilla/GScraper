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
    /// Interaction logic for SeriesImportProgress.xaml
    /// </summary>
    public partial class SeriesImportProgress : Page {
        public SeriesImportProgress() {
            InitializeComponent();
        }

        public void Initialize (int seasons) {
            System.Diagnostics.Debug.WriteLine("beforec");
            for (int i = 0; i < seasons; i++) {
                var cd = new ColumnDefinition {
                    Width = new GridLength(1, GridUnitType.Star),
                };
                progressBar.ColumnDefinitions.Add(cd);
                var rect = new Rectangle {
                    Fill = new SolidColorBrush(Colors.Red),
                    Stroke = new SolidColorBrush(Colors.Black),
                };
                rect.SetValue(Grid.ColumnProperty, i);
            }
            System.Diagnostics.Debug.WriteLine("afterc");
        }
    }
}
