using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MentalTest.Views;
using Xamarin.Forms.Internals;
using MentalTest.Interfaces;
using System.IO;

namespace MentalTest
{
    public partial class App : Application
    {
        public App()
        {
            Log.Listeners.Add(new DelegateLogListener((arg1, arg2) => System.Diagnostics.Debug.WriteLine(arg2)));
            InitializeComponent();
            MainPage = new NavigationPage(new SplashPage());
        }

        protected override void OnStart()
        {
            DependencyService.Register<IDatabaseAssetService>();

            var databaseAssetService = DependencyService.Get<IDatabaseAssetService>();

            string destinationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "MentalTestDB.db");

            databaseAssetService.CopyDatabaseIfNotExists("MentalTestDB.db", destinationPath);
        }


        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
