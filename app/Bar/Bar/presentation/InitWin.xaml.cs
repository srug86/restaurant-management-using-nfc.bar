using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;

namespace Bar.presentation
{
    /// <summary>
    /// Lógica de interacción para InitWin.xaml
    /// </summary>
    public partial class InitWin : Window
    {
        public InitWin()
        {
            InitializeComponent();
        }

        private void btnBegin_Click(object sender, RoutedEventArgs e)
        {
            startJourney();
        }

        private void btnEditProducts_Click(object sender, RoutedEventArgs e)
        {
            editProducts();
        }

        private void Close_Program(object sender, EventArgs e)
        {
            App.Current.Shutdown();
            foreach (Process p in Process.GetProcesses())
                if (p.ProcessName == "Bar.vshost") p.Kill();
        }

        private void startJourney()
        {
            JourneyManagerWin manager = new JourneyManagerWin();
            manager.Show();
        }

        private void editProducts()
        {
            EditProductsWin editor = new EditProductsWin();
            editor.Show();
        }

        private void btnStatistics_Click(object sender, RoutedEventArgs e)
        {
            StatisticsWin statistics = new StatisticsWin();
            statistics.Show();
        }
    }
}
