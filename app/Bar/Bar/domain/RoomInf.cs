using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bar.domain
{
    // 'RoomInf' resume las características de una plantilla
    public class RoomInf
    {
        /* Atributos del objeto */
        // Nombre de la plantilla
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private int width, height, tables, capacity;
        // Anchura (número de columnas)
        public int Width
        {
            get { return width; }
            set { width = value; }
        }
        // Altura (número de filas)
        public int Height
        {
            get { return height; }
            set { height = value; }
        }
        // Número de mesas
        public int Tables
        {
            get { return tables; }
            set { tables = value; }
        }
        // Capacidad total del salón
        public int Capacity
        {
            get { return capacity; }
            set { capacity = value; }
        }

        // Método constructor
        public RoomInf() { }
    }
}
