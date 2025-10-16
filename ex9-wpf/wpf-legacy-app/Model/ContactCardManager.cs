using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace WpfAddressBook.Model
{
    public class ContactCardManager
    {
        public static IEnumerable<ContactCard> Load(string filename)
        {
            return from ac in XDocument.Load(filename).Descendants("card")
                 select new ContactCard
                 {
                     Id = new Guid(ac.Element("id").Value),
                     Name = ac.Element("name").Value,
                     HomePhone = ac.Element("homePhone").Value,
                     WorkPhone = ac.Element("workPhone").Value,
                     Email = ac.Element("email").Value,
                     Address = new ContactAddress
                     {
                         Street = ac.Element("address").Element("street").Value,
                         City = ac.Element("address").Element("city").Value,
                         State = ac.Element("address").Element("state").Value,
                         ZipCode = ac.Element("address").Element("zipCode").Value
                     }
                 };
        }

        public static void Save(string filename, IEnumerable<ContactCard> cards)
        {
            XElement doc = new XElement("addressCards",
                       from card in cards
                       select new XElement("card",
                           new XElement("id", card.Id.ToString()),
                           new XElement("name", card.Name),
                           new XElement("homePhone", card.HomePhone),
                           new XElement("workPhone", card.WorkPhone),
                           new XElement("email", card.Email),
                           new XElement("address",
                               new XElement("street", card.Address.Street),
                               new XElement("city", card.Address.City),
                               new XElement("state", card.Address.State),
                               new XElement("zipCode", card.Address.ZipCode))));
            doc.Save(filename);
        }
    }
}
