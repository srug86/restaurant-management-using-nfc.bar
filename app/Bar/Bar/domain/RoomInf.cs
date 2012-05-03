using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bar.domain
{
    public class RoomInf
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private int width, height, tables, capacity;

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public int Tables
        {
            get { return tables; }
            set { tables = value; }
        }

        public int Capacity
        {
            get { return capacity; }
            set { capacity = value; }
        }

        public RoomInf() { }
    }
}
