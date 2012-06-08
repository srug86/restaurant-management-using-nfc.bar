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
    /// Lógica de interacción para BillDialog.xaml
    /// </summary>
    public partial class BillDialog : Window
    {
        private JourneyManager manager = JourneyManager.Instance;

        private Bill bill;

        public Bill Bill
        {
            get { return bill; }
            set { bill = value; }
        }

        public BillDialog(Bill bill)
        {
            Bill = bill;
            InitializeComponent();
            initializeData();
        }

        private void btnCharge_Click(object sender, RoutedEventArgs e)
        {
            manager.BillsManager.payBill(Convert.ToInt32(txtbBill.Text), 1);
            charged();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void initializeData()
        {
            txtbBill.Text = Convert.ToString(bill.Id);
            txtbSerial.Text = Convert.ToString(bill.Serial);
            txtbTable.Text = Convert.ToString(bill.TableID);
            txtbDate.Text = Convert.ToString(bill.Date);
            txtbDNI.Text = bill.ClientInfo.Dni;
            txtbName.Text = bill.ClientInfo.Name + " " + bill.ClientInfo.Surname;
            txtbAddress.Text = bill.ClientAddress.Street + ", " + bill.ClientAddress.Number;
            txtbTown.Text = bill.ClientAddress.Town;
            txtbState.Text = bill.ClientAddress.State;
            txtbSubtotal.Text = Convert.ToString(bill.Subtotal);
            txtbDiscount.Text = Convert.ToString(bill.Discount);
            txtbTaxBase.Text = Convert.ToString(bill.TaxBase);
            txtbIVA.Text = Convert.ToString(bill.Iva);
            txtbQuote.Text = Convert.ToString(bill.Quote);
            txtbTotal.Text = Convert.ToString(bill.Total);
            foreach (OrderPrice oPrice in bill.Orders)
                listVBill.Items.Add(oPrice);
            if (bill.Paid > 0) charged();
        }

        private void charged()
        {
            btnCharge.Content = "COBRADA";
            btnCharge.IsEnabled = false;
            btnCancel.Content = "Salir";
        }
    }
}
