using System;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics; // Для отладки

namespace MentalTest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VideoAdPage : ContentPage
    {
        public VideoAdPage()
        {
            InitializeComponent();
            Debug.WriteLine("VideoAdPage initialized"); // Лог инициализации страницы
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Debug.WriteLine("VideoAdPage appearing");
            var mediaElement = this.Content.FindByName<MediaElement>("mediaElement");
            if (mediaElement != null)
            {
                Debug.WriteLine("MediaElement found");
                mediaElement.MediaEnded += OnMediaEnded;
                Debug.WriteLine("MediaElement event handler attached");
            }
            else
            {
                Debug.WriteLine("MediaElement not found");
            }
        }


        private async void OnMediaEnded(object sender, EventArgs e)
        {
            Debug.WriteLine("Media ended"); // Лог после завершения воспроизведения видео
            await Navigation.PopModalAsync();
            Debug.WriteLine("Ad page closed"); // Лог после закрытия модального окна
        }
    }
}
