using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bar.domain
{
    // 'RoomDef' define una plantilla y los objetos que la ocupan
    class RoomDef
    {
        /* Atributos del objeto */
        // Nombre de la plantilla
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private int height, width;
        // Altura (número de filas)
        public int Height
        {
            get { return height; }
            set { height = value; }
        }
        // Anchura (número de columnas)
        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        private List<int[]> receiver, bar;
        // Casillas que ocupa el 'recibidor'
        public List<int[]> Receiver
        {
            get { return receiver; }
            set { receiver = value; }
        }
        // Casillas que ocupa la 'barra'
        public List<int[]> Bar
        {
            get { return bar; }
            set { bar = value; }
        }

        // Lista de mesas de la plantilla
        private List<TableInf> tables;
        internal List<TableInf> Tables
        {
            get { return tables; }
            set { tables = value; }
        }

        // Método constructor
        public RoomDef() { }

        // Calcula el número de mesas del restaurante
        public int getNTables()
        {
            return tables.Count;
        }

        // Calcula la capacidad total del salón
        public int getTCapacity()
        {
            int total = 0;
            foreach (TableInf table in tables)
                total += table.Capacity;
            return total;
        }
    }
}
