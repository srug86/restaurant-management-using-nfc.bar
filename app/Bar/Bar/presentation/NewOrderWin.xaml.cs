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
    /// Lógica de interacción para NewOrderWin.xaml
    /// </summary>
    public partial class NewOrderWin : Window
    {
        private JourneyManager manager = JourneyManager.Instance;

        private List<Category> categories;

        private List<int> tables;

        // Método constructor
        public NewOrderWin(List<Category> categories, List<int> tables)
        {
            this.categories = categories;
            this.tables = tables;
            InitializeComponent();
            generateCategories();
            initializeTables();
        }

        // Genera la lista de categorías
        private void generateCategories()
        {
            if (categories.Count > 0)
            {
                int rows = categories.Count / 4;
                if (categories.Count % 4 > 0) rows++;
                uGridCategories.Rows = rows;
                uGridCategories.Columns = 4;
                foreach (Category c in categories)
                {
                    Button button = new Button();
                    button.Name = "btn" + c.Name.Replace(' ', '_');
                    button.Content = c.Name;
                    button.Click += new RoutedEventHandler(this.category_Click);
                    uGridCategories.Children.Add(button);
                }
            }
        }

        // Inicializa las mesas candidatas para el pedido
        private void initializeTables()
        {
            foreach (int index in tables)
                cbbOrdersTable.Items.Add("Mesa " + index);
        }

        // Selecciona la mesa candidata para el pedido
        public void selectTable(int table)
        {
            cbbOrdersTable.SelectedIndex = table;
        }

        /* Lógica de control de eventos */
        // Muestra la lista de productos de la categoría seleccionada
        private void category_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Category c = categories[categories.IndexOf(new Category(btn.Name.Substring(3).Replace('_', ' ')))];
            listVProducts.Items.Clear();
            foreach (string p in c.Products)
                listVProducts.Items.Add(p);
        }

        // Click en el teclado numérico para seleccionar el número de pedidos
        private void btnNumber_Click(object sender, RoutedEventArgs e)
        {
            txtbOperation.Text += ((Button)sender).Name.Substring(3);
        }

        // Click para incrementar el número de unidades de un tipo de producto
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (listVOrders.SelectedIndex != -1)
            {
                ((NewOrderItem)listVOrders.SelectedItem).Amount++;
                listVOrders.Items.Refresh();
            }
        }

        // Click para decrementar el número de unidades de un tipo de producto
        private void btnSubtract_Click(object sender, RoutedEventArgs e)
        {
            if (listVOrders.SelectedIndex != -1)
            {
                ((NewOrderItem)listVOrders.SelectedItem).Amount--;
                // Si se llega a 0, se elimina el producto de la lista
                if (((NewOrderItem)listVOrders.SelectedItem).Amount <= 0)
                {
                    listVOrders.Items.Remove(listVOrders.SelectedItem);
                    if (listVOrders.Items.Count == 0)
                        btnAccept.IsEnabled = false;
                }
                else
                    listVOrders.Items.Refresh();
            }
        }

        // Click para eliminar de la lista un tipo de producto
        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (listVOrders.SelectedIndex != -1)
                listVOrders.Items.Remove(listVOrders.SelectedItem);
            if (listVOrders.Items.Count == 0)
                btnAccept.IsEnabled = false;
        }

        // Click para insertar un punto en la cantidad de productos
        private void btnPoint_Click(object sender, RoutedEventArgs e)
        {
            /* Funcionalidad no contemplada
            if (txtbOperation.Text != "" && !txtbOperation.Text.Contains('.'))
                txtbOperation.Text += '.'; */
        }

        // Click para borrar el contenido del selector del número de productos
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtbOperation.Text = "";
        }

        // Click en el botón "Intro" para añadir un producto a la lista del pedido
        private void btnIntro_Click(object sender, RoutedEventArgs e)
        {
            if (listVProducts.SelectedIndex != -1)
            {
                NewOrderItem order = new NewOrderItem();
                order.Id = listVOrders.Items.Count + 1;
                order.Product = listVProducts.SelectedItem.ToString();
                if (txtbOperation.Text != "")
                {
                    order.Amount = Convert.ToInt16(txtbOperation.Text);
                    txtbOperation.Text = "";
                }
                else order.Amount = 1;
                if (listVOrders.Items.Contains(order))
                {
                    ((NewOrderItem)listVOrders.Items[listVOrders.Items.IndexOf(order)]).Amount =
                        ((NewOrderItem)listVOrders.Items[listVOrders.Items.IndexOf(order)]).Amount +
                        order.Amount;
                    listVOrders.Items.Refresh();
                }
                else listVOrders.Items.Add(order);
            }
            if (listVOrders.Items.Count > 0)
                btnAccept.IsEnabled = true;
        }

        // Click para "Cancelar" el pedido
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        // Click en el botón "Aceptar" para confirmar el pedido
        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            if (cbbOrdersTable.SelectedIndex != -1 && listVOrders.Items.Count > 0)
            {
                List<Order> orders = new List<Order>();
                foreach (NewOrderItem orderItem in listVOrders.Items)
                {
                    Order order = new Order();
                    order.TableID = Convert.ToInt16(cbbOrdersTable.SelectedItem.ToString().Substring(5));
                    order.Product = orderItem.Product;
                    order.Amount = orderItem.Amount;
                    orders.Add(order);
                }
                manager.OrdersManager.addOrders(orders);    // Añade la orden a la lista de pedidos
                this.Visibility = Visibility.Hidden;
            }
        }
    }

    /* Clase auxiliar para representar la información de un ítem de la lista del pedido */
    public class NewOrderItem
    {
        private int id, amount;
        // Identificador de producto
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

        private string product;
        // Nombre del producto
        public string Product
        {
            get { return product; }
            set { product = value; }
        }

        // Método constructor
        public NewOrderItem() { }

        // Dos productos son iguales si tienen el mismo nombre
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(NewOrderItem)) return false;
            NewOrderItem o = (NewOrderItem)obj;
            return product.Equals(o.Product);
        }
    }
}
