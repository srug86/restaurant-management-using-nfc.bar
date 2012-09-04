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

        /* Atributos de la clase */
        private JourneyManagerWin super;

        private bool viewMode;

        private Bill bill;
        public Bill Bill
        {
            get { return bill; }
            set { bill = value; }
        }

        /* Métodos constructores */
        public BillDialog(Bill bill)
        {
            Bill = bill;
            InitializeComponent();
            showViewOptions();
            viewMode = true;
            initializeData();
        }

        public BillDialog(JourneyManagerWin super, Bill bill)
        {
            this.super = super;
            Bill = bill;
            InitializeComponent();
            viewMode = false;
            initializeData();
        }

        // Inicializa el contenido de la ventana con los datos de la factura
        private void initializeData()
        {
            if (!viewMode)
                if (super.btnCheckIn.Content.Equals("Ver factura")) charged();
            txtbBill.Text = Convert.ToString(Bill.Id);
            txtbSerial.Text = Convert.ToString(Bill.Serial);
            txtbTable.Text = Convert.ToString(Bill.TableID);
            txtbDate.Text = Convert.ToString(Bill.Date);
            txtbDNI.Text = Bill.ClientInfo.Dni;
            txtbName.Text = Bill.ClientInfo.Name + " " + Bill.ClientInfo.Surname;
            if (!Bill.ClientAddress.Street.Equals(""))
                txtbAddress.Text = Bill.ClientAddress.Street + ", " + Bill.ClientAddress.Number;
            txtbTown.Text = Bill.ClientAddress.Town;
            txtbState.Text = Bill.ClientAddress.State;
            txtbSubtotal.Text = Convert.ToString(Bill.Subtotal);
            txtbDiscount.Text = Convert.ToString(Bill.Discount);
            txtbTaxBase.Text = Convert.ToString(Bill.TaxBase);
            txtbIVA.Text = Convert.ToString(Bill.Iva);
            txtbQuote.Text = Convert.ToString(Bill.Quote);
            txtbTotal.Text = Convert.ToString(Bill.Total);
            foreach (OrderPrice oPrice in Bill.Orders)
                listVBill.Items.Add(oPrice);
            if (Bill.Paid > 0) charged();
        }

        // Cambios en la GUI después de cobrar la factura
        private void charged()
        {
            btnCharge.Content = "COBRADA";
            btnCharge.IsEnabled = false;
            btnCancel.Content = "Salir";
        }

        // Cambia aspecto de los botones para el modo "Ver factura"
        private void showViewOptions()
        {
            btnCharge.Content = "Salir";
            btnCancel.Visibility = Visibility.Hidden;
        }

        /* Lógica de control de eventos */
        // Click en el botón "Cobrar factura"
        private void btnCharge_Click(object sender, RoutedEventArgs e)
        {
            if (!viewMode)
            {
                manager.BillsManager.payBill(bill.Id, bill.TableID, 1);
                super.delegateToShowBarEvent(5, new Order(), bill.TableID);
                charged();
            }
            else this.Visibility = Visibility.Hidden;
        }

        // Click para cerrar la vista de la ventana
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        // Click en el botón "Imprimir factura"
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            /* Funcionalidad no implementada */
        }
    }
}
