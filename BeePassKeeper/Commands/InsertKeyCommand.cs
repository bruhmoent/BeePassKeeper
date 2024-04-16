using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BeePassKeeper.Database;
using BeePassKeeper.Database.Models;
using Xamarin.Forms;

namespace BeePassKeeper.Commands
{
    public class InsertKeyCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private readonly MainPage _mainPage;

        private readonly ViewModels.InsertEncryptionKeyViewModel _viewModel;

        public InsertKeyCommand(ViewModels.InsertEncryptionKeyViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return _viewModel != null && !string.IsNullOrEmpty(_viewModel.EncryptionKey);
        }

        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }
        public async Task ExecuteAsync(object parameter)
        {
            bool userConfirmed = await Application.Current.MainPage.DisplayAlert(
                "Confirmation",
                "Are you sure you want to use this key?",
                "Yes",
                "No");

            if (userConfirmed)
            {
                if (string.IsNullOrEmpty(_viewModel?.EncryptionKey))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Encryption key cannot be empty", "OK");
                    return;
                }

                string encryptionKey = _viewModel.EncryptionKey;

                if (encryptionKey == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Encryption key is null", "OK");
                    return;
                }

                var existingKey = await EstablishDatabase.Database.Table<EncryptionKeyModel>()
                                                .Where(k => k.Key == encryptionKey)
                                                .FirstOrDefaultAsync();

                if (existingKey == null)
                {
                    string password = await Application.Current.MainPage.DisplayPromptAsync(
                        "Set Password",
                        "Please set a password for this encryption key:",
                        maxLength: 50,
                        keyboard: Keyboard.Text,
                        placeholder: "Password");

                    if (string.IsNullOrEmpty(password))
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Password cannot be empty", "OK");
                        return;
                    }

                    var encryptionKeyModel = new EncryptionKeyModel { Key = encryptionKey, Password = password };
                    await EstablishDatabase.Database.InsertAsync(encryptionKeyModel);
                    OnKeyAdded();
                }
            }
        }

        public event EventHandler KeyAdded;
        protected virtual void OnKeyAdded()
        {
            KeyAdded?.Invoke(this, EventArgs.Empty);
        }

    }
}
