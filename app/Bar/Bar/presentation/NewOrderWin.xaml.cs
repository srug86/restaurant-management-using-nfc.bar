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

        public NewOrderWin(List<Category> categories, List<int> tables)
        {
            this.categories = categories;
            this.tables = tables;
            InitializeComponent();
            generateCategories();
            initializeTables();
        }

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

        private void initializeTables()
        {
            foreach (int index in tables)
                cbbOrdersTable.Items.Add("Mesa " + index);
        }

        public void selectTable(int table)
        {
            cbbOrdersTable.SelectedIndex = table;
        }

        private void category_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Category c = categories[categories.IndexOf(new Category(btn.Name.Substring(3).Replace('_', ' ')))];
            listVProducts.Items.Clear();
            foreach (string p in c.Products)
                listVProducts.Items.Add(p);
        }

        private void btnNumber_Click(object sender, RoutedEventArgs e)
        {
            txtbOperation.Text += ((Button)sender).Name.Substring(3);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (listVOrders.SelectedIndex != -1)
            {
                ((NewOrderItem)listVOrders.SelectedItem).Amount++;
                listVOrders.Items.Refresh();
            }
        }

        private void btnSubtract_Click(object sender, RoutedEventArgs e)
        {
            if (listVOrders.SelectedIndex != -1)
            {
                ((NewOrderItem)listVOrders.SelectedItem).Amount--;
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

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (listVOrders.SelectedIndex != -1)
                listVOrders.Items.Remove(listVOrders.SelectedItem);
            if (listVOrders.Items.Count == 0)
                btnAccept.IsEnabled = false;
        }

        private void btnPoint_Click(object sender, RoutedEventArgs e)
        {
            if (txtbOperation.Text != "" && !txtbOperation.Text.Contains('.'))
                txtbOperation.Text += '.';
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtbOperation.Text = "";
        }

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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

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
                manager.OrdersManager.addOrders(orders);
                this.Visibility = Visibility.Hidden;
            }
        }
    }

    public class NewOrderItem
    {
        private int id, amount;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public int Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        private string product;

        public string Product
        {
            get { return product; }
            set { product = value; }
        }

        public NewOrderItem() { }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(NewOrderItem)) return false;
            NewOrderItem o = (NewOrderItem)obj;
            return product.Equals(o.Product);
        }
    }
}
