using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MentalTest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SplashPage : ContentPage
    {
        public SplashPage()
        {
            InitializeComponent();
            StartLoadingAnimation();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await Task.Delay(2000);

            await Navigation.PushAsync(new NewTestSelectionPage());
        }

        private void StartLoadingAnimation()
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(500), () =>
            {
                LoadingText.Opacity = LoadingText.Opacity == 1 ? 0.5 : 1;
                return true; 
            });

            Device.StartTimer(TimeSpan.FromMilliseconds(300), () =>
            {
                Dot1.Opacity = (Dot1.Opacity + 0.3) % 1.2;
                Dot2.Opacity = (Dot2.Opacity + 0.3) % 1.2;
                Dot3.Opacity = (Dot3.Opacity + 0.3) % 1.2;
                return true;
            });
        }
    }
}
