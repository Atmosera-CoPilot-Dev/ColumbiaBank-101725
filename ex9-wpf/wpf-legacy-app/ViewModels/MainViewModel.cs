using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using JulMar.Windows.Mvvm;
using WpfAddressBook.Model;

namespace WpfAddressBook.ViewModels
{
    public class MainViewModel : JulMar.Windows.Mvvm.ViewModel
    {
        static readonly string AB_FILENAME = @"addressbook.xml";

        public ObservableCollection<ContactViewModel> Contacts { get; private set; }

        private ContactViewModel _selectedCard;
        public ContactViewModel SelectedCard
        {
            get { return _selectedCard; }
            set { _selectedCard = value; OnPropertyChanged("SelectedCard"); }
        }

        public ICommand AddCommand { get; private set; }
        public ICommand RemoveCommand { get; private set; }

        public MainViewModel()
        {
            Contacts = new ObservableCollection<ContactViewModel>();

            // Load the contact cards
            foreach (var card in ContactCardManager.Load(AB_FILENAME))
                Contacts.Add(new ContactViewModel(card));

            if (Contacts.Count > 0)
                SelectedCard = Contacts[0];

            AddCommand = new DelegatingCommand(OnAdd);
            RemoveCommand = new DelegatingCommand(OnRemove, OnCanRemove);
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                ContactCardManager.Save(AB_FILENAME, 
                    Contacts.Select(cvm => cvm.ContactCard));
            }
        }

        void OnAdd()
        {
            ContactCard newCard = new ContactCard();
            newCard.Id = Guid.NewGuid();

            ContactViewModel vm = new ContactViewModel(newCard);
            Contacts.Add(vm);
            SelectedCard = vm;
        }

        void OnRemove()
        {
            if (SelectedCard != null)
            {
                int index = Contacts.IndexOf(SelectedCard);
                Contacts.Remove(SelectedCard);
                if (Contacts.Count > index)
                    SelectedCard = Contacts[index];
                else if (Contacts.Count > 0)
                    SelectedCard = Contacts[0];
                else
                    SelectedCard = null;
            }
        }

        bool OnCanRemove()
        {
            return SelectedCard != null; 
        }
    }
}
