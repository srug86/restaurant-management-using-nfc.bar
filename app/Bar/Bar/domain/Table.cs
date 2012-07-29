using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bar.domain
{
    public class Table
    {
        private int id, capacity, status, guests;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public int Capacity
        {
            get { return capacity; }
            set { capacity = value; }
        }

        public int Status
        {
            get { return status; }
            set { status = value; }
        }

        public int Guests
        {
            get { return guests; }
            set { guests = value; }
        }

        private List<int[]> place;

        public List<int[]> Place
        {
            get { return place; }
            set { place = value; }
        }

        private string client;

        internal string Client
        {
            get { return client; }
            set { client = value; }
        }

        public Table()
        {
            Status = -1;
            Place = new List<int[]>();
        }

        public Table(int id)
        {
            Id = id;
            Status = -1;
            Place = new List<int[]>();
        }

        public Table(int id, int capacity)
        {
            Id = id;
            Capacity = capacity;
            Status = -1;
            Client = "";
            Guests = 0;
            Place = new List<int[]>();
        }

        public override bool Equals(Object o)
        {
            if (o == null) return false;
            if (o.GetType() != typeof(Table)) return false;
            Table t = (Table)o;
            return id.Equals(t.Id);
        }
    }
}
