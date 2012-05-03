﻿using System;
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

        public bool connect(string url)
        {
            try
            {
                Url = url;
                return proxy.connect();
            }
            catch (Exception e) { return false; }
        }

        public string sendMeRooms()
        {
            try
            {
                return proxy.getRooms();
            }
            catch (EndpointNotFoundException e)
            {
                return "";
            }
        }

        public string sendMeRoom(string name)
        {
            try
            {
                return proxy.getRoom(name);
            }
            catch (EndpointNotFoundException e)
            {
                return "";
            }
        }

        public string sendMeTablesStatus()
        {
            return proxy.getTablesStatus();
        }

        public string sendMeTableStatus(int tableID)
        {
            return proxy.getTableStatus(tableID);
        }

        public string sendMeProducts()
        {
            return proxy.getProducts();
        }

        public string sendMeOrdersStatus()
        {
            return proxy.getOrdersStatus();
        }

        public string sendBillPaid(string bill)
        {
            return proxy.addBillPaid(bill);
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

        public void sendResetJourney(string room)
        {
            proxy.resetJourney(room);
        }
    }
}
