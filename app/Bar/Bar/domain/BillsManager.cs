using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bar.communication;
using System.Xml;

namespace Bar.domain
{
    class BillsManager
    {
        private JourneyManager manager = JourneyManager.Instance;

        private AdapterWebServices adapter = AdapterWebServices.Instance;

        static readonly BillsManager instance = new BillsManager();

        static BillsManager() { }

        BillsManager() { }

        public static BillsManager Instance
        {
            get
            {
                return instance;
            }
        }

        public Bill getBill(int billID)
        {
            return xmlBillDecoder(adapter.sendMeBill(billID));
        }

        public List<ShortBill> getBills(int amount, bool ascending)
        {
            return xmlBillsDecoder(adapter.sendMeBills(amount, ascending));
        }

        public Bill generateBill(int tableID)
        {
            return xmlBillDecoder(adapter.sendMeBill(tableID, false));
        }

        public void payBill(int billID, int table, int type)
        {
            manager.RoomManager.xmlTablesStatus(adapter.sendBillPayment(billID, type));
            manager.OrdersManager.markOrdersAsPaid(table);
        }

        private List<ShortBill> xmlBillsDecoder(string sXml)
        {
            List<ShortBill> lob = new List<ShortBill>();
            if (!sXml.Equals(""))
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(sXml);
                XmlNodeList bills = xml.GetElementsByTagName("Bills");
                XmlNodeList bList = ((XmlElement)bills[0]).GetElementsByTagName("Bill");
                foreach (XmlElement bill in bList)
                {
                    ShortBill sb = new ShortBill();
                    sb.Id = Convert.ToInt32(bill.GetAttribute("id"));
                    XmlNodeList table = ((XmlElement)bill).GetElementsByTagName("Table");
                    sb.TableID = Convert.ToInt32(table[0].InnerText);
                    XmlNodeList client = ((XmlElement)bill).GetElementsByTagName("Client");
                    sb.Client = Convert.ToString(client[0].InnerText).Trim();
                    XmlNodeList date = ((XmlElement)bill).GetElementsByTagName("Date");
                    sb.Date = Convert.ToDateTime(date[0].InnerText);
                    XmlNodeList total = ((XmlElement)bill).GetElementsByTagName("Total");
                    sb.Total = Convert.ToDouble(total[0].InnerText);
                    XmlNodeList paid = ((XmlElement)bill).GetElementsByTagName("Paid");
                    sb.Paid = Convert.ToInt32(paid[0].InnerText);
                    lob.Add(sb);
                }
            }
            return lob;
        }

        private Bill xmlBillDecoder(string sXml)
        {
            Bill bill = new Bill();
            if (sXml != "")
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(sXml);
                XmlNodeList b = xml.GetElementsByTagName("Bill");
                XmlNodeList company = ((XmlElement)b[0]).GetElementsByTagName("Company");
                bill.CompanyInfo.Name = ((XmlElement)company[0]).GetAttribute("name");
                XmlNodeList nif = ((XmlElement)company[0]).GetElementsByTagName("NIF");
                bill.CompanyInfo.NIF = nif[0].InnerText;
                XmlNodeList address = ((XmlElement)company[0]).GetElementsByTagName("Address");
                xmlAddressDecoder((XmlElement)address[0], bill.CompanyAddress);
                XmlNodeList contact = ((XmlElement)company[0]).GetElementsByTagName("Contact");
                XmlNodeList phone = ((XmlElement)contact[0]).GetElementsByTagName("Phone");
                bill.CompanyInfo.Phone = Convert.ToInt32(phone[0].InnerText);
                XmlNodeList fax = ((XmlElement)contact[0]).GetElementsByTagName("Fax");
                bill.CompanyInfo.Fax = Convert.ToInt32(fax[0].InnerText);
                XmlNodeList mail = ((XmlElement)contact[0]).GetElementsByTagName("Email");
                bill.CompanyInfo.Email = mail[0].InnerText;
                XmlNodeList client = ((XmlElement)b[0]).GetElementsByTagName("Client");
                XmlNodeList dni = ((XmlElement)client[0]).GetElementsByTagName("DNI");
                bill.ClientInfo.Dni = dni[0].InnerText;
                XmlNodeList name = ((XmlElement)client[0]).GetElementsByTagName("Name");
                bill.ClientInfo.Name = name[0].InnerText;
                XmlNodeList surname = ((XmlElement)client[0]).GetElementsByTagName("Surname");
                bill.ClientInfo.Surname = surname[0].InnerText;
                XmlNodeList cAddress = ((XmlElement)client[0]).GetElementsByTagName("Address");
                xmlAddressDecoder((XmlElement)cAddress[0], bill.ClientAddress);
                XmlNodeList info = ((XmlElement)b[0]).GetElementsByTagName("Info");
                XmlNodeList number = ((XmlElement)info[0]).GetElementsByTagName("Number");
                bill.Id = Convert.ToInt32(number[0].InnerText);
                XmlNodeList serial = ((XmlElement)info[0]).GetElementsByTagName("Serial");
                bill.Serial = Convert.ToInt16(serial[0].InnerText);
                XmlNodeList date = ((XmlElement)info[0]).GetElementsByTagName("Date");
                bill.Date = Convert.ToDateTime(date[0].InnerText);
                XmlNodeList table = ((XmlElement)info[0]).GetElementsByTagName("Table");
                bill.TableID = Convert.ToInt16(table[0].InnerText);
                XmlNodeList orders = ((XmlElement)b[0]).GetElementsByTagName("Orders");
                XmlNodeList oList = ((XmlElement)orders[0]).GetElementsByTagName("Order");
                foreach (XmlElement order in oList)
                {
                    OrderPrice oPrice = new OrderPrice();
                    oPrice.Id = Convert.ToInt16(order.GetAttribute("id"));
                    XmlNodeList product = ((XmlElement)order).GetElementsByTagName("Product");
                    oPrice.Product = product[0].InnerText;
                    XmlNodeList amount = ((XmlElement)order).GetElementsByTagName("Amount");
                    oPrice.Amount = Convert.ToInt16(amount[0].InnerText);
                    XmlNodeList price = ((XmlElement)order).GetElementsByTagName("Price");
                    oPrice.Price = Convert.ToDouble(price[0].InnerText);
                    XmlNodeList discount = ((XmlElement)order).GetElementsByTagName("Discount");
                    oPrice.Discount = Convert.ToDouble(discount[0].InnerText);
                    XmlNodeList total = ((XmlElement)order).GetElementsByTagName("Total");
                    oPrice.Total = Convert.ToDouble(total[0].InnerText);
                    bill.Orders.Add(oPrice);
                }
                XmlNodeList amounts = ((XmlElement)b[0]).GetElementsByTagName("PriceSummary");
                XmlNodeList subtotal = ((XmlElement)amounts[0]).GetElementsByTagName("Subtotal");
                bill.Subtotal = Convert.ToDouble(subtotal[0].InnerText);
                XmlNodeList discountT = ((XmlElement)amounts[0]).GetElementsByTagName("Discount");
                bill.Discount = Convert.ToDouble(discountT[0].InnerText);
                XmlNodeList taxBase = ((XmlElement)amounts[0]).GetElementsByTagName("TaxBase");
                bill.TaxBase = Convert.ToDouble(taxBase[0].InnerText);
                XmlNodeList tIva = ((XmlElement)amounts[0]).GetElementsByTagName("IVA");
                bill.Iva = Convert.ToDouble(tIva[0].InnerText);
                XmlNodeList quote = ((XmlElement)amounts[0]).GetElementsByTagName("Quote");
                bill.Quote = Convert.ToDouble(quote[0].InnerText);
                XmlNodeList totalB = ((XmlElement)amounts[0]).GetElementsByTagName("Total");
                bill.Total = Convert.ToDouble(totalB[0].InnerText);
                XmlNodeList paid = ((XmlElement)b[0]).GetElementsByTagName("Paid");
                bill.Paid = Convert.ToInt16(paid[0].InnerText);
            }
            return bill;
        }

        private void xmlAddressDecoder(XmlElement xml, Address address)
        {
            XmlNodeList street = xml.GetElementsByTagName("Street");
            address.Street = street[0].InnerText;
            XmlNodeList number = xml.GetElementsByTagName("Number");
            address.Number = number[0].InnerText;
            XmlNodeList zip = xml.GetElementsByTagName("ZipCode");
            address.ZipCode = Convert.ToInt32(zip[0].InnerText);
            XmlNodeList town = xml.GetElementsByTagName("Town");
            address.Town = town[0].InnerText;
            XmlNodeList state = xml.GetElementsByTagName("State");
            address.State = state[0].InnerText;
        }
    }
}
