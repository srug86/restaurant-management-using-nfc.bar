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
    /// Lógica de interacción para EditOrderDialog.xaml
    /// </summary>
    public partial class EditOrderDialog : Window
    {
        private JourneyManager manager = JourneyManager.Instance;

        private Order order;

        public EditOrderDialog(Order order)
        {
            this.order = order;
            InitializeComponent();
            initializeData();
        }

        private void initializeData()
        {
            txtbId.Text = order.Id.ToString();
            txtbProduct.Text = order.Product.Trim();
            txtbAmount.Text = order.Amount.ToString();
            txtbDate.Text = order.Date.ToString();
            cbbState.SelectedIndex = order.Status + 1;
            List<int> tables = manager.RoomManager.getCandidateTables();
            for (int i = 0; i < tables.Count; i++)
            {
                cbbTable.Items.Add(tables[i]);
                if (order.TableID == tables[i])
                    cbbTable.SelectedIndex = i;
            }
        }

        private void btnPlus_Click(object sender, RoutedEventArgs e)
        {
            if (txtbAmount.Text != "")
                txtbAmount.Text = Convert.ToString(Convert.ToInt16(txtbAmount.Text) + 1);
        }

        private void btnMinus_Click(object sender, RoutedEventArgs e)
        {
            if (txtbAmount.Text != "")
                if (Convert.ToInt16(txtbAmount.Text) > 1)
                    txtbAmount.Text = Convert.ToString(Convert.ToInt16(txtbAmount.Text) - 1);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            if (cbbTable.SelectedIndex != -1 && 
                Convert.ToInt16(cbbTable.SelectedItem.ToString()) != order.TableID)
                manager.OrdersManager.changeOrderTable(order.Id, Convert.ToInt16(cbbTable.SelectedItem.ToString()));
            if (Convert.ToInt16(txtbAmount.Text) != order.Amount)
                manager.OrdersManager.changeOrderAmount(order.Id, Convert.ToInt16(txtbAmount.Text));
            if (cbbState.SelectedIndex != -1 &&
                (Convert.ToInt16(cbbState.SelectedIndex) - 1) != order.Status)
                manager.OrdersManager.changeOrderStatus(order.Id, Convert.ToInt16(cbbState.SelectedIndex) - 1);
            this.Visibility = Visibility.Hidden;
        }
    }
}
