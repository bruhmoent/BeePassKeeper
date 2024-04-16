using BeePassKeeper.Database;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BeePassKeeper
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage())
            {
                BarBackgroundColor = Color.FromHex("#040000"),
                BarTextColor = Color.White
            };

            InitializeDatabase();
        }
        private async void InitializeDatabase()
        {
            await EstablishDatabase.InitializeDatabaseAsync();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
