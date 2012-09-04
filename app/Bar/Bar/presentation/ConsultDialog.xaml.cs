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

namespace Bar.presentation
{
    /// <summary>
    /// Lógica de interacción para ConsultDialog.xaml
    /// </summary>
    public partial class ConsultDialog : Window
    {
        private StatisticsWin super;

        private int mode;

        // Método constructor
        public ConsultDialog(StatisticsWin super, int mode)
        {
            this.super = super;
            this.mode = mode;
            InitializeComponent();
            initializeData();
        }

        // Inicialización del aspecto de la ventana según el modo
        private void initializeData()
        {
            switch (mode)
            {
                case 1: // Modo "Cargar histórico de facturas"
                    wConsult.Title = "MobiCarta - Cargar historial de facturas";
                    wConsult.Background = Brushes.DarkGreen;
                    break;
                case 2: // Modo "Cargar histórico de pedidos"
                    wConsult.Title = "MobiCarta - Cargar historial de pedidos";
                    wConsult.Background = Brushes.Goldenrod;
                    break;
                default: break;
            }
        }

        /* Lógica de control de eventos */
        // Click para cancelar la búsqueda del histórico en la BD
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        // Click para confirmar la búsqueda del histórico en la BD
        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt16(txtbSize.Text) > 0 && cbbAsc.SelectedIndex != -1)
            {
                bool asc = (cbbAsc.SelectedIndex == 0);
                int amount = Convert.ToInt16(txtbSize.Text);
                switch (mode)   // Modo de búsqueda: (1) Facturas, (2) Pedidos.
                {
                    case 1: super.loadBillsList(amount, asc); break;
                    case 2: super.loadHOrdersList(amount,asc); break;
                    default: break;
                }
                this.Visibility = Visibility.Hidden;
            }
        }

        // Click para decrementar el número de elementos a devolver en la búsqueda
        private void btnDec_Click(object sender, RoutedEventArgs e)
        {
            int value = Convert.ToInt16(txtbSize.Text);
            if (value > 1)
                txtbSize.Text = (--value).ToString();
        }

        // Click para incrementar el número de elementos a devolver en la búsqueda
        private void btnInc_Click(object sender, RoutedEventArgs e)
        {
            int value = Convert.ToInt16(txtbSize.Text);
            txtbSize.Text = (++value).ToString();
        }
    }
}
