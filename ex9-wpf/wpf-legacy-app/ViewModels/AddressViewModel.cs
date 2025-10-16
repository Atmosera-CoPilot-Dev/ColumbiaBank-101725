using WpfAddressBook.Model;

namespace WpfAddressBook.ViewModels
{
    public class AddressViewModel : JulMar.Windows.Mvvm.ViewModel
    {
        ContactAddress _address;

        public AddressViewModel(ContactCard card)
        {
            if (card.Address == null)
                card.Address = new ContactAddress();
            _address = card.Address;
        }

        public string Street
        {
            get { return _address.Street; }
            set { _address.Street = value; OnPropertyChanged("Street"); }
        }

        public string City
        {
            get { return _address.City; }
            set { _address.City = value; OnPropertyChanged("City"); }
        }

        public string State
        {
            get { return _address.State; }
            set { _address.State = value; OnPropertyChanged("State"); }
        }

        public string ZipCode
        {
            get { return _address.ZipCode; }
            set { _address.ZipCode = value; OnPropertyChanged("ZipCode"); }
        }
    }
}
