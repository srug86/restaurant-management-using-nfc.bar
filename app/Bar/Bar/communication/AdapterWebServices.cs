using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Bar.communication
{
    class AdapterWebServices
    {
        private string url;

        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        private webSercives.MobiCartaWebServicesSoapClient proxy = new webSercives.MobiCartaWebServicesSoapClient();

        static readonly AdapterWebServices instance = new AdapterWebServices();

        static AdapterWebServices() { }

        AdapterWebServices() { }

        public static AdapterWebServices Instance
        {
            get
            {
                return instance;
            }
        }

        public string sendMeRooms()
        {
            return proxy.getRooms();
        }

        public string sendMeCurrentRoom()
        {
            return proxy.getCurrentRoom();
        }

        public string sendMeRoom(string name)
        {
            return proxy.getRoom(name);
        }

        public string sendMeTablesStatus()
        {
            return proxy.getTablesStatus();
        }

        public int sendMeTable(string dni)
        {
            return proxy.getTableID(dni);
        }

        public string sendMeTableStatus(int tableID)
        {
            return proxy.getTableStatus(tableID);
        }

        public void sendProductList(string products)
        {
            proxy.saveProductsList(products);
        }

        public string sendMeProducts(bool nonVisible)
        {
            return proxy.getProducts(nonVisible);
        }

        public string sendMeOrdersStatus()
        {
            return proxy.getOrdersStatus();
        }

        public string sendNewOrder(string order)
        {
            return proxy.addNewOrder(order);
        }

        public string sendChangeOrderStatus(int orderID, int status)
        {
            return proxy.setOrderStatus(orderID, status);
        }

        public void sendChangeOrderAmount(int orderID, int amount)
        {
            proxy.setProductAmount(orderID, amount);
        }

        public string sendChangeOrderTable(int orderID, int tableID)
        {
            return proxy.setOrderTable(orderID, tableID);
        }

        public string sendMeBill(int tableID, bool _short)
        {
            return proxy.getBill(tableID, _short);
        }

        public string sendBillPayment(int billID, int type)
        {
            return proxy.payBill(billID, type);
        }

        public void sendResetJourney(string room)
        {
            proxy.resetJourney(room);
        }
    }
}
