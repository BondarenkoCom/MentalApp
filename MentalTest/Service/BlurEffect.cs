using Xamarin.Forms;

namespace MentalTest.Service
{
    public class BlurEffect : RoutingEffect
    {
        public int Radius { get; set; }

        public BlurEffect() : base("MentalTest.BlurEffect")
        {
        }

        public static readonly BindableProperty RadiusProperty =
            BindableProperty.Create(nameof(Radius), typeof(int), typeof(BlurEffect), 10);
    }
}
