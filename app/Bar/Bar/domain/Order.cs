using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bar.domain
{
    // 'Order' define las características de un pedido
    public class Order
    {
        /* Atributos del objeto */
        private int id, tableID, amount, status;
        // Identificador del pedido
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        // Mesa a la que va dirigido
        public int TableID
        {
            get { return tableID; }
            set { tableID = value; }
        }
        // Cantidad de productos del mismo tipo
        public int Amount
        {
            get { return amount; }
            set { amount = value; }
        }
        // Estado del pedido: (-1) Detenido, (0) No atendido, (1) Atendido, (2) Servido
        public int Status
        {
            get { return status; }
            set { status = value; }
        }

        // Nombre del producto
        private string product;
        public string Product
        {
            get { return product; }
            set { product = value; }
        }

        // Fecha de solicitud
        private DateTime date;
        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        /* Métodos constructores */
        public Order() { }

        public Order(int id)
        {
            Id = id;
        }

        // Dos pedidos son iguales si tienen un mismo identificador
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(Order)) return false;
            Order o = (Order)obj;
            return id == o.Id;
        }
    }
}
