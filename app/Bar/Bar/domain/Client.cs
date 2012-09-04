using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bar.domain
{
    // 'Client' contiene la información de un cliente
    class Client
    {
        /* Atributos del objeto */
        private string dni, name, surname;
        // DNI
        public string Dni
        {
            get { return dni; }
            set { dni = value; }
        }
        // Nombre
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        // Apellido(s)
        public string Surname
        {
            get { return surname; }
            set { surname = value; }
        }

        // Número de apariciones del cliente
        private int appearances;
        public int Appearances
        {
            get { return appearances; }
            set { appearances = value; }
        }

        /* Métodos constructores */
        public Client() {
            Name = Surname = "";
            Appearances = 0;
        }

        public Client(string dni, string name, string surname, int appearances)
        {
            Dni = dni;
            Name = name;
            Surname = surname;
            Appearances = appearances;
        }

        // Dos clientes son iguales si tienen el mismo DNI
        public override bool Equals(Object o)
        {
            if (o == null) return false;
            if (o.GetType() != typeof(Client)) return false;
            Client c = (Client)o;
            return dni.Equals(c.Dni);
        }
    }
}
