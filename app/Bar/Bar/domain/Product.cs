using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bar.domain
{
    // 'Product' almacena la información de un producto
    public class Product
    {
        /* Atributos del objeto */
        private string name, category, description;
        // Nombre
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        // Categoría
        public string Category
        {
            get { return category; }
            set { category = value; }
        }
        // Descripción (palabras clave)
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private double price, discount;
        // Precio
        public double Price
        {
            get { return price; }
            set { price = value; }
        }
        // Descuento disponible para este producto
        public double Discount
        {
            get { return discount; }
            set { discount = value; }
        }
        
        // ¿Está disponible el producto?
        private bool visible;
        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        // Número de productos de este tipo para obtener un descuento
        private int discountedUnit;
        public int DiscountedUnit
        {
            get { return discountedUnit; }
            set { discountedUnit = value; }
        }

        /* Métodos constructores */
        public Product()
        {
            Name = Category = Description = "";
            Price = Discount = 0.0;
            DiscountedUnit = 0;
            Visible = false;
        }

        public Product(string name)
        {
            Name = name;
            Category = Description = "";
            Price = Discount = 0.0;
            DiscountedUnit = 0;
            Visible = false;
        }

        // Dos productos son similares si tienen el mismo nombre
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(Product)) return false;
            Product p = (Product)obj;
            return name.Equals(p.Name);
        }
    }
}
