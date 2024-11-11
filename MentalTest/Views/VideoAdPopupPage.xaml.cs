using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Octane.Xamarin.Forms.VideoPlayer; // Correct namespace

namespace MentalTest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VideoAdPopupPage : PopupPage
    {
        public VideoAdPopupPage()
        {
            InitializeComponent();
            Console.WriteLine("VideoAdPopupPage initialized");

            // Subscribe to the Completed event
            videoPlayer.Completed += VideoPlayer_Completed;

            // Hide the "Close" button until the video ends
            closeButton.IsVisible = false;
        }

        private void VideoPlayer_Completed(object sender, EventArgs e)
        {
            Console.WriteLine("Video playback ended");
            // Show the "Close" button when the video ends
            closeButton.IsVisible = true;
        }

        private async void OnCloseButtonClicked(object sender, EventArgs e)
        {
            Console.WriteLine("Close button clicked");
            await PopupNavigation.Instance.PopAsync();
        }
    }
}
