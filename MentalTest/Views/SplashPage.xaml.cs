using System;
using System.Diagnostics;
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

            // Проверка доступности шрифта
            CheckFontAvailability();

            StartHeartBeatAnimation();
            StartLoadingAnimation();
        }

        private void CheckFontAvailability()
        {
            try
            {
                // Тестовая метка для проверки шрифта
                var testLabel = new Label
                {
                    Text = "Font test: Origram",
                    FontFamily = "Origram",
                    FontSize = 18
                };

                Debug.WriteLine($"Font family set to: {testLabel.FontFamily}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Font loading failed: {ex.Message}");
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await Task.Delay(7000);

            await Navigation.PushAsync(new NewTestSelectionPage());
        }

        private void StartHeartBeatAnimation()
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(1000), () =>
            {
                HeartLogo.ScaleTo(1.2, 300, Easing.CubicInOut)
                        .ContinueWith(_ => HeartLogo.ScaleTo(1.0, 300, Easing.CubicInOut));
                return true;
            });
        }

        private void StartLoadingAnimation()
        {
            // Анимация текста
            Device.StartTimer(TimeSpan.FromMilliseconds(500), () =>
            {
                LoadingText.Opacity = LoadingText.Opacity == 1 ? 0.5 : 1;
                return true;
            });

            // Анимация точек
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
