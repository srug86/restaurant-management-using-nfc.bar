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
    /// Lógica de interacción para EditCategory.xaml
    /// </summary>
    public partial class EditCategory : Window
    {
        private JourneyManager manager = JourneyManager.Instance;

        private string oldCategory;

        // Método constructor
        public EditCategory(string category)
        {
            this.oldCategory = category;
            InitializeComponent();
            initializeData(category);
        }

        // Inicializa el contenido de la ventana
        private void initializeData(string category)
        {
            txtbName.Text = category;
            if (category.Equals(""))
            {
                this.Title = "MobiCarta - Crear categoría";
                lblInstructions.Content = "Establezca el nombre de la nueva categoría:";
                btnAccept.Content = "Crear";
            }
        }

        /* Lógica de control de eventos */
        // Click para cancelar la edición de la categoría
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        // Click para aceptar la edición de la categoría
        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            if (!txtbName.Text.Equals(""))
            {
                if (oldCategory.Equals(""))
                    manager.ProductsManager.addCategory(txtbName.Text);
                else manager.ProductsManager.saveCategory(oldCategory, txtbName.Text);
                this.Visibility = Visibility.Hidden;
            }
        }
    }
}
