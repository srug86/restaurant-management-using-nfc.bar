using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bar.domain
{
    public class Product
    {
        private string name, category, description;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Category
        {
            get { return category; }
            set { category = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
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
        
        private bool visible;

        public bool Visible
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

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(Product)) return false;
            Product p = (Product)obj;
            return name.Equals(p.Name);
        }
    }
}
