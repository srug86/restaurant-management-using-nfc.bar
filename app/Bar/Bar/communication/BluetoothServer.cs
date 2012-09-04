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
        // Identificador único del servicio publicado por el servidor Bluetooth
        Guid service = new Guid("888794c2-65ce-4de1-aa15-74a11342bc64");

        private bool exit;

        static readonly BluetoothServer instance = new BluetoothServer();

        /* Implementación de un 'Singleton' para esta clase */
        static BluetoothServer() { }

        BluetoothServer() { }

        public static BluetoothServer Instance
        {
            get
            {
                return instance;
            }
        }

        // Inicializa el servidor Bluetooth
        public void initBluetooth()
        {
            exit = false;

            BluetoothRadio br = BluetoothRadio.PrimaryRadio;    // Radio Bluetooth de tipo Primario
            br.Mode = RadioMode.Discoverable;                   // Radio Bluetooth visible a los clientes

            btListener = new BluetoothListener(service);
            btListener.Start();

            Thread th = new Thread(new ThreadStart(this.runBluetooth));
            th.Start();
        }

        // Hilo que mantiene el servicio Bluetooth
        public void runBluetooth()
        {
            do
            {
                try
                {
                    BluetoothClient client = btListener.AcceptBluetoothClient();    // Acepta conexión con un cliente
                    StreamReader sr = new StreamReader(client.GetStream(), Encoding.UTF8);
                    string clientData = "";
                    while (true)
                    {
                        string line = sr.ReadLine();
                        clientData += line;
                        if (line.Equals("</ClientOrder>")) break;   // Recibe XML con los datos del pedido del cliente
                    }
                    manager = JourneyManager.Instance;
                    string reply = manager.OrdersManager.manageNFCOrder(clientData.Substring(2));
                    StreamWriter sw = new StreamWriter(client.GetStream(), Encoding.ASCII);
                    sw.Write(Convert.ToString(reply));  // Envío de la respuesta calculada
                    sw.Flush();
                    sw.Close();
                    sr.Close();
                    client.Close(); // Da por finalizada la comunicación con el cliente
                }
                catch (Exception e) { }
            } while (!exit);
        }

        // Termina con el servidor Bluetooth
        public void closeBluetooth()
        {
            exit = true;
            btListener.Stop();
        }
    }
}
