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
    /// Lógica de interacción para EditProductWin.xaml
    /// </summary>
    public partial class EditProductWin : Window
    {
        private JourneyManager manager = JourneyManager.Instance;

        private string oldName;

        public EditProductWin(Product product)
        {
            this.oldName = product.Name;
            InitializeComponent();
            initializeData(product);
        }

        private void initializeData(Product product)
        {
            txtbName.Text = product.Name;
            txtbPrice.Text = Convert.ToString(product.Price);
            txtbDiscount.Text = Convert.ToString(product.Discount);
            txtbDiscountedUnit.Text = Convert.ToString(product.DiscountedUnit);
            checkBVisible.IsChecked = product.Visible ? true : false;
            txtbDescription.Text = product.Description;
            List<Category> categories = manager.ProductsManager.Categories;
            for (int i = 0; i < categories.Count; i++)
            {
                cbbCategories.Items.Add(categories[i].Name);
                if (product.Category.Equals(categories[i].Name))
                    cbbCategories.SelectedIndex = i;
            }
            if (oldName.Equals(""))
            {
                this.Title = "MobiCarta - Crear producto";
                lblInstructions.Content = "Establezca los valores del nuevo producto:";
                btnAccept.Content = "Crear";
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!txtbName.Text.Equals("") && Convert.ToDouble(txtbPrice.Text) >= 0.0 &&
                    Convert.ToDouble(txtbDiscount.Text) >= 0.0 &&
                    Convert.ToDouble(txtbDiscountedUnit.Text) >= 0 &&
                    cbbCategories.SelectedIndex != -1)
                {
                    Product p = new Product(txtbName.Text);
                    p.Category = Convert.ToString(cbbCategories.SelectedItem);
                    p.Description = txtbDescription.Text;
                    p.Price = Convert.ToDouble(txtbPrice.Text);
                    p.Discount = Convert.ToDouble(txtbDiscount.Text);
                    p.DiscountedUnit = Convert.ToInt32(txtbDiscountedUnit.Text);
                    p.Visible = checkBVisible.IsChecked.Value;
                    if (oldName.Equals(""))
                        manager.ProductsManager.addProduct(p);
                    else manager.ProductsManager.saveProduct(oldName, p);
                    this.Visibility = Visibility.Hidden;
                }
            }
            catch (FormatException ex) { }
        }
    }
}
