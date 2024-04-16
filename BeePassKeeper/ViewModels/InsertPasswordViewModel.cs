using BeePassKeeper.Commands;
using BeePassKeeper.Database;
using BeePassKeeper.Models;
using BeePassKeeper.Security;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BeePassKeeper.ViewModels
{
    public class InsertPasswordViewModel : INotifyPropertyChanged
    {
        private string _password;
        private string _name;
        private string _description;
        private string _encryptionKey;
        private bool _isSaveEnabled = false;

        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged(nameof(Password));
                    UpdateSaveEnabled();
                }
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                    UpdateSaveEnabled();
                }
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public ICommand InsertPasswordCommand { get; }

        public bool IsSaveEnabled
        {
            get { return _isSaveEnabled; }
            set
            {
                if (_isSaveEnabled != value)
                {
                    _isSaveEnabled = value;
                    OnPropertyChanged(nameof(IsSaveEnabled));
                }
            }
        }

        public InsertPasswordViewModel(string encryptionKey)
        {
            _encryptionKey = encryptionKey;
            InsertPasswordCommand = new Command(async () => await InsertPasswordAsync(), CanInsertPassword);
            UpdateSaveEnabled();
        }

        private void UpdateSaveEnabled()
        {
            IsSaveEnabled = !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Password);
        }

        private bool CanInsertPassword()
        {
            return IsSaveEnabled;
        }

        private async Task InsertPasswordAsync()
        {
            Crypter crypter = new Crypter(_encryptionKey);
            string encryptedPassword = crypter.Encrypt(Password);

            PasswordModel newPassword = new PasswordModel
            {
                Name = Name,
                Description = Description,
                Password = encryptedPassword,
                EncryptionKey = _encryptionKey,
                DateAdded = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            await EstablishDatabase.Database.InsertAsync(newPassword);

            Name = string.Empty;
            Description = string.Empty;
            Password = string.Empty;

            await Application.Current.MainPage.DisplayAlert("Success", "Password added successfully!", "OK");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
