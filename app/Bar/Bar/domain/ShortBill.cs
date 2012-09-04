using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bar.domain
{
    // 'ShortBill' resume el contenido de una factura
    public class ShortBill
    {
        /* Atributos del objeto */
        private int id, tableID, paid;
        // Identificador
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        // Mesa cobrada
        public int TableID
        {
            get { return tableID; }
            set { tableID = value; }
        }
        // Método de pago: (0) No cobrada, (1) Cobro normal, (2) Cobro NFC
        public int Paid
        {
            get { return paid; }
            set { paid = value; }
        }

        // Cliente cobrado
        private string client;
        public string Client
        {
            get { return client; }
            set { client = value; }
        }

        // Fecha de facturación
        private DateTime date;
        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        // Importe total
        private double total;
        public double Total
        {
            get { return total; }
            set { total = value; }
        }

        // Método constructor
        public ShortBill()
        {
            TableID = Paid = 0;
            Client = "";
            Date = DateTime.Now;
            Total = 0;
        }
    }
}
