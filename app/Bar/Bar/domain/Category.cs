using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bar.domain
{
    /* 'Category' define una lista de productos de una misma categoría */
    public class Category
    {
        /* Atributos del objeto */
        // Nombre de la categoría
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        // Lista de productos de la categoría
        private List<string> products;
        public List<string> Products
        {
            get { return products; }
            set { products = value; }
        }

        /* Métodos constructores */
        public Category() { }

        public Category(string name)
        {
            Name = name;
            Products = new List<string>();
        }

        // Dos categorías son iguales si tienen el mismo nombre
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(Category)) return false;
            Category c = (Category)obj;
            return name.Equals(c.Name);
        }
    }
}
