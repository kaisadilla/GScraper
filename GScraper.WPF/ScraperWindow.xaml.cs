using Kaisa.GScraper.WPF.Scraper.Pages;
using Kaisa.GScraper.WPF.Scraper.UserControls;
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
    public partial class ScraperWindow : Window {
        public ScraperWindow() {
            BindingObjects.Initialize();
            BindingObjects.InitializeScraper();
            InitializeComponent();
            page_search = new SeriesSearch();
            frame_main.Navigate(page_search);
        }

        public Frame DisplayFrame => frame_main;

        public SeriesSearch page_search;
        public SeriesImportOptions page_options;
        public SeriesImportProgress page_download;
    }
}
