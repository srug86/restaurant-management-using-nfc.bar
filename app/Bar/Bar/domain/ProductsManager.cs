using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bar.communication;
using System.Xml;
using Bar.presentation;

namespace Bar.domain
{
    class ProductsManager
    {
        private AdapterWebServices adapter = AdapterWebServices.Instance;

        private EditProductsWin gui;

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

        public ProductsManager()
        {
            Products = new List<Product>();
            Categories = new List<Category>();
        }

        public void setGuiReference(EditProductsWin gui)
        {
            this.gui = gui;
        }

        public void updateProducts(bool notVisibles)
        {
            Products = xmlProductsDecoder(adapter.sendMeProducts(notVisibles));
            Categories = getCategories();
        }

        public Product getProduct(string name)
        {
            foreach (Product p in Products)
                if (p.Name.Equals(name))
                    return p;
            return new Product();
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

        public void saveProducts()
        {
            adapter.sendProductList(xmlProductsBuilder(Products));
        }

        public void updateGuiCategories()
        {
            gui.delegateToChangeCategoriesList(Categories);
        }

        public void updateGuiProducts(string category)
        {
            List<Product> ps = new List<Product>();
            foreach (Product p in Products)
                if (p.Category.Equals(category))
                    ps.Add(p);
            gui.delegateToChangeProductList(ps);
        }

        public void addProduct(Product p)
        {
            if (Products.IndexOf(p) == -1)
            {
                Products.Add(p);
                Categories[Categories.IndexOf(new Category(p.Category))].Products.Add(p.Name);
                updateGuiProducts(p.Category);
            }
        }

        public void saveProduct(string oldName, Product p)
        {
            int i = Products.IndexOf(new Product(oldName));
            if (i != -1)
            {
                Products[i] = p;
                List<string> ps = Categories[Categories.IndexOf(new Category(p.Category))].Products;
                for (int j = 0; j < ps.Count; j++)
                    if (ps[j].Equals(oldName))
                        ps[j] = p.Name;
                updateGuiProducts(p.Category);
            }
        }

        public void removeProduct(string name, string category)
        {
            int i = Products.IndexOf(new Product(name));
            if (i != -1)
            {
                Products.RemoveAt(i);
                List<string> ps = Categories[Categories.IndexOf(new Category(category))].Products;
                for (int j = 0; j < ps.Count; j++)
                    if (ps[j].Equals(name))
                        ps.RemoveAt(j);
                updateGuiProducts(category);
            }
        }

        public void addCategory(string name)
        {
            if (Categories.IndexOf(new Category(name)) == -1)
            {
                Categories.Add(new Category(name));
                updateGuiCategories();
            }
        }

        public void saveCategory(string oldName, string newName)
        {
            int i = Categories.IndexOf(new Category(oldName));
            if (i != -1 && Categories.IndexOf(new Category(newName)) == -1)
            {
                Categories[i].Name = newName;
                for (int j = 0; j < Products.Count; j++)
                    if (Products[j].Category.Equals(oldName))
                        Products[j].Category = newName;
                updateGuiCategories();
            }
        }

        public void removeCategory(string name)
        {
            int i = Categories.IndexOf(new Category(name));
            if (i != -1)
            {
                Categories.RemoveAt(i);
                for (int j = 0; j < Products.Count; )
                {
                    if (Products[j].Category.Equals(name))
                        Products.RemoveAt(j);
                    else j++;
                }
                updateGuiCategories();
                updateGuiProducts("");
            }
        }

        private List<Product> xmlProductsDecoder(string sXml)
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

        public static string xmlProductsBuilder(List<Product> products)
        {
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<Products>\n";
            if (products.Count > 0)
                foreach (Product product in products)
                {
                    xml += "\t<Product name=\"" + product.Name + "\">\n";
                    xml += "\t\t<Category>" + product.Category + "</Category>\n";
                    xml += "\t\t<Price>" + product.Price + "</Price>\n";
                    xml += "\t\t<Description>" + product.Description + "</Description>\n";
                    xml += "\t\t<Visible>" + product.Visible + "</Visible>\n";
                    xml += "\t\t<Discount>" + product.Discount + "</Discount>\n";
                    xml += "\t\t<DiscountedUnit>" + product.DiscountedUnit + "</DiscountedUnit>\n";
                    xml += "\t</Product>\n";
                }
            xml += "</Products>";
            return xml;
        }
    }
}
