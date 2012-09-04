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
using System.Windows.Threading;

namespace Bar.presentation
{
    /// <summary>
    /// Lógica de interacción para JourneyManagerWin.xaml
    /// </summary>
    public partial class JourneyManagerWin : Window, ObserverRE
    {
        private JourneyManager manager = JourneyManager.Instance;

        /* Atributos de la clase */
        // Nombre de la plantilla
        private string roomName;
        public string RoomName
        {
            get { return roomName; }
            set { roomName = value; }
        }

        private int roomHeight, roomWidth;
        // Número de filas de la plantilla
        public int RoomHeight
        {
            get { return roomHeight; }
            set { roomHeight = value; }
        }
        // Número de columnas de la plantilla
        public int RoomWidth
        {
            get { return roomWidth; }
            set { roomWidth = value; }
        }

        // Matriz de casillas de la plantilla
        private Image[,] room;
        public Image[,] Room
        {
            get { return room; }
            set { room = value; }
        }

        private Grid[] gridsOnMode;

        // 'Logger'
        private FlowDocument fdEvents;

        // Diccionario que relaciona el estado de una mesa con el color que la representa
        private Dictionary<int, string> colorBox = new Dictionary<int, string> {
            {-1, "/Bar;component/Images/black.png"},
            {0, "/Bar;component/Images/green.png"},
            {1, "/Bar;component/Images/white.png"},
            {2, "/Bar;component/Images/dgreen.png"},
            {3, "/Bar;component/Images/orange.png"},
            {4, "/Bar;component/Images/red.png"},
            {5, "/Bar;component/Images/yellow.png"},
            {6, "/Bar;component/Images/pgreen.png"},
        };

        // Diccionario que relaciona el número de estado de una mesa con el mensaje que se muestra
        private Dictionary<int, string> tableStatus = new Dictionary<int, string> {
            {-1, "Vacía"}, {0, "Ocupada"}, {1, "Esperando pedido"}, {2, "Servida"}, {3, "Cobrada"},};

        // Diccionario que relaciona el número de estado de un pedido con el mensaje que se muestra
        private Dictionary<int, string> orderStatus = new Dictionary<int, string> {
            {-1, "Detenido"}, {0, "No atendido"}, {1, "Atendido"}, {2, "Servido"}, {3, "Pagado"},};

        // Diccionario que relaciona el estado de un pedido con el color que lo representa
        private Dictionary<int, Brush> itemColor = new Dictionary<int, Brush> {
            {-1, Brushes.IndianRed}, {0, Brushes.WhiteSmoke}, {1, Brushes.Yellow},
            {2, Brushes.LightGreen}, {3, Brushes.Orange},};

        // Método constructor
        public JourneyManagerWin()
        {
            InitializeComponent();
            initGridsOnMode();
            openOffPerspective();
        }

        // Generación de una matriz con los 'grids' de los modos de una jornada
        private void initGridsOnMode()
        {
            gridsOnMode = new Grid[] { gridORoomView, gridOOrdersList, gridOSeeTable };
            foreach (Grid g in gridsOnMode)
                g.Visibility = Visibility.Hidden;
        }

        // Muestra el 'grid' de un modo concreto
        private void showOnModeGrid(Grid grid)
        {
            foreach (Grid g in gridsOnMode)
            {
                if (g.Equals(grid)) g.Visibility = Visibility.Visible;
                else g.Visibility = Visibility.Hidden;
            }
        }

        // 'Setea' los 'grids' para el modo 'OFF' del "gestor de mesas"
        private void openOffPerspective()
        {
            gridInitial.Visibility = Visibility.Visible;
            gridOptions.Visibility = Visibility.Hidden;
            btnOOrders.IsEnabled = false;
            btnOView.IsEnabled = false;
        }

        // 'Setea' los 'grids' para el modo 'ON' del "gestor de mesas"
        private void openOnPerspective()
        {
            gridInitial.Visibility = Visibility.Hidden;
            gridOptions.Visibility = Visibility.Visible;
            btnOOrders.IsEnabled = true;
            btnOView.IsEnabled = true;
            openViewPerspective();
        }

        // Carga una nueva jornada o una jornada existente
        public void loadSelectedRoom(string name, bool reset)
        {
            manager.createRoomManager();
            manager.RoomManager.loadRoom(name, reset);  // Carga la plantilla seleccionada
            generateEmptyRoom(manager.RoomManager.Room.Name,
                manager.RoomManager.Room.Height, manager.RoomManager.Room.Width);
            registerSubjects(manager.RoomManager);      // Se registra como 'Observer'
            manager.OrdersManager.setGuiReference(this);
            manager.RoomManager.locateObjects();        // Carga los objetos en la plantilla
            if (reset) manager.resetCurrentJourney(name);   // 'Resetea' la información de la jornada anterior
            else
            {
                manager.RoomManager.updateTables();         // o carga el estado de las mesas
                manager.OrdersManager.updateOrders();       // y de los pedidos de la jornada anterior
            }
            manager.ProductsManager.updateProducts(true);   // Carga los productos disponibles
            manager.initBluetoothServer();                  // Inicializa el servidor Bluetooth
            this.delegateToShowBarEvent(reset ? 0 : 1, new Order(), 0);
            openOnPerspective();
        }

        // Genera una plantilla de restaurante vacía
        private void generateEmptyRoom(string name, int height, int width)
        {
            RoomName = name;
            RoomHeight = height;
            RoomWidth = width;
            Room = new Image[RoomHeight, RoomWidth];
            if (uGridStatus.Children.Count > 0) uGridStatus.Children.Clear();
            uGridStatus.Rows = RoomHeight;
            uGridStatus.Columns = RoomWidth;
            fdEvents = new FlowDocument();
            fdEvents.LineHeight = 0.2;
            rtbEvents.Document = fdEvents;
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

        // Abrir la perspectiva del modo "Ver pedidos"
        private void openOrdersPerspective()
        {
            manager.OrdersManager.updateOrders();
            showOnModeGrid(gridOOrdersList);
        }

        // Abrir la perspectiva del modo "Ver mesa"
        private void openTablesInfoPerspective()
        {
            if (Convert.ToInt16(txtbTableS.Text) > 0)
            {
                cleanTableData();
                loadTableData(Convert.ToInt16(txtbTableS.Text));
                showOnModeGrid(gridOSeeTable);
            }
        }

        // Abrir la perspectiva del modo "Vista general"
        private void openViewPerspective()
        {
            resetProvisionalData();
            manager.RoomManager.updateTables();
            showOnModeGrid(gridORoomView);
        }

        // 'Resetea' la información residual de la información de una mesa que ya no se muestra
        private void cleanTableData()
        {
            cbbTablesView.Items.Clear();
            txtbDNI.Text = txtbName.Text = txtbSurname.Text = txtbAppearances.Text =
                txtbTableID.Text = txtbTableOccupation.Text = txtbTableStatus.Text = "";
            btnCheckIn.Content = "Facturar";
            btnCheckIn.IsEnabled = false;
            listVTablesOrders.Items.Clear();
        }

        // Carga la información detallada de una mesa
        private void loadTableData(int tableID)
        {
            List<int> tables = manager.RoomManager.getCandidateTables();
            int i = 0;
            foreach (int table in tables)   // Carga las mesas ocupadas
            {
                cbbTablesView.Items.Add("Mesa " + table);
                if (tableID == table) cbbTablesView.SelectedIndex = i;
                i++;
            }
            List<Object> info = manager.RoomManager.updateTable(tableID);   // Devuelve el estado actualizado de la mesa
            foreach (Object o in info)
            {
                if (o.GetType() == typeof(Client))  // Información del cliente que ocupa la mesa
                {
                    txtbDNI.Text = ((Client)o).Dni;
                    txtbName.Text = ((Client)o).Name;
                    txtbSurname.Text = ((Client)o).Surname;
                    txtbAppearances.Text = Convert.ToString(((Client)o).Appearances);
                }
                else if (o.GetType() == typeof(Bar.domain.TableInf))   // Información de la mesa
                {
                    txtbTableID.Text = Convert.ToString(((Bar.domain.TableInf)o).Id);
                    txtbTableStatus.Text = tableStatus[((Bar.domain.TableInf)o).Status];
                    if (((Bar.domain.TableInf)o).Status == 2)
                    {
                        btnCheckIn.IsEnabled = true;
                        btnCheckIn.Content = "Facturar";
                    }
                    else if (((Bar.domain.TableInf)o).Status == 3)
                    {
                        btnCheckIn.IsEnabled = true;
                        btnCheckIn.Content = "Ver factura";
                    }
                    txtbTableOccupation.Text = Convert.ToString(((Bar.domain.TableInf)o).Guests) +
                        "/" + Convert.ToString(((Bar.domain.TableInf)o).Capacity);
                }
                else if (o.GetType() == typeof(List<Order>))    // Lista de pedidos de la mesa
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

        // Elimina la información residual del modo "Vista general"
        private void resetProvisionalData()
        {
            btnSeeTable.IsEnabled = false;
            txtbClientS.Text = txtbTableS.Text = "";
        }

        /* Métodos para implementar el patrón 'Observer' de la GUI*/
        private void registerSubjects(SubjectRE subjectRE)
        {
            subjectRE.registerInterest(this);
        }

        // Método 'notify' que avisa del cambio de estado de una de las casillas de la plantilla
        public void notifyChangesInABox(int row, int column, int state)
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri((string)colorBox[state], UriKind.RelativeOrAbsolute);
            bi.EndInit();
            Room[row, column].Source = bi;
        }

        /* Delegados de la GUI */
        // Delegado que registra los eventos producidos en el 'logger' del modo "Vista general"
        private delegate void BarEvent(int type, Order order, int tableID);
        public void delegateToShowBarEvent(int type, Order order, int tableID)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new BarEvent(this.barEvent), type, order, tableID);
        }

        public void barEvent(int type, Order order, int tableID)
        {
            Paragraph p = new Paragraph();
            string message = "";
            switch (type)
            {
                case 0: p.Foreground = Brushes.DarkBlue;    // Inicio de jornada
                    message = "(" + DateTime.Now + ")\tSe inicia de una nueva jornada.";
                    break;
                case 1: p.Foreground = Brushes.DarkBlue;    // Inicio de jornada
                    message = "(" + DateTime.Now + ")\tSe inicia una jornada existente.";
                    break;
                case 2: p.Foreground = Brushes.Black;       // Añadido nuevo pedido
                    message = "(" + order.Date + ")\tPedido añadido:\t\t" + order.Amount +
                        " de " + order.Product + ", Mesa: " + order.TableID + ".";
                    break;
                case 3: p.Foreground = Brushes.Black;       // Eliminado pedido
                    message = "(" + order.Date + ")\tPedido eliminado:\t" + order.Amount +
                        " de " + order.Product + ", Mesa: " + order.TableID + ".";
                    break;
                case 4: p.Foreground = Brushes.Black;       // Editado pedido
                    message = "(" + order.Date + ")\tPedido editado:\t\t" + order.Amount +
                        " de " + order.Product + ", Mesa: " + order.TableID + ".";
                    break;
                case 5: p.Foreground = Brushes.DarkOrange;  // Mesa cobrada
                    message = "(" + DateTime.Now + ")\tLa mesa " + tableID + " ha sido cobrada.";
                    break;
                case 6: p.Foreground = Brushes.Black;       // Producto "No atendido"
                    message = "(" + order.Date + ")\tPedido 'no atendido':\t" + order.Amount +
                        " de " + order.Product + ", Mesa: " + order.TableID + ".";
                    break;
                case 7: p.Foreground = Brushes.Gold;        // Producto "Atendido"
                    message = "(" + order.Date + ")\tPedido 'atentido':\t" + order.Amount +
                        " de " + order.Product + ", Mesa: " + order.TableID + ".";
                    break;
                case 8: p.Foreground = Brushes.DarkGreen;   // Producto "Servido"
                    message = "(" + order.Date + ")\tPedido 'servido':\t\t" + order.Amount +
                        " de " + order.Product + ", Mesa: " + order.TableID + ".";
                    break;
                case 9: p.Foreground = Brushes.DarkRed;     // Producto "Detenido"
                    message = "(" + order.Date + ")\tPedido 'detenido':\t" + order.Amount +
                        " de " + order.Product + ", Mesa: " + order.TableID + ".";
                    break;
                default: break;
            }
            p.Inlines.Add(new Bold(new Run(message)));
            if (fdEvents.Blocks.Count > 0)
                fdEvents.Blocks.InsertBefore(fdEvents.Blocks.FirstBlock, p);
            else fdEvents.Blocks.Add(p);
        }

        // Delegado que registra los cambios en la lista de pedidos
        private delegate void OrdersList(List<Order> orders);
        public void delegateToChangeTheOrdersList(List<Order> orders)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new OrdersList(this.changeTheOrdersList), orders);
        }

        public void changeTheOrdersList(List<Order> orders)
        {
            listVOrders.Items.Clear();
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
            }
            if (cbbTablesView.SelectedIndex != -1)
            {
                int tableID = Convert.ToInt16(cbbTablesView.SelectedItem.ToString().Substring(5));
                cleanTableData();
                loadTableData(tableID);
            }
        }

        /* Lógica de control de eventos */
        // Click en el botón "Nueva jornada"
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            LoadRoomDialog roomDialog = new LoadRoomDialog(this, manager.consultingRooms(), true);
            roomDialog.Show();
        }

        // Click en el botón "Cargar jornada existente"
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadRoomDialog roomDialog = new LoadRoomDialog(this, manager.consultingCurrentRoom(), false);
            roomDialog.Show();
        }

        // Click en el botón del modo "Ver pedidos"
        private void btnOOrders_Click(object sender, RoutedEventArgs e)
        {
            openOrdersPerspective();
        }

        // Click en el botón del modo "Vista general"
        private void btnOView_Click(object sender, RoutedEventArgs e)
        {
            openViewPerspective();
        }

        // Click en el botón del modo "Ver mesa"
        private void btnSeeTable_Click(object sender, RoutedEventArgs e)
        {
            openTablesInfoPerspective();
        }

        // Click en el botón del modo "Atrás" para volver al modo "Vista general"
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            openViewPerspective();
        }

        // Click en el botón "Añadir pedido"
        private void btnAddOrder_Click(object sender, RoutedEventArgs e)
        {
            NewOrderWin newOrder = new NewOrderWin(manager.ProductsManager.Categories, manager.RoomManager.getCandidateTables());
            if (((Button)sender).Name == "btnAddOT") newOrder.selectTable(cbbTablesView.SelectedIndex);
            newOrder.Show();
        }

        // Click en el botón "Eliminar pedido"
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

        // Click en el botón "Editar pedido"
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

        // Click en el botón "No atendido" para cambiar el estado del pedido
        private void btnUnattender_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Name == "btnUnattender" && listVOrders.SelectedIndex != -1)
                manager.OrdersManager.changeOrderStatus(((OrderTableItem)((ListViewItem)listVOrders.SelectedItem).Content).OrderID, 0);
            else if (((Button)sender).Name == "btnUnattenderOT" && listVTablesOrders.SelectedIndex != -1)
                manager.OrdersManager.changeOrderStatus(((OrderTableItem)((ListViewItem)listVTablesOrders.SelectedItem).Content).OrderID, 0);
        }

        // Click en el botón "Atendido" para cambiar el estado del pedido
        private void btnAttended_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Name == "btnAttended" && listVOrders.SelectedIndex != -1)
                manager.OrdersManager.changeOrderStatus(((OrderTableItem)((ListViewItem)listVOrders.SelectedItem).Content).OrderID, 1);
            else if (((Button)sender).Name == "btnAttendedOT" && listVTablesOrders.SelectedIndex != -1)
                manager.OrdersManager.changeOrderStatus(((OrderTableItem)((ListViewItem)listVTablesOrders.SelectedItem).Content).OrderID, 1);
        }

        // Click en el botón "Servido" para cambiar el estado del pedido
        private void btnServed_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Name == "btnServed" && listVOrders.SelectedIndex != -1)
                manager.OrdersManager.changeOrderStatus(((OrderTableItem)((ListViewItem)listVOrders.SelectedItem).Content).OrderID, 2);
            else if (((Button)sender).Name == "btnServedOT" && listVTablesOrders.SelectedIndex != -1)
                manager.OrdersManager.changeOrderStatus(((OrderTableItem)((ListViewItem)listVTablesOrders.SelectedItem).Content).OrderID, 2);
        }

        // Click en el botón "Detenido" para cambiar el estado del pedido
        private void btnStopped_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Name == "btnStopped" && listVOrders.SelectedIndex != -1)
                manager.OrdersManager.changeOrderStatus(((OrderTableItem)((ListViewItem)listVOrders.SelectedItem).Content).OrderID, -1);
            else if (((Button)sender).Name == "btnStoppedOT" && listVTablesOrders.SelectedIndex != -1)
                manager.OrdersManager.changeOrderStatus(((OrderTableItem)((ListViewItem)listVTablesOrders.SelectedItem).Content).OrderID, -1);
        }

        // Click en el botón "Facturar" para facturar una mesa
        private void btnCheckIn_Click(object sender, RoutedEventArgs e)
        {
            BillDialog billDialog = new BillDialog(this, manager.BillsManager.generateBill(Convert.ToInt16(txtbTableID.Text)));
            billDialog.Show();
        }

        // Click en alguna de las casillas de la plantilla del restaurante
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

        // Click en el botón "Ver mesa"
        private void btnConsultTable_Click(object sender, RoutedEventArgs e)
        {
            if (cbbTablesView.SelectedIndex != -1)
            {
                int tableID = Convert.ToInt16(cbbTablesView.SelectedItem.ToString().Substring(5));
                cleanTableData();
                loadTableData(tableID);
            }
        }
    }

    /* Clase auxiliar para representar la información de una pedido en la lista de pedidos */
    public class OrderTableItem : ListViewItem
    {
        private int orderID, amount, tableID;
        // Identificador del pedido
        public int OrderID
        {
            get { return orderID; }
            set { orderID = value; }
        }
        // Cantidad de productos del mismo tipo
        public int Amount
        {
            get { return amount; }
            set { amount = value; }
        }
        // Identificador de la mesa solicitante
        public int TableID
        {
            get { return tableID; }
            set { tableID = value; }
        }

        private string product, state, date;
        // Producto solicitado
        public string Product
        {
            get { return product; }
            set { product = value; }
        }
        // Estado del pedido
        public string State
        {
            get { return state; }
            set { state = value; }
        }
        // Fecha de la solicitud
        public string Date
        {
            get { return date; }
            set { date = value; }
        }

        /* Métodos constructores */
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
