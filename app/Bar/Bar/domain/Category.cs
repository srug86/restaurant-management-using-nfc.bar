using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bar.domain
{
    public class Category
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private List<string> products;

        public List<string> Products
        {
            get { return products; }
            set { products = value; }
        }

        public Category() { }

        public Category(string name)
        {
            Name = name;
            Products = new List<string>();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(Category)) return false;
            Category c = (Category)obj;
            return name.Equals(c.Name);
        }
    }
}
