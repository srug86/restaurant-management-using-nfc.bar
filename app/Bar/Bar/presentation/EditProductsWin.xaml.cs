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
using System.Windows.Threading;

namespace Bar.presentation
{
    /// <summary>
    /// Lógica de interacción para EditProductsWin.xaml
    /// </summary>
    public partial class EditProductsWin : Window
    {
        private JourneyManager manager = JourneyManager.Instance;

        public EditProductsWin()
        {
            manager.initProductsManager();
            manager.ProductsManager.setGuiReference(this);
            InitializeComponent();
        }

        private void btnLoadPL_Click(object sender, RoutedEventArgs e)
        {
            manager.ProductsManager.updateProducts(true);
            manager.ProductsManager.updateGuiCategories();
        }

        private void btnSavePL_Click(object sender, RoutedEventArgs e)
        {
            manager.ProductsManager.saveProducts();
        }

        private void btnAddCat_Click(object sender, RoutedEventArgs e)
        {
            EditCategory editC = new EditCategory("");
            editC.Show();
        }

        private void btnRemoveCat_Click(object sender, RoutedEventArgs e)
        {
            if (listVCategories.SelectedIndex != -1)
                manager.ProductsManager.removeCategory(((Category)((ListViewItem)listVCategories.SelectedItem).Content).Name);
        }

        private void btnEditCat_Click(object sender, RoutedEventArgs e)
        {
            if (listVCategories.SelectedIndex != -1)
            {
                EditCategory editC = new EditCategory(((Category)((ListViewItem)listVCategories.SelectedItem).Content).Name);
                editC.Show();
            }
        }

        private void btnAddPro_Click(object sender, RoutedEventArgs e)
        {
            EditProductWin editP = new EditProductWin(new Product());
            editP.Show();
        }

        private void btnRemovePro_Click(object sender, RoutedEventArgs e)
        {
            if (listVProducts.SelectedIndex != -1)
                manager.ProductsManager.removeProduct((((ProductItem)((ListViewItem)listVProducts.SelectedItem).Content).PName),
                (((Category)((ListViewItem)listVCategories.SelectedItem).Content).Name));
        }

        private void btnEditPro_Click(object sender, RoutedEventArgs e)
        {
            if (listVProducts.SelectedIndex != -1)
            {
                EditProductWin editP = new EditProductWin(manager.ProductsManager.getProduct(((ProductItem)((ListViewItem)listVProducts.SelectedItem).Content).PName));
                editP.Show();
            }
        }

        private delegate void ProductList(List<Product> products);
        public void delegateToChangeProductList(List<Product> products)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new ProductList(this.changeProductList), products);
        }

        private void changeProductList(List<Product> products)
        {
            listVProducts.Items.Clear();
            foreach (Product p in products)
            {
                ProductItem pi = new ProductItem();
                pi.PName = p.Name;
                pi.Price = p.Price;
                pi.Discount = p.Discount * 100;
                pi.DiscountedUnit = p.DiscountedUnit;
                pi.Visible = p.Visible ? "Si" : "No";
                listVProducts.Items.Add(new ListViewItem());
                ((ListViewItem)listVProducts.Items[listVProducts.Items.Count - 1]).Content = pi;
            }
        }

        private delegate void CategoriesList(List<Category> categories);
        public void delegateToChangeCategoriesList(List<Category> categories)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new CategoriesList(this.changeCategoriesList), categories);
        }

        private void changeCategoriesList(List<Category> categories)
        {
            listVCategories.Items.Clear();
            foreach (Category c in categories)
            {
                listVCategories.Items.Add(new ListViewItem());
                ((ListViewItem)listVCategories.Items[listVCategories.Items.Count - 1]).Content = c;
            }
        }

        private void listVCategories_MouseUp(object sender, MouseButtonEventArgs e)
        {
            manager.ProductsManager.updateGuiProducts(((Category)((ListViewItem)listVCategories.SelectedItem).Content).Name);
        }
    }

    public class ProductItem : ListViewItem
    {
        private string name, visible;

        public string PName
        {
            get { return name; }
            set { name = value; }
        }

        public string Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        private int discountedUnit;

        public int DiscountedUnit
        {
            get { return discountedUnit; }
            set { discountedUnit = value; }
        }

        private double price, discount;

        public double Price
        {
            get { return price; }
            set { price = value; }
        }

        public double Discount
        {
            get { return discount; }
            set { discount = value; }
        }

        public ProductItem() { }
    }
}
