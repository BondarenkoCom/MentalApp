using Xamarin.Forms;

namespace MentalTest.Views
{
    public class CustomFrame : Frame
    {
        public new CornerRadius CornerRadius { get; set; }

        public CustomFrame()
        {
            HasShadow = false;
        }
    }
}
