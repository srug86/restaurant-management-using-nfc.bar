using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bar.domain
{
    // 'HOrder' define las características de un pedido histórico
    class HOrder
    {
        /* Atributos del objeto */
        // Cantidad de productos del mismo tipo
        private int amount;
        public int Amount
        {
            get { return amount; }
            set { amount = value; }
        }

        private string client, product;
        // Cliente solicitante
        public string Client
        {
          get { return client; }
          set { client = value; }
        }
        // Producto solicitado
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

        // Método constructor
        public HOrder() { }
    }
}
