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
        private bool _videoWatched;

        public VideoAdPopupPage()
        {
            InitializeComponent();
            Console.WriteLine("VideoAdPopupPage initialized");

            videoPlayer.Completed += VideoPlayer_Completed;

            closeButton.IsVisible = false;
            _videoWatched = false;
        }

        private void VideoPlayer_Completed(object sender, EventArgs e)
        {
            Console.WriteLine("Video playback ended");
            closeButton.IsVisible = true;

            _videoWatched = true;
        }

        private async void OnCloseButtonClicked(object sender, EventArgs e)
        {
            if (_videoWatched)
            {
                Console.WriteLine("Close button clicked, video watched");
                await PopupNavigation.Instance.PopAsync();
            }
            else
            {
                Console.WriteLine("Attempt to close before watching video");
                await DisplayAlert("Watch the ad", "Please watch the full ad before closing.", "OK");
            }
        }
    }
}
