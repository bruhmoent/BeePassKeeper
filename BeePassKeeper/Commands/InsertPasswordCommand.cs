using System;
using System.Windows.Input;
using BeePassKeeper.Database;
using BeePassKeeper.Models;
using BeePassKeeper.ViewModels;
using Xamarin.Forms;

namespace BeePassKeeper.Commands
{
    public class InsertPasswordCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly InsertPasswordViewModel _viewModel;
        private readonly string _encryptionKey;

        public InsertPasswordCommand(InsertPasswordViewModel viewModel, string encryptionKey)
        {
            _viewModel = viewModel;
            _encryptionKey = encryptionKey;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            bool userConfirmed = await Application.Current.MainPage.DisplayAlert(
               "Confirmation",
               "Are you sure you want to save the password?",
               "Yes",
               "No");

            if (userConfirmed)
            {
                string password = _viewModel.Password;
                string name = _viewModel.Name;
                string description = _viewModel.Description;

                var passwordModel = new PasswordModel
                {
                    Password = password,
                    Name = name,
                    Description = description,
                    DateAdded = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    EncryptionKey = _encryptionKey
                };

                await EstablishDatabase.Database.InsertAsync(passwordModel);
            }
        }
    }
}
