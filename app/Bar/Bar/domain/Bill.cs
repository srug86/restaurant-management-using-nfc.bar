using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bar.domain
{
    // 'Bill' describe la información de una factura
    public class Bill
    {
        /* Atributos del objeto */
        // Información de la compañía
        private Company companyInfo;
        public Company CompanyInfo
        {
            get { return companyInfo; }
            set { companyInfo = value; }
        }

        private Address companyAddress, clientAddress;
        // Dirección de la compañía
        public Address CompanyAddress
        {
            get { return companyAddress; }
            set { companyAddress = value; }
        }
        // Dirección del cliente
        public Address ClientAddress
        {
            get { return clientAddress; }
            set { clientAddress = value; }
        }

        // Información del cliente
        private Client clientInfo;
        internal Client ClientInfo
        {
            get { return clientInfo; }
            set { clientInfo = value; }
        }

        private int id, tableID, serial, paid;
        // Identificador de la factura
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        // Mesa facturada
        public int TableID
        {
            get { return tableID; }
            set { tableID = value; }
        }
        // Número de serie de la factura
        public int Serial
        {
            get { return serial; }
            set { serial = value; }
        }
        // Método de pago: (0) No cobrada, (1) Cobro normal, (2) Cobro NFC
        public int Paid
        {
            get { return paid; }
            set { paid = value; }
        }

        // Fecha de facturación
        private DateTime date;
        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        private double iva, discount, taxBase, quote, subtotal, total;
        // IVA
        public double Iva
        {
            get { return iva; }
            set { iva = value; }
        }
        // Descuento en el precio acumulado
        public double Discount
        {
            get { return discount; }
            set { discount = value; }
        }
        // Base imponible
        public double TaxBase
        {
            get { return taxBase; }
            set { taxBase = value; }
        }
        // Cuota
        public double Quote
        {
            get { return quote; }
            set { quote = value; }
        }
        // Subtotal
        public double Subtotal
        {
            get { return subtotal; }
            set { subtotal = value; }
        }
        // Total
        public double Total
        {
            get { return total; }
            set { total = value; }
        }

        // Lista de pedidos
        private List<OrderPrice> orders;
        public List<OrderPrice> Orders
        {
            get { return orders; }
            set { orders = value; }
        }

        /* Métodos constructores */
        public Bill()
        {
            Paid = 0;
            TaxBase = 0;
            Total = Subtotal = 0;
            CompanyInfo = new Company();
            ClientInfo = new Client();
            CompanyAddress = clientAddress = new Address();
            Orders = new List<OrderPrice>();
        }

        public Bill(int billID)
        {
            Id = billID;
        }

        // Dos facturas son iguales si tienen el mismo identificador
        public override bool Equals(Object o)
        {
            if (o == null) return false;
            if (o.GetType() != typeof(Bill)) return false;
            Bill b = (Bill)o;
            return Id.Equals(b.id);
        }
    }

    /* Clase auxiliar para representar los datos de la empresa */
    public class Company
    {
        /* Atributos del objeto */
        private string name, nif, email;
        // Nombre
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        // NIF
        public string NIF
        {
            get { return nif; }
            set { nif = value; }
        }
        // Correo electrónico
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        private int phone, fax;
        // Teléfono
        public int Phone
        {
            get { return phone; }
            set { phone = value; }
        }
        // Fax
        public int Fax
        {
            get { return fax; }
            set { fax = value; }
        }

        // Método constructor
        public Company() { }
    }

    /* Clase auxiliar para representar la dirección de un domicilio o un establecimiento */
    public class Address
    {
        /* Atributos del objeto */
        private string street, number, town, state;
        // Calle
        public string Street
        {
            get { return street; }
            set { street = value; }
        }
        // Número
        public string Number
        {
            get { return number; }
            set { number = value; }
        }
        // Localidad
        public string Town
        {
            get { return town; }
            set { town = value; }
        }
        // Provincia
        public string State
        {
            get { return state; }
            set { state = value; }
        }

        // Código postal
        private int zipCode;
        public int ZipCode
        {
            get { return zipCode; }
            set { zipCode = value; }
        }

        // Método constructor
        public Address() { }
    }

    /* Clase auxiliar para representar los ítems de una factura */
    public class OrderPrice : Order
    {
        /* Atributos del objeto */
        // Información del pedido
        private Order order;
        internal Order Order
        {
            get { return order; }
            set { order = value; }
        }

        private double price, discount, total;
        // Precio de cada producto
        public double Price
        {
            get { return price; }
            set { price = value; }
        }
        // Descuentos aplicados a los productos
        public double Discount
        {
            get { return discount; }
            set { discount = value; }
        }
        // Importe total del pedido
        public double Total
        {
            get { return total; }
            set { total = value; }
        }

        // Método constructor
        public OrderPrice() {
            Order = new Order();
        }
    }
}