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
                    //sw.Write(clientData.Substring(2));
                    //sw.Write("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Bill><Company>MobiCarta</Company><Date>09/06/2012 0:00:00</Date><Table>5</Table><Orders><Order name=\"Macarrones con tomate\" amount=\"2\" price=\"10\" discount=\"0\" total=\"20\"/><Order name=\"Sepia a la plancha\" amount=\"1\" price=\"13,5\" discount=\"0\" total=\"13,5\"/></Orders><PriceSummary subtotal=\"33,5\" discount=\"0\" taxBase=\"33,5\" IVA=\"18\" quote=\"6,03\" total=\"39,53\"/></Bill>");
                    //sw.Write("<Bill><PriceSummary subtotal=\"33,5\" discount=\"0\" taxBase=\"33,5\" IVA=\"18\" quote=\"6,03\" total=\"39,53\"/></Bill>");
                    //sw.Write("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Bill><Orders><Order name=\"Macarrones con tomate\" amount=\"2\" price=\"10\" discount=\"0\" total=\"20\"/></Orders></Bill>");
                    //sw.Write(Convert.ToString("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Bill><Company>MobiCarta</Company><Date>09/06/2012 0:00:00</Date><Table>5</Table><PriceSummary><Subtotal>33</Subtotal><Discount>0</Discount><TaxBase>33</TaxBase><IVA>18</IVA><Quote>6</Quote><Total>39</Total></PriceSummary></Bill>"));
                    sw.Write(Convert.ToString("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Bill><Company>MobiCarta</Company><Date>09/06/2012 0:00:00</Date><Table>5</Table><Orders><Order><Name>Macarrones con tomate</Name><Amount>2</Amount><Price>10</Price><Discount>0</Discount><Total>20</Total></Order><Order><Name>Sepia a la plancha</Name><Amount>1</Amount><Price>13</Price><Discount>0</Discount><Total>13</Total></Order></Orders><PriceSummary><Subtotal>33</Subtotal><Discount>0</Discount><TaxBase>33</TaxBase><IVA>18</IVA><Quote>6</Quote><Total>39</Total></PriceSummary></Bill>"));
                    //sw.Write(Convert.ToString(reply));
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
