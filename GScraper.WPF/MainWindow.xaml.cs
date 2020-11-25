﻿using Kaisa.GScraper.Scraper.Pages;
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
            page_search = new SeriesSearch();
            frame_main.Navigate(page_search);
        }

        public Frame DisplayFrame => frame_main;

        public SeriesSearch page_search;
        public SeriesImportOptions page_options;
        public int page_download;
    }
}
