using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Windows.Input;
using BeePassKeeper.Database;
using BeePassKeeper.Database.Models;
using BeePassKeeper.Commands;
using BeePassKeeper.Models;
using System.Collections.Generic;
using System.Linq;

namespace BeePassKeeper.ViewModels
{
    public class InsertEncryptionKeyViewModel : INotifyPropertyChanged
    {
        private string _encryptionKey;
        private string _password;
        private string _name;
        private string _description;
        private InsertPasswordViewModel _passwordViewModel;
        private InsertKeyCommand _insertKeyCommand;
        private InsertPasswordCommand _insertPasswordCommand;
        private bool _showButton = false;
        private bool _canExecuteLogin;
        private ResetDatabaseCommand _resetKeysCommand;

        private IList<string> _keys;

        public IList<string> Keys
        {
            get
            {
                return _keys;
            }
            set
            {
                if (_keys != value)
                {
                    _keys = value;
                }
            }
        }

        public ResetDatabaseCommand ResetDatabaseCommand
        {
            get { return _resetKeysCommand; }
            set
            {
                _resetKeysCommand = value;
                OnPropertyChanged(nameof(Keys));
            }
        }

        public void RefreshKeys()
        {
            var allKeys = GetEncryptionKeys().Result;

            _keys = new List<string>();

            _keys.Clear();

            foreach (var key in allKeys)
            {
                _keys.Add(key);
            }

            _keys.Add("Add New Key");
            Keys = _keys;

            OnPropertyChanged(nameof(Keys));
        }

        public async Task RefreshKeys2()
        {
            var allKeys = await GetEncryptionKeys();

            if (_keys == null)
            {
                _keys = new List<string>();
            }

            _keys = allKeys;
            _keys.Add("Add New Key");

            OnPropertyChanged(nameof(Keys));
        }

        public IList<string> EncryptionKeys { get; private set; }

        public string EncryptionKey
        {
            get { return _encryptionKey; }
            set
            {
                if (_encryptionKey != value)
                {
                    _encryptionKey = value;
                    OnPropertyChanged(nameof(EncryptionKey));

                    ShowButton = !string.IsNullOrEmpty(_encryptionKey);
                    OnPropertyChanged(nameof(ShowButton));
                    _passwordViewModel = new InsertPasswordViewModel(_encryptionKey);
                }
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged(nameof(Password));
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

        public bool ShowButton
        {
            get { return _showButton; }
            set
            {
                if (_showButton != value)
                {
                    _showButton = value;
                    OnPropertyChanged(nameof(ShowButton));
                }
            }
        }

        public Color ButtonTextColor { get; } = Color.White;

        public InsertKeyCommand InsertKeyCommand
        {
            get { return _insertKeyCommand; }
            set
            {
                _insertKeyCommand = value;
                OnPropertyChanged(nameof(InsertKeyCommand));
            }
        }
        public async Task<IList<string>> GetEncryptionKeys()
        {
            try
            {
                var encryptionKeyModels = await EstablishDatabase.Database.Table<EncryptionKeyModel>().ToListAsync().ConfigureAwait(false);
                return encryptionKeyModels.Select(k => k.Key).ToList();
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }


        public InsertPasswordCommand InsertPasswordCommand
        {
            get { return _insertPasswordCommand; }
            set
            {
                _insertPasswordCommand = value;
                OnPropertyChanged(nameof(InsertPasswordCommand));
            }
        }

        public ICommand SetEncryptionKeyCommand { get; }

        public InsertEncryptionKeyViewModel()
        {
            ResetDatabaseCommand = new ResetDatabaseCommand(this);
            ButtonTextColor = Color.White;

            InsertKeyCommand = new InsertKeyCommand(this);

            SetEncryptionKeyCommand = new Command(SetEncryptionKey);
        }

        public void CreateInsertKeyCommand()
        {
            InsertKeyCommand.CanExecuteChanged += (s, e) => CanExecuteLogin = InsertKeyCommand.CanExecute(ShowButton);
        }

        public async void ExecuteResetDatabaseCommand()
        {
            bool userConfirmed = await Application.Current.MainPage.DisplayAlert(
                "Confirmation",
                "Are you sure you want to reset the database?",
                "Yes",
                "No");

            if (userConfirmed)
            {
                await ClearDatabase();
                await RefreshKeys2();
            }
        }

        public bool CanExecuteLogin
        {
            get => _canExecuteLogin;
            set
            {
                if (_canExecuteLogin != value)
                {
                    _canExecuteLogin = value;
                    OnPropertyChanged(nameof(CanExecuteLogin));
                }
            }
        }

        public bool IsExecutable()
        {
            return true;
        }

        private async Task ClearDatabase()
        {
            // Drop every table to ensure there is no leftover data.
            await EstablishDatabase.Database.DropTableAsync<PasswordModel>().ConfigureAwait(false);
            await EstablishDatabase.Database.DropTableAsync<EncryptionKeyModel>().ConfigureAwait(false);

            await EstablishDatabase.Database.CreateTableAsync<PasswordModel>().ConfigureAwait(false);
            await EstablishDatabase.Database.CreateTableAsync<EncryptionKeyModel>().ConfigureAwait(false);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async void SetEncryptionKey()
        {
            if (!string.IsNullOrEmpty(EncryptionKey))
            {
                await InsertKeyCommand.ExecuteAsync(null);


                await Application.Current.MainPage.Navigation.PushAsync(new AddPassword(EncryptionKey));
            }
        }

    }
}
