using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bar.domain
{
    class Client
    {
        private string dni, name, surname;

        public string Dni
        {
            get { return dni; }
            set { dni = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Surname
        {
            get { return surname; }
            set { surname = value; }
        }

        private int appearances;

        public int Appearances
        {
            get { return appearances; }
            set { appearances = value; }
        }

        public Client() { }

        public Client(string dni, string name, string surname, int appearances)
        {
            Dni = dni;
            Name = name;
            Surname = surname;
            Appearances = appearances;
        }

        public override bool Equals(Object o)
        {
            if (o == null) return false;
            if (o.GetType() != typeof(Client)) return false;
            Client c = (Client)o;
            return dni.Equals(c.Dni);
        }
    }
}
