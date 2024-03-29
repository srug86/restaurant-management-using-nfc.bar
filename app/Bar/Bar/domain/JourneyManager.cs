﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bar.communication;
using System.Xml;

namespace Bar.domain
{
    class JourneyManager
    {
        private AdapterWebServices adapter = AdapterWebServices.Instance;

        private BluetoothServer bluetooth = BluetoothServer.Instance;

        static readonly JourneyManager instance = new JourneyManager();

        /* Atributos de la clase */
        // Gestor de las mesas del restaurante
        RoomManager roomManager;
        internal RoomManager RoomManager
        {
            get { return roomManager; }
            set { roomManager = value; }
        }

        // Gestor de pedidos del restaurante
        OrdersManager ordersManager;
        internal OrdersManager OrdersManager
        {
            get { return ordersManager; }
            set { ordersManager = value; }
        }

        // Gestor de productos del restaurante
        ProductsManager productsManager;
        internal ProductsManager ProductsManager
        {
            get { return productsManager; }
            set { productsManager = value; }
        }

        // Gestor de facturas del restaurante
        BillsManager billsManager;
        internal BillsManager BillsManager
        {
            get { return billsManager; }
            set { billsManager = value; }
        }

        /* Implementación de un 'Singleton' para esta clase */
        static JourneyManager() { }

        JourneyManager() { }

        public static JourneyManager Instance
        {
            get
            {
                return instance;
            }
        }

        // Inicializa el servidor Bluetooth
        public void initBluetoothServer()
        {
            bluetooth.initBluetooth();
        }
        
        // Cierra el servidor Bluetooth
        public void closeBluetoothServer()
        {
            bluetooth.closeBluetooth();
        }

        // Devuelve el listado de plantillas del restaurante
        public List<RoomInf> consultingRooms()
        {
            return xmlListOfRooms(adapter.sendMeRooms());
        }

        // Devuelve la plantilla de la jornada actual
        public List<RoomInf> consultingCurrentRoom()
        {
            return xmlListOfRooms(adapter.sendMeCurrentRoom());
        }

        // Inicializa todos los gestores
        public void createRoomManager()
        {
            initRoomManager();
            initOrdersManager();
            initProductsManager();
            initBillsManager();
        }

        // Inicializa el gestor de mesas
        private void initRoomManager()
        {
            roomManager = new RoomManager();
        }

        // Inicializa el gestor de pedidos
        public void initOrdersManager()
        {
            ordersManager = OrdersManager.Instance;
        }

        // Inicializa el gestor de productos
        public void initProductsManager()
        {
            productsManager = new ProductsManager();
        }

        // Inicializa el gestor de facturas
        public void initBillsManager()
        {
            billsManager = BillsManager.Instance;
        }

        // 'Resetea' los valores de la jornada actual para iniciar una nueva jornada
        public void resetCurrentJourney(string room)
        {
            adapter.sendResetJourney(room);
        }

        // Decodifica XML de la lista de plantillas para el restaurante
        private List<RoomInf> xmlListOfRooms(string sXml)
        {
            List<RoomInf> listOfRooms = new List<RoomInf>();
            if (sXml != "")
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(sXml);
                XmlNodeList rooms = xml.GetElementsByTagName("Rooms");
                XmlNodeList rList = ((XmlElement)rooms[0]).GetElementsByTagName("Room");
                foreach (XmlElement room in rList)
                {
                    RoomInf roomInf = new RoomInf();
                    roomInf.Name = Convert.ToString(room.GetAttribute("name"));
                    XmlNodeList width = ((XmlElement)room).GetElementsByTagName("Width");
                    roomInf.Width = Convert.ToInt16(width[0].InnerText);
                    XmlNodeList height = ((XmlElement)room).GetElementsByTagName("Height");
                    roomInf.Height = Convert.ToInt16(height[0].InnerText);
                    XmlNodeList nTables = ((XmlElement)room).GetElementsByTagName("Tables");
                    roomInf.Tables = Convert.ToInt16(nTables[0].InnerText);
                    XmlNodeList tCapacity = ((XmlElement)room).GetElementsByTagName("Capacity");
                    roomInf.Capacity = Convert.ToInt16(tCapacity[0].InnerText);
                    listOfRooms.Add(roomInf);
                }
            }
            return listOfRooms;
        }
    }
}
