using Android.Content;
using Android.Graphics.Drawables;
using MentalTest.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(CustomFrame), typeof(MentalTest.Droid.Renderers.CustomFrameRenderer))]
namespace MentalTest.Droid.Renderers
{
    public class CustomFrameRenderer : FrameRenderer
    {
        public CustomFrameRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                UpdateBackground();
                UpdateCornerRadius();
                UpdateShadow();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == nameof(CustomFrame.CornerRadius) || e.PropertyName == nameof(CustomFrame))
            {
                UpdateCornerRadius();
            }

            if (e.PropertyName == nameof(Frame.HasShadow) || e.PropertyName == nameof(Frame))
            {
                UpdateShadow();
            }
        }

        private void UpdateBackground()
        {
            if (this.Element.BackgroundColor != Xamarin.Forms.Color.Default)
            {
                var color = this.Element.BackgroundColor.ToAndroid();
                this.SetBackgroundColor(color);
            }
            else
            {
                this.SetBackground(null);
            }
        }

        private void UpdateCornerRadius()
        {
            if (this.Background is GradientDrawable backgroundGradient)
            {
                var cornerRadius = (Element as CustomFrame)?.CornerRadius;
                if (!cornerRadius.HasValue)
                {
                    return;
                }

                var radii = new float[]
                {
                    Context.ToPixels(cornerRadius.Value.TopLeft),
                    Context.ToPixels(cornerRadius.Value.TopLeft),

                    Context.ToPixels(cornerRadius.Value.TopRight),
                    Context.ToPixels(cornerRadius.Value.TopRight),

                    Context.ToPixels(cornerRadius.Value.BottomRight),
                    Context.ToPixels(cornerRadius.Value.BottomRight),

                    Context.ToPixels(cornerRadius.Value.BottomLeft),
                    Context.ToPixels(cornerRadius.Value.BottomLeft)
                };

                backgroundGradient.SetCornerRadii(radii);
                this.SetBackground(backgroundGradient);
            }
        }

        private void UpdateShadow()
        {
            Elevation = 20;
            TranslationZ = 10;
        }
    }
}
