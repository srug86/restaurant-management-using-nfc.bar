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
    /// Lógica de interacción para StatisticsWin.xaml
    /// </summary>
    public partial class StatisticsWin : Window
    {
        private BillsManager billsManager = BillsManager.Instance;

        private OrdersManager ordersManager = OrdersManager.Instance;

        // Método constructor
        public StatisticsWin()
        {
            InitializeComponent();
        }

        // Carga la lista del histórico de facturas
        public void loadBillsList(int amount, bool ascending)
        {
            List<ShortBill> bills = billsManager.getBills(amount, ascending);
            generateBillsList(bills);
            btnOBills.IsEnabled = IsEnabled;
            gridOBillsList.Visibility = Visibility.Visible;
            gridOptions.Visibility = Visibility.Visible;
        }

        // Carga la lista del histórico de pedidos
        public void loadHOrdersList(int amount, bool ascending)
        {
            List<HOrder> orders = ordersManager.getHistoricalOrders(amount, ascending);
            generateHOrdersList(orders);
            btnOOrders.IsEnabled = IsEnabled;
            gridOOrdersList.Visibility = Visibility.Visible;
            gridOptions.Visibility = Visibility.Visible;
        }

        // Genera la lista del histórico de facturas
        private void generateBillsList(List<ShortBill> bills)
        {
            listVBills.Items.Clear();
            foreach (ShortBill b in bills)
            {
                BillItem bi = new BillItem();
                bi.Id = b.Id;
                bi.TableID = b.TableID;
                bi.ClientID = b.Client;
                bi.Date = b.Date.ToString();
                bi.Total = b.Total;
                switch (b.Paid) // ¿Pagado? ¿Por qué método?
                {
                    case 0: bi.Paid = "No"; break;
                    case 1: bi.Paid = "Si"; break;
                    case 2: bi.Paid = "Si (NFC)"; break;
                    default: break;
                }
                listVBills.Items.Add(new ListViewItem());
                ((ListViewItem)listVBills.Items[listVBills.Items.Count - 1]).Content = bi;
            }
        }

        // Genera la lista del histórico de pedidos
        private void generateHOrdersList(List<HOrder> orders)
        {
            listVOrders.Items.Clear();
            for (int i = 0; i < orders.Count; i++)
            {
                HOrderItem hoi = new HOrderItem();
                hoi.Id = i + 1;
                hoi.Date = orders[i].Date.ToString();
                hoi.Product = orders[i].Product;
                hoi.Amount = orders[i].Amount;
                hoi.Client = orders[i].Client;
                listVOrders.Items.Add(new ListViewItem());
                ((ListViewItem)listVOrders.Items[listVOrders.Items.Count - 1]).Content = hoi;
            }
        }

        /* Lógica de control de eventos */
        // Click en el botón "Cargar histórico de facturas"
        private void btnLoadBills_Click(object sender, RoutedEventArgs e)
        {
            ConsultDialog cd = new ConsultDialog(this, 1);
            cd.Show();
        }

        // Click en el botón "Cargar histórico de pedidos"
        private void btnLoadOrders_Click(object sender, RoutedEventArgs e)
        {
            ConsultDialog cd = new ConsultDialog(this, 2);
            cd.Show();
        }

        // Click en el botón "Consultar" para ver los detalles de la factura seleccionada
        private void btnConsult_Click(object sender, RoutedEventArgs e)
        {
            if (listVBills.SelectedIndex != -1)
            {
                BillDialog billDialog = new BillDialog(billsManager.getBill(((BillItem)((ListViewItem)listVBills.SelectedItem).Content).Id));
                billDialog.Show();
            }
        }

        // Click para abrir el modo "Ver facturas"
        private void btnOBills_Click(object sender, RoutedEventArgs e)
        {
            gridOOrdersList.Visibility = Visibility.Hidden;
            gridOBillsList.Visibility = Visibility.Visible;
        }

        // Click para abrir el modo "Ver pedidos"
        private void btnOOrders_Click(object sender, RoutedEventArgs e)
        {
            gridOBillsList.Visibility = Visibility.Hidden;
            gridOOrdersList.Visibility = Visibility.Visible;
        }
    }

    /* Clase auxiliar para representar la información de una factura en una lista */
    public class BillItem : ListViewItem
    {
        private int id, tableID;
        // Identificador
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        // Mesa
        public int TableID
        {
            get { return tableID; }
            set { tableID = value; }
        }

        private string clientID, date, paid;
        // DNI del cliente
        public string ClientID
        {
            get { return clientID; }
            set { clientID = value; }
        }
        // Fecha de facturación
        public string Date
        {
            get { return date; }
            set { date = value; }
        }
        // Estado del cobro: 'No', 'Si', "Si (NFC)"
        public string Paid
        {
            get { return paid; }
            set { paid = value; }
        }

        private double total;
        // Importe total
        public double Total
        {
            get { return total; }
            set { total = value; }
        }

        // Método constructor
        public BillItem() { }
    }

    /* Clase auxiliar para representar la información de un pedido en una lista */
    public class HOrderItem : ListViewItem
    {
        string date, product, client;
        // Fecha de la solicitud
        public string Date
        {
            get { return date; }
            set { date = value; }
        }
        // Producto solicitado
        public string Product
        {
            get { return product; }
            set { product = value; }
        }
        // Cliente que lo solicitó
        public string Client
        {
            get { return client; }
            set { client = value; }
        }

        int id, amount;
        // Identificador del pedido en la lista
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        // Cantidad de productos del mismo tipo
        public int Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        // Método constructor
        public HOrderItem() { }
    }
}
