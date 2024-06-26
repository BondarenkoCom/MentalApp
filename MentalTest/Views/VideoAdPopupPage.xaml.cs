using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MentalTest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VideoAdPopupPage : PopupPage
    {
        public VideoAdPopupPage()
        {
            InitializeComponent();
            Console.WriteLine("VideoAdPopupPage initialized");
        }

        private async void OnCloseButtonClicked(object sender, EventArgs e)
        {
            Console.WriteLine("Close button clicked");
            await PopupNavigation.Instance.PopAsync();
        }
    }

}
