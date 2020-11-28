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

namespace Kaisa.GScraper.WPF.LocalDatabase.UserControls {
    /// <summary>
    /// Interaction logic for ArticleResult.xaml
    /// </summary>
    public partial class ArticleResult : UserControl {
        public ArticleResult() {
            InitializeComponent();
            DataContext = this;
        }

        public string ImgPath { get; set; }
        public string ArticleName { get; set; }
        public string Year { get; set; }
    }
}
