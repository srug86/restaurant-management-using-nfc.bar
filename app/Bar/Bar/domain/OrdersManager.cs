using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bar.communication;
using System.Xml;

namespace Bar.domain
{
    class OrdersManager : SubjectLO
    {
        private JourneyManager manager = JourneyManager.Instance;

        private AdapterWebServices adapter = AdapterWebServices.Instance;

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

        public void updateOrders()
        {
            Orders = xmlListOfOrders(adapter.sendMeOrdersStatus());
            listOfOrdersHasChanged();
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
                    xml += "\t\t<Date>" + dateTimeToString(order.Date) + "</Date>\n";
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
                listOfOrdersHasChanged();
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
            listOfOrdersHasChanged();
        }

        public void changeOrderAmount(int orderID, int amount)
        {
            if (Orders[Orders.IndexOf(new Order(orderID))].Amount != amount)
            {
                Orders[Orders.IndexOf(new Order(orderID))].Amount = amount;
                adapter.sendChangeOrderAmount(orderID, amount);
                listOfOrdersHasChanged();
            }
        }

        public void changeOrderStatus(int orderID, int status)
        {
            if (Orders[Orders.IndexOf(new Order(orderID))].Status != status)
            {
                Orders[Orders.IndexOf(new Order(orderID))].Status = status;
                manager.RoomManager.xmlTablesStatus(adapter.sendChangeOrderStatus(orderID, status));
                listOfOrdersHasChanged();
            }
        }

        public void changeOrderTable(int orderID, int tableID)
        {
            if (Orders[Orders.IndexOf(new Order(orderID))].TableID != tableID)
            {
                Orders[Orders.IndexOf(new Order(orderID))].TableID = tableID;
                manager.RoomManager.xmlTablesStatus(adapter.sendChangeOrderTable(orderID, tableID));
                listOfOrdersHasChanged();
            }
        }

        public Bill generateBill(int table)
        {
            Bill bill = new Bill();
            return bill;
        }

        public void confirmPayment(int table)
        {
        }

        private static string dateTimeToString(DateTime date)
        {
            string month = date.Month.ToString(), day = date.Day.ToString(), hour = date.Hour.ToString(),
                minute = date.Minute.ToString(), second = date.Second.ToString();
            if (date.Month < 10) month = "0" + date.Month.ToString();
            if (date.Day < 10) day = "0" + date.Day.ToString();
            if (date.Hour < 10) hour = "0" + date.Hour.ToString();
            if (date.Minute < 10) minute = "0" + date.Minute.ToString();
            if (date.Second < 10) second = "0" + date.Second.ToString();
            return date.Year.ToString() + "-" + month + "-" + day + " " +
                hour + ":" + minute + ":" + second;
        }

        private void switchListOfOrders()
        {
            for (int i = 0; i < loObservers.Count; i++)
            {
                ObserverLO obs = (ObserverLO)loObservers[i];
                obs.notifyChangesInListOfOrders(Orders);
            }
        }

        private void listOfOrdersHasChanged()
        {
            DelegateOfListOfOrders oSwitchLoO = new DelegateOfListOfOrders();
            DelegateOfListOfOrders.OrdersDelegate oLoODelegate = new DelegateOfListOfOrders.OrdersDelegate(switchListOfOrders);
            oSwitchLoO.switchListOfOrders += oLoODelegate;
            oSwitchLoO.changeContentsListOfOrders = Orders;

            oSwitchLoO.switchListOfOrders -= oLoODelegate;
        }

        public void registerInterest(ObserverLO obs)
        {
            loObservers.Add(obs);
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
