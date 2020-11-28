using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows.Media;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Kaisa.GScraper.WPF {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public static Brush MOVIE_COLOR = new SolidColorBrush(Color.FromRgb(34, 177, 76)); // #FF22B14C
        public static Brush SERIES_COLOR = new SolidColorBrush(Color.FromRgb(220, 120, 35)); // #FFDC7823
    }
}
