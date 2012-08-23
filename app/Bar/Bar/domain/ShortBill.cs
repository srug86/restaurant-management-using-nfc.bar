using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bar.domain
{
    public class ShortBill
    {
        private int id, tableID, paid;
        
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public int TableID
        {
            get { return tableID; }
            set { tableID = value; }
        }

        public int Paid
        {
            get { return paid; }
            set { paid = value; }
        }

        private string client;

        public string Client
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

        private double total;

        public double Total
        {
            get { return total; }
            set { total = value; }
        }

        public ShortBill()
        {
            TableID = Paid = 0;
            Client = "";
            Date = DateTime.Now;
            Total = 0;
        }
    }
}
