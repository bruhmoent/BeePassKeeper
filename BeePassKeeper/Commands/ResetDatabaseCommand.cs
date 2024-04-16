using System;
using System.Windows.Input;
using BeePassKeeper.ViewModels;

namespace BeePassKeeper.Commands
{
    public class ResetDatabaseCommand : ICommand
    {
        private readonly InsertEncryptionKeyViewModel _viewModel;

        public event EventHandler DatabaseReset;

        public ResetDatabaseCommand(InsertEncryptionKeyViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _viewModel.ExecuteResetDatabaseCommand();
            OnDatabaseReset();
        }

        protected virtual void OnDatabaseReset()
        {
           //_viewModel?.RefreshKeys();
        }
    }
}
