using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bar.domain
{
    class RoomDef
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private int height, width;

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        private List<int[]> receiver, bar;

        public List<int[]> Receiver
        {
            get { return receiver; }
            set { receiver = value; }
        }

        public List<int[]> Bar
        {
            get { return bar; }
            set { bar = value; }
        }

        private List<Table> tables;

        internal List<Table> Tables
        {
            get { return tables; }
            set { tables = value; }
        }

        public RoomDef() { }

        public int getNTables()
        {
            return tables.Count;
        }

        public int getTCapacity()
        {
            int total = 0;
            foreach (Table table in tables)
                total += table.Capacity;
            return total;
        }
    }
}
