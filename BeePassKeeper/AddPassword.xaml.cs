using BeePassKeeper.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BeePassKeeper
{
    public partial class AddPassword : ContentPage
    {
        private string _encryptionkey = "";
        public AddPassword(string encryption_key)
        {
            InitializeComponent();
            BindingContext = new InsertPasswordViewModel(encryption_key);
            _encryptionkey = encryption_key;
        }

        private async void OnViewPasswordsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ViewPasswords(_encryptionkey));
        }
    }
}
