using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bar.communication;
using System.Xml;
using System.Windows.Threading;
using Bar.presentation;

namespace Bar.domain
{
    class OrdersManager
    {
        private JourneyManager manager = JourneyManager.Instance;

        private AdapterWebServices adapter = AdapterWebServices.Instance;

        private JourneyManagerWin gui;

        private List<ObserverLO> loObservers = new List<ObserverLO>();

        private List<Order> orders = new List<Order>();

        internal List<Order> Orders
        {
            get { return orders; }
            set { orders = value; }
        }

        private List<Bill> bills;

        internal List<Bill> Bills
        {
            get { return bills; }
            set { bills = value; }
        }

        private int lastOrderID;

        public OrdersManager() { }

        public void setGuiReference(JourneyManagerWin gui)
        {
            this.gui = gui;
        }

        public void updateOrders()
        {
            Orders = xmlListOfOrders(adapter.sendMeOrdersStatus());
            gui.delegateToChangeTheOrdersList(Orders);
        }

        private List<Order> xmlListOfOrders(string sXml)
        {
            List<Order> loo = new List<Order>();
            if (sXml != "")
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(sXml);
                XmlNodeList oders = xml.GetElementsByTagName("Orders");
                XmlNodeList oList = ((XmlElement)oders[0]).GetElementsByTagName("Order");
                foreach (XmlElement order in oList)
                {
                    Order o = new Order();
                    o.Id = Convert.ToInt16(order.GetAttribute("id"));
                    if (o.Id > lastOrderID) lastOrderID = o.Id;
                    XmlNodeList table = ((XmlElement)order).GetElementsByTagName("Table");
                    o.TableID = Convert.ToInt16(table[0].InnerText);
                    XmlNodeList product = ((XmlElement)order).GetElementsByTagName("Product");
                    o.Product = Convert.ToString(product[0].InnerText);
                    XmlNodeList amount = ((XmlElement)order).GetElementsByTagName("Amount");
                    o.Amount = Convert.ToInt16(amount[0].InnerText);
                    XmlNodeList status = ((XmlElement)order).GetElementsByTagName("Status");
                    o.Status = Convert.ToInt16(status[0].InnerText);
                    XmlNodeList date = ((XmlElement)order).GetElementsByTagName("Date");
                    o.Date = Convert.ToDateTime(date[0].InnerText);
                    loo.Add(o);
                }
            }
            else lastOrderID = 0;
            return loo;
        }

        public string manageNFCOrder(string xml)
        {
            string reply = "Su pedido esta en camino...";
            try
            {
                List<Object> objects = xmlNFCOrderDecoder(xml);
                int tableID = adapter.sendMeTable((string)objects[0]);
                if (tableID != 0)
                {
                    foreach (Order order in (List<Order>)objects[1])
                    {
                        order.TableID = tableID;
                        if (order.Product.Equals("Solicitud de facturacion"))
                            reply = adapter.sendMeBill(tableID, true);
                        else addOrders(new List<Order> { order });
                    }
                }
            }
            catch (Exception e) { reply = "ERROR al procesar el pedido\n" + e.ToString(); }
            return reply;
        }

        private List<Object> xmlNFCOrderDecoder(string sXml)
        {
            List<Object> objects = new List<Object>();
            List<Order> loo = new List<Order>();
            if (!sXml.Equals(""))
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(sXml);
                XmlNodeList cOrder = xml.GetElementsByTagName("ClientOrder");
                XmlNodeList client = ((XmlElement)cOrder[0]).GetElementsByTagName("Client");
                string dni = Convert.ToString(((XmlElement)client[0]).GetAttribute("dni"));
                objects.Add(dni);
                XmlNodeList products = ((XmlElement)cOrder[0]).GetElementsByTagName("Products");
                XmlNodeList pList = ((XmlElement)products[0]).GetElementsByTagName("Product");
                foreach (XmlElement product in pList)
                {
                    Order o = new Order();
                    o.Product = Convert.ToString(product.GetAttribute("name"));
                    o.Amount = Convert.ToInt16(product.GetAttribute("amount"));
                    loo.Add(o);
                }
            }
            objects.Add(loo);
            return objects;
        }

        public string xmlNewOrder(List<Order> orders)
        {
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            xml += "\n<Orders>\n";
            if (orders.Count > 0)
                foreach (Order order in orders)
                {
                    xml += "\t<Order id=\"" + order.Id + "\">\n";
                    xml += "\t\t<Table>" + order.TableID + "</Table>\n";
                    xml += "\t\t<Product>" + order.Product + "</Product>\n";
                    xml += "\t\t<Amount>" + order.Amount + "</Amount>\n";
                    xml += "\t\t<Status>" + order.Status + "</Status>\n";
                    xml += "\t\t<Date>" + order.Date.ToString() + "</Date>\n";
                    xml += "\t</Order>\n";
                }
            xml += "</Orders>";
            return xml;
        }

        public void addOrders(List<Order> orders)
        {
            foreach (Order order in orders)
            {
                order.Id = ++lastOrderID;
                order.Status = 0;
                order.Date = DateTime.Now;
                Orders.Add(order);
            }
            if (orders.Count > 0)
            {
                adapter.sendNewOrder(xmlNewOrder(orders));
                gui.delegateToChangeTheOrdersList(Orders);
            }
        }

        public Order getOrder(int orderID)
        {
            return Orders[Orders.IndexOf(new Order(orderID))];
        }

        public void removeOrder(int orderID)
        {
            adapter.sendChangeOrderStatus(orderID, -2);
            Orders.Remove(Orders[Orders.IndexOf(new Order(orderID))]);
            gui.delegateToChangeTheOrdersList(Orders);
        }

        public void changeOrderAmount(int orderID, int amount)
        {
            if (Orders[Orders.IndexOf(new Order(orderID))].Amount != amount)
            {
                Orders[Orders.IndexOf(new Order(orderID))].Amount = amount;
                adapter.sendChangeOrderAmount(orderID, amount);
                gui.delegateToChangeTheOrdersList(Orders);
            }
        }

        public void changeOrderStatus(int orderID, int status)
        {
            if (Orders[Orders.IndexOf(new Order(orderID))].Status != status)
            {
                Orders[Orders.IndexOf(new Order(orderID))].Status = status;
                manager.RoomManager.xmlTablesStatus(adapter.sendChangeOrderStatus(orderID, status));
                gui.delegateToChangeTheOrdersList(Orders);
            }
        }

        public void changeOrderTable(int orderID, int tableID)
        {
            if (Orders[Orders.IndexOf(new Order(orderID))].TableID != tableID)
            {
                Orders[Orders.IndexOf(new Order(orderID))].TableID = tableID;
                manager.RoomManager.xmlTablesStatus(adapter.sendChangeOrderTable(orderID, tableID));
                gui.delegateToChangeTheOrdersList(Orders);
            }
        }

        public void markOrdersAsPaid(int tableID)
        {
            foreach (Order o in Orders)
                if (o.TableID == tableID)
                    o.Status = 3;
            gui.delegateToChangeTheOrdersList(Orders);
        }
    }

    public interface SubjectLO
    {
        void registerInterest(ObserverLO obs);
    }

    public interface ObserverLO
    {
        void notifyChangesInListOfOrders(List<Order> orders);
    }

    class DelegateOfListOfOrders
    {
        public delegate void OrdersDelegate();

        public event OrdersDelegate switchListOfOrders;

        public object changeContentsListOfOrders
        {
            set
            {
                switchListOfOrders();
            }
        }
    }
}
