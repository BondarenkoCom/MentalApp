using System;
using Xamarin.Forms;
using MentalTest.Views;
using Xamarin.Forms.Internals;
using MentalTest.ViewModels;
using MentalTest.Service;

namespace MentalTest
{
    public partial class NewApp : Application
    {
        public NewApp()
        {
            Log.Listeners.Add(new DelegateLogListener((arg1, arg2) => System.Diagnostics.Debug.WriteLine(arg2)));
            InitializeComponent();

            // Initialize Sharpnado MaterialFrame
            Sharpnado.MaterialFrame.Initializer.Initialize(loggerEnable: false, debugLogEnable: false);

            DependencyService.Register<ApiService>();

            MessagingCenter.Subscribe<SurveyViewModel, string>(this, "FinishTest", async (sender, resultText) =>
            {
                await MainPage.Navigation.PushAsync(new NewFinalResultsPage(resultText));
            });

            MainPage = new NavigationPage(new SplashPage());
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
