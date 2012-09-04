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
        // Método constructor
        public InitWin()
        {
            InitializeComponent();
        }

        // Método que cierra los subprocesos de la aplicación tras el cierre de esta
        private void Close_Program(object sender, EventArgs e)
        {
            App.Current.Shutdown();
            foreach (Process p in Process.GetProcesses())
                if (p.ProcessName == "Bar.vshost") p.Kill();
        }

        /* Lógica de control de eventos */
        // Click en el botón "Iniciar jornada"
        private void btnBegin_Click(object sender, RoutedEventArgs e)
        {
            JourneyManagerWin manager = new JourneyManagerWin();
            manager.Show();
        }

        // Click en el botón "Editar productos"
        private void btnEditProducts_Click(object sender, RoutedEventArgs e)
        {
            EditProductsWin editor = new EditProductsWin();
            editor.Show();
        }

        // Click en el botón "Histórico de pedidos y facturas"
        private void btnStatistics_Click(object sender, RoutedEventArgs e)
        {
            StatisticsWin statistics = new StatisticsWin();
            statistics.Show();
        }
    }
}
