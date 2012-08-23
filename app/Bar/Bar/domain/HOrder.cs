using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bar.domain
{
    class HOrder
    {
        private int amount;

        public int Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        private string client, product;

        public string Client
        {
          get { return client; }
          set { client = value; }
        }

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

        public HOrder() { }
    }
}
