using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bar.domain;
using InTheHand.Net.Sockets;
using System.IO;
using InTheHand.Net.Bluetooth;
using System.Threading;

namespace Bar.communication
{
    class BluetoothServer
    {
        private JourneyManager manager;

        BluetoothListener btListener;
        // UID del servicio de la aplicación de la barra
        Guid service = new Guid("888794c2-65ce-4de1-aa15-74a11342bc64");

        private bool exit;

        static readonly BluetoothServer instance = new BluetoothServer();

        static BluetoothServer() { }

        BluetoothServer() { }

        public static BluetoothServer Instance
        {
            get
            {
                return instance;
            }
        }

        public void initBluetooth()
        {
            exit = false;

            BluetoothRadio br = BluetoothRadio.PrimaryRadio;
            br.Mode = RadioMode.Discoverable;

            btListener = new BluetoothListener(service);
            btListener.Start();

            Thread th = new Thread(new ThreadStart(this.runBluetooth));
            th.Start();
        }

        public void runBluetooth()
        {
            do
            {
                try
                {
                    BluetoothClient client = btListener.AcceptBluetoothClient();
                    StreamReader sr = new StreamReader(client.GetStream(), Encoding.UTF8);
                    string clientData = "";
                    while (true)
                    {
                        string line = sr.ReadLine();
                        clientData += line;
                        if (line.Equals("</ClientOrder>")) break;
                    }
                    manager = JourneyManager.Instance;
                    string reply = manager.OrdersManager.manageNFCOrder(clientData.Substring(2));
                    StreamWriter sw = new StreamWriter(client.GetStream(), Encoding.ASCII);
                    sw.Write(Convert.ToString(reply));
                    sw.Flush();
                    sw.Close();
                    sr.Close();
                    client.Close();
                }
                catch (Exception e) { }
            } while (!exit);
        }

        public void closeBluetooth()
        {
            exit = true;
            btListener.Stop();
        }
    }
}
