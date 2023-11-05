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

        //protected override void OnStart()
        //{
        //    // Убедитесь, что класс, реализующий IDatabaseAssetService, называется DatabaseAssetService
        //    DependencyService.Register<IDatabaseAssetService>();
        //    //DependencyService.Register<IDatabaseAssetService;
        //
        //}

        protected override void OnStart()
        {
            // Регистрируем сервис для доступа к нему через DependencyService.
            DependencyService.Register<IDatabaseAssetService>();

            // Получаем экземпляр сервиса.
            var databaseAssetService = DependencyService.Get<IDatabaseAssetService>();

            // Путь, куда будет скопирована база данных. Это зависит от платформы и может варьироваться.
            string destinationPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "MentalTestDB.db");

            // Вызываем метод для копирования базы данных, если она не существует.
            databaseAssetService.CopyDatabaseIfNotExists("MentalTestDB.db", destinationPath);

            // Прочие действия при старте приложения.
        }


        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
