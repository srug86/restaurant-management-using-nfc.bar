using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bar.domain
{
    public class Bill
    {
        private Company companyInfo;

        public Company CompanyInfo
        {
            get { return companyInfo; }
            set { companyInfo = value; }
        }

        private Address companyAddress, clientAddress;

        public Address CompanyAddress
        {
            get { return companyAddress; }
            set { companyAddress = value; }
        }

        public Address ClientAddress
        {
            get { return clientAddress; }
            set { clientAddress = value; }
        }

        private Client clientInfo;

        internal Client ClientInfo
        {
            get { return clientInfo; }
            set { clientInfo = value; }
        }

        private int id, tableID, serial, paid;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public int TableID
        {
            get { return tableID; }
            set { tableID = value; }
        }

        public int Serial
        {
            get { return serial; }
            set { serial = value; }
        }

        public int Paid
        {
            get { return paid; }
            set { paid = value; }
        }

        private DateTime date;

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        private double iva, discount, taxBase, quote, total;

        public double Iva
        {
            get { return iva; }
            set { iva = value; }
        }

        public double Discount
        {
            get { return discount; }
            set { discount = value; }
        }

        public double TaxBase
        {
            get { return taxBase; }
            set { taxBase = value; }
        }

        public double Quote
        {
            get { return quote; }
            set { quote = value; }
        }

        public double Total
        {
            get { return total; }
            set { total = value; }
        }

        private List<OrderPrice> orders;

        public List<OrderPrice> Orders
        {
            get { return orders; }
            set { orders = value; }
        }

        public Bill()
        {
            Paid = 0;
            TaxBase = 0;
            Total = 0;
            CompanyInfo = new Company();
            ClientInfo = new Client();
            CompanyAddress = clientAddress = new Address();
            Orders = new List<OrderPrice>();
        }

        public Bill(int billID)
        {
            Id = billID;
        }

        public override bool Equals(Object o)
        {
            if (o == null) return false;
            if (o.GetType() != typeof(Bill)) return false;
            Bill b = (Bill)o;
            return Id.Equals(b.id);
        }
    }

    public class Company
    {
        private string name, nif, email;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string NIF
        {
            get { return nif; }
            set { nif = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        private int phone, fax;

        public int Phone
        {
            get { return phone; }
            set { phone = value; }
        }

        public int Fax
        {
            get { return fax; }
            set { fax = value; }
        }

        public Company() { }
    }

    public class Address
    {
        private string street, number, town, state;

        public string Street
        {
            get { return street; }
            set { street = value; }
        }

        public string Number
        {
            get { return number; }
            set { number = value; }
        }

        public string Town
        {
            get { return town; }
            set { town = value; }
        }

        public string State
        {
            get { return state; }
            set { state = value; }
        }

        private int zipCode;

        public int ZipCode
        {
            get { return zipCode; }
            set { zipCode = value; }
        }

        public Address() { }
    }

    public class OrderPrice : Order
    {
        private Order order;

        internal Order Order
        {
            get { return order; }
            set { order = value; }
        }

        private double price, iva, discount, total;

        public double Price
        {
            get { return price; }
            set { price = value; }
        }

        public double Iva
        {
            get { return iva; }
            set { iva = value; }
        }

        public double Discount
        {
            get { return discount; }
            set { discount = value; }
        }

        public double Total
        {
            get { return total; }
            set { total = value; }
        }

        public OrderPrice() {
            Order = new Order();
        }
    }
}