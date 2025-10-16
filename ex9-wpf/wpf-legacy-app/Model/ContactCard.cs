using System;

namespace WpfAddressBook.Model
{
    public class ContactCard
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string HomePhone { get; set; }
        public string WorkPhone { get; set; }
        public string Email { get; set; }
        public ContactAddress Address { get; set; }
    }
}
