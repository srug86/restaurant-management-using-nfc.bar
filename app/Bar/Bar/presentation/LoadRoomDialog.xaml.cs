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
    /// Lógica de interacción para LoadRoomDialog.xaml
    /// </summary>
    public partial class LoadRoomDialog : Window
    {
        private Object dad;

        private List<RoomInf> list;

        private bool resetJourney;

        public LoadRoomDialog(Object dad, List<RoomInf> list, bool resetJourney)
        {
            this.dad = dad;
            this.list = list;
            this.resetJourney = resetJourney;
            InitializeComponent();
            if (!resetJourney)
            {
                wLoadRoom.Title = "MobiCarta - Cargar una jornada existente.";
                if (list.Count == 0)
                {
                    lblInstructions.Content = "No hay ninguna jornada cargada en el servidor.";
                    lblLoadMessage.Content = "";
                }
                else
                {
                    lblInstructions.Content = "Pulse 'Cargar' para iniciar la jornada con el restaurante actual.";
                    lblLoadMessage.Content = "* Se conservarán los datos almacenados durante la jornada anterior.";
                }
            }
            showRooms();
        }

        private void listVRooms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnLoad.IsEnabled = IsEnabled;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            JourneyManagerWin win = (JourneyManagerWin)this.dad;
            win.loadSelectedRoom(((RoomItem)listVRooms.SelectedValue).Name, resetJourney);
            this.Visibility = Visibility.Hidden;
        }

        public void showRooms()
        {
            List<RoomItem> collection = new List<RoomItem>();
            foreach (RoomInf room in list)
            {
                collection.Add(new RoomItem()
                {
                    Name = room.Name,
                    Size = room.Height + "x" + room.Width,
                    Tables = Convert.ToString(room.Tables),
                    Capacity = Convert.ToString(room.Capacity)
                });
            }
            listVRooms.ItemsSource = collection;
        }
    }

    public class RoomItem
    {
        private string name, size, tables, capacity;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Size
        {
            get { return size; }
            set { size = value; }
        }

        public string Tables
        {
            get { return tables; }
            set { tables = value; }
        }

        public string Capacity
        {
            get { return capacity; }
            set { capacity = value; }
        }
    }
}
