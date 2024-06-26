using MentalTest.ViewModels;
using Rg.Plugins.Popup.Services;
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
            await ShowVideoAdPopup();
        }

        private async Task ShowVideoAdPopup()
        {
            try
            {
                Debug.WriteLine("Attempting to show video ad popup");
                var videoAdPage = new VideoAdPopupPage();
                await PopupNavigation.Instance.PushAsync(videoAdPage);
                Debug.WriteLine("Video ad popup shown successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to show video ad popup: {ex.Message}");
            }
        }
    }
}
