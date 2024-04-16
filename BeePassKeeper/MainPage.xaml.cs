using BeePassKeeper.Commands;
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
    public partial class MainPage : ContentPage
    {
        InsertEncryptionKeyViewModel viewModel = new InsertEncryptionKeyViewModel();
        ResetDatabaseCommand resetDatabaseCommand;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = viewModel;

            InitializePicker();
            // Subscribe to the onkeyadded event
            viewModel.InsertKeyCommand.KeyAdded += OnKeyAdded;
            resetDatabaseCommand = new ResetDatabaseCommand(viewModel);
            // Subscribe to the ResetDatabaseCommand_DatabaseReset event
            resetDatabaseCommand.DatabaseReset += ResetDatabaseCommand_DatabaseReset;
        }

        private async void ResetDatabaseCommand_DatabaseReset(object sender, EventArgs e)
        {
            await viewModel.RefreshKeys2();
        }

        private async void OnKeyAdded(object sender, EventArgs e)
        {
            await viewModel.RefreshKeys2();
        }

        private async void InitializePicker()
        {
            await viewModel.RefreshKeys2();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await viewModel.RefreshKeys2();
        }

        public async Task RefreshPicker22()
        {
            var allKeys = await viewModel.GetEncryptionKeys();

            KeyPicker.Items.Clear();

            foreach (var key in allKeys)
            {
                KeyPicker.Items.Add(key);
            }

            KeyPicker.Items.Add("Add New Key");
        }

        private void KeyPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            var selectedKey = (string)picker.SelectedItem;

            if (selectedKey == "Add New Key")
            {
                NewKeyEntry.IsVisible = true;
            }
            else
            {
                NewKeyEntry.IsVisible = false;
                viewModel.EncryptionKey = selectedKey;
            }
        }

    }
}
