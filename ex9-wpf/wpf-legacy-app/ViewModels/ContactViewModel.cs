using WpfAddressBook.Model;

namespace WpfAddressBook.ViewModels
{
    public class ContactViewModel : JulMar.Windows.Mvvm.ViewModel
    {
        private ContactCard _card;
        private AddressViewModel _cardAddress;

        public ContactViewModel(ContactCard card)
        {
            _card = card;
            _cardAddress = new AddressViewModel(card);
        }

        internal ContactCard ContactCard
        {
            get { return _card; }
        }

        public string Name 
        {
            get { return _card.Name; }
            set { _card.Name = value; OnPropertyChanged("Name"); }
        }

        public string HomePhone
        {
            get { return _card.HomePhone; }
            set { _card.HomePhone = value; OnPropertyChanged("HomePhone"); }
        }

        public string WorkPhone
        {
            get { return _card.WorkPhone; }
            set { _card.WorkPhone = value; OnPropertyChanged("WorkPhone"); }
        }

        public string Email
        {
            get { return _card.Email; }
            set { _card.Email = value; OnPropertyChanged("Email"); }
        }

        public AddressViewModel Address
        {
            get { return _cardAddress; }
        }
    }
}
