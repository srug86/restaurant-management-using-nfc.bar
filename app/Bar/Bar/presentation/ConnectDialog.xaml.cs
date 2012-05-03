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
using Bar.domain;

namespace Bar.presentation
{
    /// <summary>
    /// Lógica de interacción para ConnectDialog.xaml
    /// </summary>
    public partial class ConnectDialog : Window
    {
        private JourneyManager manager = JourneyManager.Instance;

        public ConnectDialog()
        {
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            btnAccept.IsEnabled = false;
            if (!tbURL.Text.Equals(""))
            {
                bool connection = manager.connectToWS(tbURL.Text);
                if (connection)
                {
                    lblResult.Content = "* Conexión realizada con éxito";
                    btnAccept.IsEnabled = true;
                }
                else lblResult.Content = "* Conexión fallida";
            }
            else
                lblResult.Content = "* El campo Dirección URL no puede ser vacío";
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void btnClose_Click(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }
    }
}
