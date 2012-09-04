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

        private List<ObserverLO> loObservers = new List<ObserverLO>();

        /* Atributos de la clase */
        private List<Order> orders = new List<Order>();
        internal List<Order> Orders
        {
            get { return orders; }
            set { orders = value; }
        }

        private JourneyManagerWin gui;

        private int lastOrderID;

        static readonly OrdersManager instance = new OrdersManager();

        /* Implementación de un 'Singleton' para esta clase */
        static OrdersManager() { }

        OrdersManager() { }

        public static OrdersManager Instance
        {
            get
            {
                return instance;
            }
        }

        // Actualiza el estado de los pedidos
        public void updateOrders()
        {
            Orders = xmlListOfOrders(adapter.sendMeOrdersStatus());
            gui.delegateToChangeTheOrdersList(Orders);
        }

        // Obtiene la lista de pedidos históricos
        public List<HOrder> getHistoricalOrders(int amount, bool ascending)
        {
            return xmlHOrdersDecoder(adapter.sendMeHOrders(amount, ascending));
        }

        // Calcula la respuesta a la solicitud de un pedido NFC
        public string manageNFCOrder(string xml)
        {
            string reply = "Su pedido esta en camino...";
            try
            {
                List<Object> objects = xmlNFCOrderDecoder(xml);
                int tableID = adapter.sendMeTable((string)objects[0]);
                if (tableID != 0)   // Solicitud de facturación
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

        // Añadir pedido a la lista de pedidos
        public void addOrders(List<Order> orders)
        {
            foreach (Order order in orders)
            {
                order.Id = ++lastOrderID;
                order.Status = 0;
                order.Date = DateTime.Now;
                Orders.Add(order);
                gui.delegateToShowBarEvent(2, order, order.TableID);
            }
            if (orders.Count > 0)
            {
                adapter.sendNewOrder(xmlNewOrder(orders));
                gui.delegateToChangeTheOrdersList(Orders);
            }
        }

        // Devuelve la información de un pedido a través de su identificador
        public Order getOrder(int orderID)
        {
            return Orders[Orders.IndexOf(new Order(orderID))];
        }

        // Elimina pedido de la lista a través de su identificador
        public void removeOrder(int orderID)
        {
            Order o = Orders[Orders.IndexOf(new Order(orderID))];
            adapter.sendChangeOrderStatus(orderID, -2);
            Orders.Remove(o);
            gui.delegateToShowBarEvent(3, o, o.TableID);
            gui.delegateToChangeTheOrdersList(Orders);
        }

        // Edita la cantidad de productos de un tipo en un pedido
        public void changeOrderAmount(int orderID, int amount)
        {
            Order o = Orders[Orders.IndexOf(new Order(orderID))];
            if (o.Amount != amount)
            {
                o.Amount = amount;
                adapter.sendChangeOrderAmount(orderID, amount);
                gui.delegateToShowBarEvent(4, o, o.TableID);
                gui.delegateToChangeTheOrdersList(Orders);
            }
        }

        // Edita el estado del pedido
        public void changeOrderStatus(int orderID, int status)
        {
            Order o = Orders[Orders.IndexOf(new Order(orderID))];
            if (o.Status != status)
            {
                o.Status = status;
                manager.RoomManager.xmlTablesStatus(adapter.sendChangeOrderStatus(orderID, status));
                int eventN = 4;
                switch (status)
                {
                    case -1: eventN = 9; break; // Detenido
                    case 0: eventN = 6; break;  // No atendido
                    case 1: eventN = 7; break;  // Atentido
                    case 2: eventN = 8; break;  // Servido
                }
                gui.delegateToShowBarEvent(eventN, o, o.TableID);
                gui.delegateToChangeTheOrdersList(Orders);
            }
        }

        // Cambia el pedido de mesa
        public void changeOrderTable(int orderID, int tableID)
        {
            Order o = Orders[Orders.IndexOf(new Order(orderID))];
            if (o.TableID != tableID)
            {
                o.TableID = tableID;
                manager.RoomManager.xmlTablesStatus(adapter.sendChangeOrderTable(orderID, tableID));
                gui.delegateToShowBarEvent(4, o, tableID);
                gui.delegateToChangeTheOrdersList(Orders);
            }
        }

        // Marca como 'Pagados' los pedidos de una mesa
        public void markOrdersAsPaid(int tableID)
        {
            foreach (Order o in Orders)
                if (o.TableID == tableID)
                    o.Status = 3;
            gui.delegateToChangeTheOrdersList(Orders);
        }

        public void setGuiReference(JourneyManagerWin gui)
        {
            this.gui = gui;
        }

        // Decodifica XML con el listado actualizado de los pedidos
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

        // Decodifica XML de un pedido NFC
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

        // Decodifica XML de un listado de pedidos históricos
        private List<HOrder> xmlHOrdersDecoder(string sXml)
        {
            List<HOrder> lho = new List<HOrder>();
            if (sXml != "")
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(sXml);
                XmlNodeList hOrders = xml.GetElementsByTagName("Historical");
                XmlNodeList hoList = ((XmlElement)hOrders[0]).GetElementsByTagName("Order");
                foreach (XmlElement hOrder in hoList)
                {
                    HOrder ho = new HOrder();
                    XmlNodeList client = ((XmlElement)hOrder).GetElementsByTagName("Client");
                    ho.Client = Convert.ToString(client[0].InnerText).Trim();
                    XmlNodeList product = ((XmlElement)hOrder).GetElementsByTagName("Product");
                    ho.Product = Convert.ToString(product[0].InnerText).Trim();
                    XmlNodeList amount = ((XmlElement)hOrder).GetElementsByTagName("Amount");
                    ho.Amount = Convert.ToInt32(amount[0].InnerText);
                    XmlNodeList date = ((XmlElement)hOrder).GetElementsByTagName("Date");
                    ho.Date = Convert.ToDateTime(date[0].InnerText);
                    lho.Add(ho);
                }
            }
            return lho;
        }

        // Codifica XML con la lista actual de pedidos
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
