using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Bar.domain;

namespace Bar.presentation
{
    /// <summary>
    /// Lógica de interacción para JourneyManagerWin.xaml
    /// </summary>
    public partial class JourneyManagerWin : Window, ObserverRE, ObserverLO
    {
        private JourneyManager manager = JourneyManager.Instance;

        private string roomName;

        public string RoomName
        {
            get { return roomName; }
            set { roomName = value; }
        }

        private int roomHeight, roomWidth;

        public int RoomHeight
        {
            get { return roomHeight; }
            set { roomHeight = value; }
        }

        public int RoomWidth
        {
            get { return roomWidth; }
            set { roomWidth = value; }
        }

        private Image[,] room;

        public Image[,] Room
        {
            get { return room; }
            set { room = value; }
        }

        private Grid[] gridsOnMode;

        private ConnectDialog connect;

        private Dictionary<int, string> colorBox = new Dictionary<int, string> {
            {-1, "/Bar;component/Images/black.jpg"},
            {0, "/Bar;component/Images/green.jpg"},
            {1, "/Bar;component/Images/white.jpg"},
            {2, "/Bar;component/Images/dgreen.jpg"},
            {3, "/Bar;component/Images/orange.jpg"},
            {4, "/Bar;component/Images/red.jpg"},
            {5, "/Bar;component/Images/yellow.jpg"},
            {6, "/Bar;component/Images/pgreen.jpg"},
        };

        private Dictionary<int, string> tableStatus = new Dictionary<int,string> {
            {-1, "Vacía"}, {0, "Ocupada"}, {1, "Esperando pedido"}, {2, "Servida"}, {3, "Cobrada"},};

        private Dictionary<int, string> orderStatus = new Dictionary<int, string> {
            {-1, "Detenido"}, {0, "No atendido"}, {1, "Atendido"}, {2, "Servido"}, {3, "Pagado"},};

        private Dictionary<int, Brush> itemColor = new Dictionary<int, Brush> {
            {-1, Brushes.IndianRed}, {0, Brushes.WhiteSmoke}, {1, Brushes.Yellow},
            {2, Brushes.LightGreen}, {3, Brushes.Orange},};

        public JourneyManagerWin()
        {
            connect = new ConnectDialog();
            InitializeComponent();
            initGridsOnMode();
            openOffPerspective();
        }

        private void initGridsOnMode()
        {
            gridsOnMode = new Grid[] { gridORoomView, gridOOrdersList, gridOSeeTable };
            foreach (Grid g in gridsOnMode)
                g.Visibility = Visibility.Hidden;
        }

        private void showOnModeGrid(Grid grid)
        {
            foreach (Grid g in gridsOnMode)
            {
                if (g.Equals(grid)) g.Visibility = Visibility.Visible;
                else g.Visibility = Visibility.Hidden;
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            LoadRoomDialog roomDialog = new LoadRoomDialog(this, manager.consultingRooms(), true);
            roomDialog.Show();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadRoomDialog roomDialog = new LoadRoomDialog(this, manager.consultingRooms(), false);
            roomDialog.Show();
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            connect.Show();
        }

        private void btnOOrders_Click(object sender, RoutedEventArgs e)
        {
            openOrdersPerspective();
        }

        private void btnOView_Click(object sender, RoutedEventArgs e)
        {
            openViewPerspective();
        }

        private void btnSeeTable_Click(object sender, RoutedEventArgs e)
        {
            openTablesInfoPerspective();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            openViewPerspective();
        }

        private void btnAddOrder_Click(object sender, RoutedEventArgs e)
        {
            NewOrderWin newOrder = new NewOrderWin(manager.ProductsManager.Categories, manager.RoomManager.getCandidateTables());
            if (((Button)sender).Name == "btnAddOT") newOrder.selectTable(cbbTablesView.SelectedIndex);
            newOrder.Show();
        }

        private void btnRemoveOrder_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Name == "btnRemove" && listVOrders.SelectedIndex != -1)
                manager.OrdersManager.removeOrder(((OrderTableItem)((ListViewItem)listVOrders.SelectedItem).Content).OrderID);
            else if (((Button)sender).Name == "btnRemoveOT" && listVTablesOrders.SelectedIndex != -1)
            {
                manager.OrdersManager.removeOrder(((OrderTableItem)((ListViewItem)listVTablesOrders.SelectedItem).Content).OrderID);
                listVTablesOrders.Items.Remove(listVTablesOrders.SelectedItem);
            }
        }

        private void btnEditOrder_Click(object sender, RoutedEventArgs e)
        {
            EditOrderDialog editOrder;
            if (((Button)sender).Name == "btnEdit" && listVOrders.SelectedIndex != -1)
            {
                editOrder = new EditOrderDialog(manager.OrdersManager.getOrder(((OrderTableItem)((ListViewItem)listVOrders.SelectedItem).Content).OrderID));
                editOrder.Show();
            }
            else if (((Button)sender).Name == "btnEditOT" && listVTablesOrders.SelectedIndex != -1)
            {
                editOrder = new EditOrderDialog(manager.OrdersManager.getOrder(((OrderTableItem)((ListViewItem)listVTablesOrders.SelectedItem).Content).OrderID));
                editOrder.Show();
            }
        }

        private void btnUnattender_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Name == "btnUnattender" && listVOrders.SelectedIndex != -1)
                manager.OrdersManager.changeOrderStatus(((OrderTableItem)((ListViewItem)listVOrders.SelectedItem).Content).OrderID, 0);
            else if (((Button)sender).Name == "btnUnattenderOT" && listVTablesOrders.SelectedIndex != -1)
                manager.OrdersManager.changeOrderStatus(((OrderTableItem)((ListViewItem)listVTablesOrders.SelectedItem).Content).OrderID, 0);
        }

        private void btnAttended_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Name == "btnAttended" && listVOrders.SelectedIndex != -1)
                manager.OrdersManager.changeOrderStatus(((OrderTableItem)((ListViewItem)listVOrders.SelectedItem).Content).OrderID, 1);
            else if (((Button)sender).Name == "btnAttendedOT" && listVTablesOrders.SelectedIndex != -1)
                manager.OrdersManager.changeOrderStatus(((OrderTableItem)((ListViewItem)listVTablesOrders.SelectedItem).Content).OrderID, 1);
        }

        private void btnServed_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Name == "btnServed" && listVOrders.SelectedIndex != -1)
                manager.OrdersManager.changeOrderStatus(((OrderTableItem)((ListViewItem)listVOrders.SelectedItem).Content).OrderID, 2);
            else if (((Button)sender).Name == "btnServedOT" && listVTablesOrders.SelectedIndex != -1)
                manager.OrdersManager.changeOrderStatus(((OrderTableItem)((ListViewItem)listVTablesOrders.SelectedItem).Content).OrderID, 2);
        }

        private void btnStopped_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Name == "btnStopped" && listVOrders.SelectedIndex != -1)
                manager.OrdersManager.changeOrderStatus(((OrderTableItem)((ListViewItem)listVOrders.SelectedItem).Content).OrderID, -1);
            else if (((Button)sender).Name == "btnStoppedOT" && listVTablesOrders.SelectedIndex != -1)
                manager.OrdersManager.changeOrderStatus(((OrderTableItem)((ListViewItem)listVTablesOrders.SelectedItem).Content).OrderID, -1);
        }

        private void btnCheckIn_Click(object sender, RoutedEventArgs e)
        {
            BillDialog billDialog = new BillDialog(manager.BillsManager.getBill(Convert.ToInt16(txtbTableID.Text)));
            billDialog.Show();
        }

        public void loadSelectedRoom(string name, bool reset)
        {
            manager.createRoomManager();
            manager.RoomManager.loadRoom(name, reset);
            generateEmptyRoom(manager.RoomManager.Room.Name, 
                manager.RoomManager.Room.Height, manager.RoomManager.Room.Width);
            registerSubjects(manager.RoomManager, manager.OrdersManager);
            manager.RoomManager.locateObjects();
            if (reset) manager.resetCurrentJourney(name);
            else
            {
                manager.RoomManager.updateTables();
                manager.OrdersManager.updateOrders();
            }
            manager.ProductsManager.updateProducts();
            openOnPerspective();
        }

        private void generateEmptyRoom(string name, int height, int width)
        {
            RoomName = name;
            RoomHeight = height;
            RoomWidth = width;
            Room = new Image[RoomHeight, RoomWidth];
            if (uGridStatus.Children.Count > 0) uGridStatus.Children.Clear();
            uGridStatus.Rows = RoomHeight;
            uGridStatus.Columns = RoomWidth;
            for (int i = 0; i < RoomHeight; i++)
                for (int j = 0; j < RoomWidth; j++)
                {
                    Image image = new Image();
                    image.Name = "img" + i + "x" + j;
                    image.Stretch = Stretch.Fill;
                    image.MouseUp += new MouseButtonEventHandler(this.box_Click);
                    Room[i, j] = image;
                    uGridStatus.Children.Add(image);
                }
        }

        private void openOrdersPerspective()
        {
            showOnModeGrid(gridOOrdersList);
        }

        private void openTablesInfoPerspective()
        {
            if (Convert.ToInt16(txtbTableS.Text) > 0)
            {
                cleanTableData();
                loadTableData(Convert.ToInt16(txtbTableS.Text));
                showOnModeGrid(gridOSeeTable);
            }
        }

        private void openViewPerspective()
        {
            resetProvisionalData();
            manager.RoomManager.updateTables();
            showOnModeGrid(gridORoomView);
        }

        private void cleanTableData()
        {
            cbbTablesView.Items.Clear();
            txtbDNI.Text = txtbName.Text = txtbSurname.Text = txtbAppearances.Text =
                txtbTableID.Text = txtbTableOccupation.Text = txtbTableStatus.Text = "";
            btnCheckIn.Content = "Facturar";
            btnCheckIn.IsEnabled = false;
            listVTablesOrders.Items.Clear();
        }

        private void loadTableData(int tableID)
        {
            List<int> tables = manager.RoomManager.getCandidateTables();
            int i = 0;
            foreach (int table in tables)
            {
                cbbTablesView.Items.Add("Mesa " + table);
                if (tableID == table) cbbTablesView.SelectedIndex = i;
                i++;
            }
            List<Object> info = manager.RoomManager.updateTable(tableID);
            foreach (Object o in info)
            {
                if (o.GetType() == typeof(Client))
                {
                    txtbDNI.Text = ((Client)o).Dni;
                    txtbName.Text = ((Client)o).Name;
                    txtbSurname.Text = ((Client)o).Surname;
                    txtbAppearances.Text = Convert.ToString(((Client)o).Appearances);
                }
                else if (o.GetType() == typeof(Bar.domain.Table))
                {
                    txtbTableID.Text = Convert.ToString(((Bar.domain.Table)o).Id);
                    txtbTableStatus.Text = tableStatus[((Bar.domain.Table)o).Status];
                    if (((Bar.domain.Table)o).Status == 2)
                    {
                        btnCheckIn.IsEnabled = true;
                        btnCheckIn.Content = "Facturar";
                    }
                    else if (((Bar.domain.Table)o).Status == 3)
                    {
                        btnCheckIn.IsEnabled = true;
                        btnCheckIn.Content = "Ver factura";
                    }
                    txtbTableOccupation.Text = Convert.ToString(((Bar.domain.Table)o).Guests) +
                        "/" + Convert.ToString(((Bar.domain.Table)o).Capacity);
                }
                else if (o.GetType() == typeof(List<Order>))
                    foreach (Order order in ((List<Order>)o))
                    {
                        OrderTableItem item = new OrderTableItem();
                        item.OrderID = order.Id;
                        item.TableID = order.TableID;
                        item.Product = order.Product;
                        item.Amount = order.Amount;
                        item.State = orderStatus[order.Status];
                        item.Date = order.Date.ToString();
                        listVTablesOrders.Items.Add(new ListViewItem());
                        ((ListViewItem)listVTablesOrders.Items[listVTablesOrders.Items.Count - 1]).Content = item;
                        ((ListViewItem)listVTablesOrders.Items[listVTablesOrders.Items.Count - 1]).Background = itemColor[order.Status];
                    }
            }
        }

        private void box_Click(object sender, MouseButtonEventArgs e)
        {
            Image img = (Image)sender;
            string box = img.Name.Substring(3);
            string[] coordinates = box.Split('x');
            int selectedTable = manager.RoomManager.selectedBox(Convert.ToInt16(coordinates[0]),
                Convert.ToInt16(coordinates[1]));
            if (selectedTable != -1)
            {
                txtbTableS.Text = Convert.ToString(selectedTable);
                txtbClientS.Text = manager.RoomManager.getClientsTable(selectedTable);
                btnSeeTable.IsEnabled = true;
            }
        }

        private void btnConsultTable_Click(object sender, RoutedEventArgs e)
        {
            if (cbbTablesView.SelectedIndex != -1)
            {
                int tableID = Convert.ToInt16(cbbTablesView.SelectedItem.ToString().Substring(5));
                cleanTableData();
                loadTableData(tableID);
            }
        }

        private void openOffPerspective()
        {
            gridInitial.Visibility = Visibility.Visible;
            gridOptions.Visibility = Visibility.Hidden;
            btnOOrders.IsEnabled = false;
            btnOView.IsEnabled = false;
        }

        private void openOnPerspective()
        {
            gridInitial.Visibility = Visibility.Hidden;
            gridOptions.Visibility = Visibility.Visible;
            btnOOrders.IsEnabled = true;
            btnOView.IsEnabled = true;
            openViewPerspective();
        }

        private void resetProvisionalData()
        {
            btnSeeTable.IsEnabled = false;
            txtbClientS.Text = txtbTableS.Text = "";
        }

        private void registerSubjects(SubjectRE subjectRE, SubjectLO subjectLO)
        {
            subjectRE.registerInterest(this);
            subjectLO.registerInterest(this);
        }

        public void notifyChangesInABox(int row, int column, int state)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri((string)colorBox[state], UriKind.RelativeOrAbsolute);
            bi.EndInit();
            Room[row, column].Source = bi;
        }

        public void notifyChangesInListOfOrders(List<Order> orders)
        {
            listVOrders.Items.Clear();
            listVTablesOrders.Items.Clear();
            foreach (Order order in orders)
            {
                OrderTableItem item = new OrderTableItem();
                item.OrderID = order.Id;
                item.TableID = order.TableID;
                item.Product = order.Product;
                item.Amount = order.Amount;
                item.State = orderStatus[order.Status];
                item.Date = order.Date.ToString();
                if (order.Status < 3)
                {
                    listVOrders.Items.Add(new ListViewItem());
                    ((ListViewItem)listVOrders.Items[listVOrders.Items.Count - 1]).Content = item;
                    ((ListViewItem)listVOrders.Items[listVOrders.Items.Count - 1]).Background = itemColor[order.Status];
                }
                if (cbbTablesView.SelectedIndex != -1)
                    if (Convert.ToInt16(cbbTablesView.SelectedItem.ToString().Substring(5)) == order.TableID)
                    {
                        listVTablesOrders.Items.Add(new ListViewItem());
                        ((ListViewItem)listVTablesOrders.Items[listVTablesOrders.Items.Count - 1]).Content = 
                            new OrderTableItem(item.OrderID, item.TableID, item.Product, item.Amount, item.State, item.Date);
                        ((ListViewItem)listVTablesOrders.Items[listVTablesOrders.Items.Count - 1]).Background = itemColor[order.Status];
                    }
            }
        }
    }

    public class OrderTableItem : ListViewItem
    {
        private int orderID, amount, tableID;

        public int OrderID
        {
            get { return orderID; }
            set { orderID = value; }
        }

        public int Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        public int TableID
        {
            get { return tableID; }
            set { tableID = value; }
        }

        private string product, state, date;

        public string Product
        {
            get { return product; }
            set { product = value; }
        }

        public string State
        {
            get { return state; }
            set { state = value; }
        }

        public string Date
        {
            get { return date; }
            set { date = value; }
        }

        public OrderTableItem() { }

        public OrderTableItem(int orderID, int tableID, string product, int amount,
            string state, string date)
        {
            OrderID = orderID;
            TableID = tableID;
            Product = product;
            Amount = amount;
            State = state;
            Date = date;
        }
    }
}
