using BeePassKeeper.Database;
using BeePassKeeper.Models;
using BeePassKeeper.Security;
using System;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace BeePassKeeper
{
    public partial class PasswordPage : ContentPage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private PasswordModel _password;
        private string _encryptionKey;

        public PasswordModel Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string DecipheredPassword { get; set; }

        public ICommand SaveCommand { get; }

        public PasswordPage(PasswordModel password, string encryptionKey)
        {
            InitializeComponent();
            _encryptionKey = encryptionKey;

            Password = password;
            DecipheredPassword = DecipherPassword(password.Password);

            SaveCommand = new Command(SaveChanges);

            BindingContext = this;
        }

        private string DecipherPassword(string encryptedPassword)
        {
            Crypter crypter = new Crypter(_encryptionKey);

            return crypter.Decrypt(encryptedPassword);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void SaveChanges()
        {
            Crypter crypter = new Crypter(_encryptionKey);
            string encryptedPassword = crypter.Encrypt(DecipheredPassword);

            Password.Password = encryptedPassword;

            await EstablishDatabase.Database.UpdateAsync(Password);

            await DisplayAlert("Success", "Changes saved successfully!", "OK");
        }
    }
}
