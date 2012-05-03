using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bar.domain
{
    class Bill
    {
        private string billID;

        public string BillID
        {
            get { return billID; }
            set { billID = value; }
        }

        private int tableID;

        public int TableID
        {
            get { return tableID; }
            set { tableID = value; }
        }

        private Client client;

        internal Client Client
        {
            get { return client; }
            set { client = value; }
        }

        private DateTime date;

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        private List<Order> orders;

        internal List<Order> Orders
        {
            get { return orders; }
            set { orders = value; }
        }

        private double iva, total;

        public double IVA
        {
            get { return iva; }
            set { iva = value; }
        }

        public double Total
        {
            get { return total; }
            set { total = value; }
        }

        private bool paid;

        public bool Paid
        {
            get { return paid; }
            set { paid = value; }
        }

        public Bill() { }
    }
}
