using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bar.communication;
using System.Xml;

namespace Bar.domain
{
    class ProductsManager
    {
        private AdapterWebServices adapter = AdapterWebServices.Instance;

        private List<Product> products;

        internal List<Product> Products
        {
            get { return products; }
            set { products = value; }
        }

        private List<Category> categories;

        internal List<Category> Categories
        {
            get { return categories; }
            set { categories = value; }
        }

        public ProductsManager() { }

        public void updateProducts()
        {
            Products = xmlListOfProducts(adapter.sendMeProducts());
            Categories = getCategories();
        }

        private List<Product> xmlListOfProducts(string sXml)
        {
            List<Product> lop = new List<Product>();
            if (sXml != "")
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(sXml);
                XmlNodeList pducts = xml.GetElementsByTagName("Products");
                XmlNodeList pList = ((XmlElement)pducts[0]).GetElementsByTagName("Product");
                foreach (XmlElement product in pList)
                {
                    Product p = new Product();
                    p.Name = Convert.ToString(product.GetAttribute("name")).Trim();
                    XmlNodeList price = ((XmlElement)product).GetElementsByTagName("Price");
                    p.Price = Convert.ToDouble(price[0].InnerText);
                    XmlNodeList category = ((XmlElement)product).GetElementsByTagName("Category");
                    p.Category = Convert.ToString(category[0].InnerText).Trim();
                    XmlNodeList description = ((XmlElement)product).GetElementsByTagName("Description");
                    p.Description = Convert.ToString(description[0].InnerText);
                    XmlNodeList visible = ((XmlElement)product).GetElementsByTagName("Visible");
                    p.Visible = Convert.ToBoolean(visible[0].InnerText);
                    XmlNodeList discount = ((XmlElement)product).GetElementsByTagName("Discount");
                    p.Discount = Convert.ToDouble(discount[0].InnerText);
                    XmlNodeList discountedUnit = ((XmlElement)product).GetElementsByTagName("DiscountedUnit");
                    p.DiscountedUnit = Convert.ToInt32(discountedUnit[0].InnerText);
                    lop.Add(p);
                }
            }
            return lop;
        }

        public List<Category> getCategories()
        {
            List<Category> categories = new List<Category>();
            foreach (Product product in Products)
            {
                if (product.Visible)
                {
                    if (categories.IndexOf(new Category(product.Category)) != -1)
                        categories[categories.IndexOf(new Category(product.Category))].Products.Add(product.Name);
                    else
                    {
                        Category c = new Category(product.Category);
                        c.Products.Add(product.Name);
                        categories.Add(c);
                    }
                }
            }
            return categories;
        }
    }
}
