using BeePassKeeper;
using BeePassKeeper.Database;
using BeePassKeeper.Models;
using BeePassKeeper.Security;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

public class ViewPasswordsViewModel : INotifyPropertyChanged
{
    private ObservableCollection<PasswordModel> allPasswords;
    private PasswordModel selectedPassword;
    private string _activeEncryptionKey;

    public event PropertyChangedEventHandler PropertyChanged;
    public ICommand FilterPasswords { get; }
    public ICommand ResetSearch { get; }
    public ICommand ShowPasswordDetails { get; }

    public string SearchCriteria { get; set; }

    public string SearchText { get; set; }

    public PasswordModel SelectedPassword
    {
        get => selectedPassword;
        set
        {
            if (selectedPassword != value)
            {
                selectedPassword = value;
                OnPropertyChanged(nameof(SelectedPassword));
            }
        }
    }

    private ObservableCollection<PasswordModel> displayedPasswords;
    public ObservableCollection<PasswordModel> DisplayedPasswords
    {
        get { return displayedPasswords; }
        set { displayedPasswords = value; }
    }

    private ObservableCollection<PasswordModel> AllPasswords
    {
        get { return allPasswords; }
        set
        {
            allPasswords = value;
            displayedPasswords = value;
        }
    }

    public ViewPasswordsViewModel(string activeEncryptionKey)
    {
        FilterPasswords = new Command(FilterPasswordsMethod);
        ResetSearch = new Command(ResetSearchMethod);
        ShowPasswordDetails = new Command<PasswordModel>(ShowPasswordDetailsMethod);
        _activeEncryptionKey = activeEncryptionKey;
        SearchCriteria = "Name";

        LoadPasswordsAsync();
    }

    public async Task LoadPasswordsAsync()
    {
        var allPasswordsList = await EstablishDatabase.Database.Table<PasswordModel>().Where(p => p.EncryptionKey == _activeEncryptionKey).ToListAsync();
        AllPasswords = new ObservableCollection<PasswordModel>(allPasswordsList);

        DisplayedPasswords = AllPasswords;

        OnPropertyChanged(nameof(DisplayedPasswords));
    }

    private void FilterPasswordsMethod()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            DisplayedPasswords = AllPasswords;
        }
        else
        {
            DisplayedPasswords = new ObservableCollection<PasswordModel>(
                AllPasswords.Where(p =>
                {
                    switch (SearchCriteria)
                    {
                        case "Name":
                            return p.Name != null && p.Name.ToLower().Contains(SearchText.ToLower());
                        case "Password":
                            // It is necessary to decrypt a password.
                            Crypter crypter = new Crypter(_activeEncryptionKey);
                            string decryptedPassword = crypter.Decrypt(p.Password);
                            return decryptedPassword != null && decryptedPassword.ToLower().Contains(SearchText.ToLower());
                        case "Description":
                            return p.Description != null && p.Description.ToLower().Contains(SearchText.ToLower());
                        default:
                            return false;
                    }
                }));
        }

        OnPropertyChanged(nameof(DisplayedPasswords));
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void ResetSearchMethod()
    {
        SearchText = string.Empty;
        DisplayedPasswords = AllPasswords;

        OnPropertyChanged(nameof(DisplayedPasswords));
    }

    private async void ShowPasswordDetailsMethod(PasswordModel selectedPassword)
    {
        if (selectedPassword != null)
        {
            string enteredPassword = await Application.Current.MainPage.DisplayPromptAsync(
                "Enter Password",
                "Please enter the encryption key password to view the password details:",
                maxLength: 50,
                keyboard: Keyboard.Text,
                placeholder: "Password");

            if (enteredPassword == selectedPassword.EncryptionKey)
            {
                await Application.Current.MainPage.Navigation.PushAsync(new PasswordPage(selectedPassword, _activeEncryptionKey));
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Incorrect encryption key password", "OK");
            }
        }
    }
}
