using MentalTest.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics; 
using System.Threading.Tasks; 
using System;

namespace MentalTest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FinalResultsPage : ContentPage
    {
        public FinalResultsPage(string resultText)
        {
            InitializeComponent();
            BindingContext = new FinalResultsViewModel(resultText);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Debug.WriteLine("FinalResultsPage appearing"); 
            await ShowAd();
        }

        private async Task ShowAd()
        {
            try
            {
                Debug.WriteLine("Creating ad page"); 
                var adPage = new VideoAdPage();
                Debug.WriteLine("Pushing modal ad page"); 
                await Navigation.PushModalAsync(adPage);
                Debug.WriteLine("Ad page shown"); 
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error showing ad: {ex}"); 
            }
        }
    }
}
