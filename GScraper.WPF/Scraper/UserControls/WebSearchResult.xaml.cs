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

namespace Kaisa.GScraper.Scraper.UserControls {
    /// <summary>
    /// Interaction logic for WebSearchResult.xaml
    /// </summary>
    public partial class WebSearchResult : UserControl {
        public WebSearchResult() {
            InitializeComponent();
            DataContext = this;
        }

        public void SetStandardSize () {
            Height = 95;
            Width = 600;
        }

        public string SeriesName { get; set; }
        public string ImgUrl { get; set; }
        public string Year { get; set; }
    }
}
