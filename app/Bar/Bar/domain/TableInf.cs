using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bar.domain
{
    // 'TableInf' define el estado actual de una mesa
    public class TableInf
    {
        /* Atributos del objeto */
        private int id, capacity, status, guests;
        // Identificador
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        // Capacidad total
        public int Capacity
        {
            get { return capacity; }
            set { capacity = value; }
        }
        // Estado: (-1) Vacía, (0) Ocupada, (1) Esperando pedido, (2) Servida, (3) Cobrada
        public int Status
        {
            get { return status; }
            set { status = value; }
        }
        // Número de comensales que la ocupan
        public int Guests
        {
            get { return guests; }
            set { guests = value; }
        }

        // Casillas que ocupa en la plantilla
        private List<int[]> place;
        public List<int[]> Place
        {
            get { return place; }
            set { place = value; }
        }

        // DNI del cliente que la ocupa
        private string client;
        internal string Client
        {
            get { return client; }
            set { client = value; }
        }

        /* Métodos constructores */
        public TableInf()
        {
            Status = -1;
            Place = new List<int[]>();
        }

        public TableInf(int id)
        {
            Id = id;
            Status = -1;
            Place = new List<int[]>();
        }

        public TableInf(int id, int capacity)
        {
            Id = id;
            Capacity = capacity;
            Status = -1;
            Client = "";
            Guests = 0;
            Place = new List<int[]>();
        }

        // Dos mesas son iguales si tienen el mismo identificador
        public override bool Equals(Object o)
        {
            if (o == null) return false;
            if (o.GetType() != typeof(TableInf)) return false;
            TableInf t = (TableInf)o;
            return id.Equals(t.Id);
        }
    }
}
