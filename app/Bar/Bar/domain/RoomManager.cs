using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bar.communication;
using System.Xml;

namespace Bar.domain
{
    class RoomManager : SubjectRE
    {
        private AdapterWebServices adapter = AdapterWebServices.Instance;

        private List<ObserverRE> reObservers = new List<ObserverRE>();

        /* Atributos de la clase */
        private RoomDef room;
        internal RoomDef Room
        {
            get { return room; }
            set { room = value; }
        }

        private int selectedTable, rowSelected, columnSelected, boxStatus;

        public int RowSelected
        {
            get { return rowSelected; }
            set { rowSelected = value; }
        }

        public int ColumnSelected
        {
            get { return columnSelected; }
            set { columnSelected = value; }
        }

        public int BoxStatus
        {
            get { return boxStatus; }
            set { boxStatus = value; }
        }

        private TableInf tableInfo;
        internal TableInf TableInfo
        {
            get { return tableInfo; }
            set { tableInfo = value; }
        }

        private Client clientInfo;
        internal Client ClientInfo
        {
            get { return clientInfo; }
            set { clientInfo = value; }
        }

        private List<Order> ordersInfo;
        internal List<Order> OrdersInfo
        {
            get { return ordersInfo; }
            set { ordersInfo = value; }
        }

        // Método constructor
        public RoomManager() { }

        // Cargar plantilla
        public void loadRoom(string name, bool newJourney)
        {
            xmlDistributionOfRoom(adapter.sendMeRoom(name, newJourney));
        }

        // Calcula el evento producido tras la selección de una casilla de la plantilla
        public int selectedBox(int row, int column)
        {
            int tableID = findTable(row, column);
            if (tableID != -1)
            {
                TableInf table = Room.Tables[Room.Tables.IndexOf(new TableInf(tableID))];
                if (table.Status != -1)
                {
                    if (selectedTable > 0)
                    {
                        TableInf prevTable = Room.Tables[Room.Tables.IndexOf(new TableInf(selectedTable))];
                        paintTable(prevTable, prevTable.Status);
                    }
                    paintTable(table, 6);
                    selectedTable = table.Id;
                    return table.Id;
                }
                return -1;
            }
            return -1;
        }

        // Cambia de color las casillas de una mesa
        private void paintTable(TableInf table, int colour)
        {
            foreach (int[] coordinates in table.Place)
            {
                RowSelected = coordinates[0];
                ColumnSelected = coordinates[1];
                BoxStatus = colour;
                boxStatusHasChanged();
            }
        }

        // Calcula cuál es el cliente de una mesa
        public string getClientsTable(int table)
        {
            return Room.Tables[Room.Tables.IndexOf(new TableInf(table))].Client;
        }

        // Devuelve el estado de la mesa
        public List<Object> updateTable(int tableID)
        {
            List<Object> info = new List<Object>();
            xmlTableInfo(adapter.sendMeTableStatus(tableID));
            info.Add(clientInfo);
            info.Add(tableInfo);
            info.Add(ordersInfo);
            return info;
        }

        // Actualiza el estado de las mesas
        public void updateTables()
        {
            xmlTablesStatus(adapter.sendMeTablesStatus());
            refreshTables();
        }

        // Devuelve las mesas candidatas a realizar algún pedido
        public List<int> getCandidateTables()
        {
            List<int> tables = new List<int>();
            xmlTablesStatus(adapter.sendMeTablesStatus());
            foreach (TableInf table in Room.Tables)
                if (table.Status >= 0)
                    tables.Add(table.Id);
            return tables;
        }

        // Decodifica el XML con la información de la plantilla del restaurante
        private void xmlDistributionOfRoom(string sXml)
        {
            if (sXml != "")
            {
                Room = new RoomDef();
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(sXml);
                XmlNodeList _room = xml.GetElementsByTagName("Room");
                Room.Name = Convert.ToString(((XmlElement)_room[0]).GetAttribute("name"));
                XmlNodeList dimension = ((XmlElement)_room[0]).GetElementsByTagName("Dimension");
                XmlNodeList width = ((XmlElement)dimension[0]).GetElementsByTagName("Width");
                Room.Width = Convert.ToInt16(width[0].InnerText);
                XmlNodeList height = ((XmlElement)dimension[0]).GetElementsByTagName("Height");
                Room.Height = Convert.ToInt16(height[0].InnerText);
                XmlNodeList receiver = ((XmlElement)_room[0]).GetElementsByTagName("Receiver");
                XmlNodeList rBoxes = ((XmlElement)receiver[0]).GetElementsByTagName("Boxes");
                XmlNodeList rBoxesList = ((XmlElement)rBoxes[0]).GetElementsByTagName("Box");
                Room.Receiver = new List<int[]>();
                readBoxes(rBoxesList, room.Receiver);
                XmlNodeList bar = ((XmlElement)_room[0]).GetElementsByTagName("Bar");
                XmlNodeList bBoxes = ((XmlElement)bar[0]).GetElementsByTagName("Boxes");
                XmlNodeList bBoxesList = ((XmlElement)bBoxes[0]).GetElementsByTagName("Box");
                Room.Bar = new List<int[]>();
                readBoxes(bBoxesList, room.Bar);
                XmlNodeList tables = ((XmlElement)_room[0]).GetElementsByTagName("Tables");
                XmlNodeList tList = ((XmlElement)tables[0]).GetElementsByTagName("Table");
                Room.Tables = new List<TableInf>();
                foreach (XmlElement table in tList)
                {
                    TableInf td = new TableInf();
                    td.Id = Convert.ToInt16(table.GetAttribute("id"));
                    td.Capacity = Convert.ToInt16(table.GetAttribute("capacity"));
                    XmlNodeList tBoxes = ((XmlElement)table).GetElementsByTagName("Boxes");
                    XmlNodeList tBoxesList = ((XmlElement)tBoxes[0]).GetElementsByTagName("Box");
                    readBoxes(tBoxesList, td.Place);
                    Room.Tables.Add(td);
                }
            }
        }

        // Decodifica el XML de la ubicación de las mesas de la plantilla
        private void readBoxes(XmlNodeList srcList, List<int[]> dstList)
        {
            foreach (XmlElement node in srcList)
            {
                XmlNodeList x = node.GetElementsByTagName("X");
                XmlNodeList y = node.GetElementsByTagName("Y");
                dstList.Add(new int[2] { Convert.ToInt16(y[0].InnerText), Convert.ToInt16(x[0].InnerText) });
            }
        }

        // Decodifica el XML del estado de las mesas del restaurante
        public void xmlTablesStatus(string sXml)
        {
            if (sXml != "")
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(sXml);
                XmlNodeList tables = xml.GetElementsByTagName("Tables");
                XmlNodeList tList = ((XmlElement)tables[0]).GetElementsByTagName("Table");
                foreach (XmlElement table in tList)
                {
                    int id = Convert.ToInt16(table.GetAttribute("id"));
                    TableInf taux = new TableInf(id);
                    XmlNodeList status = ((XmlElement)table).GetElementsByTagName("Status");
                    Room.Tables[Room.Tables.IndexOf(taux)].Status = Convert.ToInt16(status[0].InnerText);
                    XmlNodeList client = ((XmlElement)table).GetElementsByTagName("Client");
                    Room.Tables[Room.Tables.IndexOf(taux)].Client = Convert.ToString(client[0].InnerText);
                    XmlNodeList guests = ((XmlElement)table).GetElementsByTagName("Guests");
                    Room.Tables[Room.Tables.IndexOf(taux)].Guests = Convert.ToInt16(guests[0].InnerText);
                }
            }
        }

        // Decodifica el XML del estado de una mesa
        private void xmlTableInfo(string sXml)
        {
            if (sXml != "")
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(sXml);
                XmlNodeList tableinf = xml.GetElementsByTagName("TableInf");
                XmlNodeList table = ((XmlElement)tableinf[0]).GetElementsByTagName("Table");
                XmlNodeList tableID = ((XmlElement)table[0]).GetElementsByTagName("Id");
                TableInfo = new TableInf();
                TableInfo.Id = Convert.ToInt16(tableID[0].InnerText);
                XmlNodeList capacity = ((XmlElement)table[0]).GetElementsByTagName("Capacity");
                TableInfo.Capacity = Convert.ToInt16(capacity[0].InnerText);
                XmlNodeList status = ((XmlElement)table[0]).GetElementsByTagName("Status");
                TableInfo.Status = Convert.ToInt16(status[0].InnerText);
                XmlNodeList client = ((XmlElement)tableinf[0]).GetElementsByTagName("Client");
                XmlNodeList dni = ((XmlElement)client[0]).GetElementsByTagName("DNI");
                ClientInfo = new Client();
                ClientInfo.Dni = Convert.ToString(dni[0].InnerText).Trim();
                XmlNodeList name = ((XmlElement)client[0]).GetElementsByTagName("Name");
                ClientInfo.Name = Convert.ToString(name[0].InnerText).Trim();
                XmlNodeList surname = ((XmlElement)client[0]).GetElementsByTagName("Surname");
                ClientInfo.Surname = Convert.ToString(surname[0].InnerText).Trim();
                XmlNodeList guests = ((XmlElement)client[0]).GetElementsByTagName("Guests");
                TableInfo.Guests = Convert.ToInt16(guests[0].InnerText);
                XmlNodeList appearances = ((XmlElement)client[0]).GetElementsByTagName("Appearances");
                ClientInfo.Appearances = Convert.ToInt16(appearances[0].InnerText);
                XmlNodeList orders = ((XmlElement)tableinf[0]).GetElementsByTagName("Orders");
                XmlNodeList oList = ((XmlElement)orders[0]).GetElementsByTagName("Order");
                OrdersInfo = new List<Order>();
                foreach (XmlElement order in oList)
                {
                    Order o = new Order();
                    o.Id = Convert.ToInt16(order.GetAttribute("id"));
                    XmlNodeList product = ((XmlElement)order).GetElementsByTagName("Product");
                    o.Product = Convert.ToString(product[0].InnerText).Trim();
                    XmlNodeList amount = ((XmlElement)order).GetElementsByTagName("Amount");
                    o.Amount = Convert.ToInt16(amount[0].InnerText);
                    XmlNodeList state = ((XmlElement)order).GetElementsByTagName("Status");
                    o.Status = Convert.ToInt16(state[0].InnerText);
                    XmlNodeList date = ((XmlElement)order).GetElementsByTagName("Date");
                    o.Date = Convert.ToDateTime(date[0].InnerText);
                    OrdersInfo.Add(o);
                }
            }
        }

        // Coloca los objetos del restaurante en la plantilla
        public void locateObjects()
        {
            foreach (int[] coordinates in Room.Receiver)
            {
                RowSelected = coordinates[0];
                ColumnSelected = coordinates[1];
                BoxStatus = 4;
                boxStatusHasChanged();
            }
            foreach (int[] coordinates in Room.Bar)
            {
                RowSelected = coordinates[0];
                ColumnSelected = coordinates[1];
                BoxStatus = 5;
                boxStatusHasChanged();
            }
        }

        // Actualiza el estado de las mesas del restaurante
        private void refreshTables()
        {
            foreach (TableInf table in Room.Tables)
                foreach (int[] coordinates in table.Place)
                {
                    RowSelected = coordinates[0];
                    ColumnSelected = coordinates[1];
                    BoxStatus = table.Status;
                    boxStatusHasChanged();
                }
        }

        // Devuelve el identificador de la mesa que ocupa una coordenada dada
        private int findTable(int row, int column)
        {
            foreach (TableInf table in Room.Tables)
                foreach (int[] coordinates in table.Place)
                    if (coordinates[0] == row && coordinates[1] == column)
                        return table.Id;
            return -1;
        }

        /* Métodos que implementan la función 'Observable' */
        // Se notifica a los observadores ante el cambio de estado de una casilla en la plantilla
        private void switchBox()
        {
            for (int i = 0; i < reObservers.Count; i++)
            {
                ObserverRE obs = (ObserverRE)reObservers[i];
                obs.notifyChangesInABox(RowSelected, ColumnSelected, BoxStatus);
            }
        }

        private void boxStatusHasChanged()
        {
            DelegateOfTheBox oSwitchBox = new DelegateOfTheBox();
            DelegateOfTheBox.BoxDelegate oBoxDelegate = new DelegateOfTheBox.BoxDelegate(switchBox);
            oSwitchBox.switchBox += oBoxDelegate;
            oSwitchBox.changeContentsBox = BoxStatus;

            oSwitchBox.switchBox -= oBoxDelegate;
        }

        public void registerInterest(ObserverRE obs)
        {
            reObservers.Add(obs);
        }
    }

    public interface SubjectRE
    {
        void registerInterest(ObserverRE obs);
    }

    public interface ObserverRE
    {
        void notifyChangesInABox(int row, int column, int state);
    }

    class DelegateOfTheBox
    {
        public delegate void BoxDelegate();

        public event BoxDelegate switchBox;

        public object changeContentsBox
        {
            set
            {
                switchBox();
            }
        }
    }
}
