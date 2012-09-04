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

        /* Implementación de un 'Singleton' para esta clase */
        static AdapterWebServices() { }

        AdapterWebServices() { }

        public static AdapterWebServices Instance
        {
            get
            {
                return instance;
            }
        }

        // Devuelve XML con las plantillas del restaurante
        public string sendMeRooms()
        {
            return proxy.getRooms();
        }

        // Devuelve datos resumen de la plantilla actual
        public string sendMeCurrentRoom()
        {
            return proxy.getCurrentRoom();
        }

        // Devuelve XML con los datos de la plantilla seleccionada
        public string sendMeRoom(string name, bool save)
        {
            return proxy.getRoom(name, save);
        }

        // Devuelve XML con el estado actualizado de las mesas
        public string sendMeTablesStatus()
        {
            return proxy.getTablesStatus();
        }

        // Devuelve el identificador de la mesa donde está el cliente 'dni'
        public int sendMeTable(string dni)
        {
            return proxy.getTableID(dni);
        }

        // Devuelve XML con el estado de la mesa 'tableID'
        public string sendMeTableStatus(int tableID)
        {
            return proxy.getTableStatus(tableID);
        }

        // Envía XML con la lista de productos a la BD
        public void sendProductList(string products)
        {
            proxy.saveProductsList(products);
        }

        // Devuelve XML con la lista de productos del restaurante
        public string sendMeProducts(bool nonVisible)
        {
            return proxy.getProducts(nonVisible);
        }

        // Devuelve XML con la lista de pedidos históricos
        public string sendMeHOrders(int amount, bool ascending)
        {
            return proxy.getHOrders(amount, ascending);
        }

        // Devuelve XML con el estado de los pedidos de la jornada actual
        public string sendMeOrdersStatus()
        {
            return proxy.getOrdersStatus();
        }

        // Envía XML con un nuevo pedido a la BD y devuelve el estado resultante
        public string sendNewOrder(string order)
        {
            return proxy.addNewOrder(order);
        }

        // Envía cambio de estado para el pedido 'orderID' y devuelve el estado resultante
        public string sendChangeOrderStatus(int orderID, int status)
        {
            return proxy.setOrderStatus(orderID, status);
        }

        // Envía cambio de número de productos para el pedido 'orderID'
        public void sendChangeOrderAmount(int orderID, int amount)
        {
            proxy.setProductAmount(orderID, amount);
        }

        // Envía cambio de mesa para el pedido 'orderID' y devuelve el estado resultante
        public string sendChangeOrderTable(int orderID, int tableID)
        {
            return proxy.setOrderTable(orderID, tableID);
        }

        // Devuelve XML con el histórico de facturas
        public string sendMeBills(int amount, bool ascending)
        {
            return proxy.getBills(amount, ascending);
        }

        // Devuelve XML con la descripción de una factura
        public string sendMeBill(int billID)
        {
            return proxy.getStaticBill(billID);
        }

        // Devuelve XML con la descripción de una factura (simplificada, si _short = 'true')
        public string sendMeBill(int tableID, bool _short)
        {
            return proxy.getBill(tableID, _short);
        }

        // Envía la confirmación de un pago y devuelve el estado resultante
        public string sendBillPayment(int tableID, int type)
        {
            return proxy.payBill(tableID, type);
        }

        // Envía solicitud de iniciar nueva jornada con la plantilla 'room'
        public void sendResetJourney(string room)
        {
            proxy.resetJourney(room);
        }
    }
}