using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bar.domain
{
    public class Order
    {
        private int id, tableID, amount, status;

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

        public int Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        public int Status
        {
            get { return status; }
            set { status = value; }
        }

        private string product;

        public string Product
        {
            get { return product; }
            set { product = value; }
        }

        private DateTime date;

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        public Order() { }

        public Order(int id)
        {
            Id = id;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(Order)) return false;
            Order o = (Order)obj;
            return id == o.Id;
        }
    }
}
