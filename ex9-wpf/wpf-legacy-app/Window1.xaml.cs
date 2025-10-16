using System;
using System.ComponentModel;
using System.Windows;
using WpfAddressBook.ViewModels;

namespace WpfAddressBook
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1
    {
        public Window1()
        {
            DataContext = new MainViewModel();
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (MessageBox.Show("Would you like to save the changes to the address book?", "Save Changes", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                ((IDisposable)DataContext).Dispose();

            base.OnClosing(e);
        }
   }
}