using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BeePassKeeper.Database;
using BeePassKeeper.Models;
using Xamarin.Forms;

namespace BeePassKeeper
{
    public partial class ViewPasswords : ContentPage
    {
        private ViewPasswordsViewModel _viewModel;
        private string enckey = "";

        public ViewPasswords(string activeEncryptionKey)
        {
            InitializeComponent();
            _viewModel = new ViewPasswordsViewModel(activeEncryptionKey);
            BindingContext = _viewModel;
            enckey = activeEncryptionKey;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await _viewModel.LoadPasswordsAsync();
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is PasswordModel password)
            {
                _viewModel.ShowPasswordDetails.Execute(password);
            }
        }

        private async void OnViewPasswordsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ViewPasswords(enckey));
        }
    }
}
