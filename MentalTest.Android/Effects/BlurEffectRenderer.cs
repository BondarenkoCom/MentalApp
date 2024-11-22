using System;
using System.Linq;
using Android.Graphics;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using MentalTest.Service;
using MentalTest.Droid.Effects;
using Android.Graphics.Drawables;
using static Android.Views.View;

[assembly: ResolutionGroupName("MentalTest")]
[assembly: ExportEffect(typeof(BlurEffectRenderer), nameof(BlurEffect))]
namespace MentalTest.Droid.Effects
{
    public class BlurEffectRenderer : PlatformEffect
    {
        private Android.Views.View view;

        protected override void OnAttached()
        {
            view = Control ?? Container;

            if (view != null)
            {
                // Wait until the view is laid out
                view.Post(() =>
                {
                    if (view.Width > 0 && view.Height > 0)
                    {
                        UpdateBlur();
                    }
                    else
                    {
                        // Attach LayoutChange event to wait for the view to be laid out
                        view.LayoutChange += OnLayoutChange;
                    }
                });
            }
        }

        protected override void OnDetached()
        {
            if (view != null)
            {
                view.LayoutChange -= OnLayoutChange;
            }
        }

        private void OnLayoutChange(object sender, LayoutChangeEventArgs e)
        {
            if (view.Width > 0 && view.Height > 0)
            {
                view.LayoutChange -= OnLayoutChange;
                UpdateBlur();
            }
        }

        private void UpdateBlur()
        {
            try
            {
                var blurEffect = (BlurEffect)Element.Effects.FirstOrDefault(e => e is BlurEffect);

                if (blurEffect != null && view != null)
                {
                    if (view.Width <= 0 || view.Height <= 0)
                    {
                        System.Diagnostics.Debug.WriteLine("View dimensions are invalid.");
                        return;
                    }

                    Bitmap bitmap = GetBitmapFromView(view);

                    if (bitmap == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Failed to create bitmap from view.");
                        return;
                    }

                    var blurredBitmap = BlurBitmap(bitmap, blurEffect.Radius);

                    if (blurredBitmap == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Failed to blur bitmap.");
                        return;
                    }

                    // Set the blurred bitmap as the background
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        var drawable = new BitmapDrawable(view.Resources, blurredBitmap);
                        view.Background = drawable;
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unable to apply blur effect: {ex.Message}");
            }
        }

        private Bitmap GetBitmapFromView(Android.Views.View view)
        {
            try
            {
                Bitmap bitmap = Bitmap.CreateBitmap(view.Width, view.Height, Bitmap.Config.Argb8888);
                Canvas canvas = new Canvas(bitmap);
                view.Draw(canvas);
                return bitmap;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetBitmapFromView exception: {ex.Message}");
                return null;
            }
        }

        private Bitmap BlurBitmap(Bitmap bitmap, int radius)
        {
            try
            {
                // Ensure radius is at least 1
                radius = Math.Max(1, radius);

                // Calculate the downscale factor
                float downscaleFactor = 1f / radius;

                // Scale down the bitmap
                int width = (int)(bitmap.Width * downscaleFactor);
                int height = (int)(bitmap.Height * downscaleFactor);

                if (width <= 0 || height <= 0)
                {
                    System.Diagnostics.Debug.WriteLine("Invalid scaled bitmap dimensions.");
                    return null;
                }

                Bitmap scaledDown = Bitmap.CreateScaledBitmap(bitmap, width, height, false);

                // Scale back up to original size
                Bitmap blurredBitmap = Bitmap.CreateScaledBitmap(scaledDown, bitmap.Width, bitmap.Height, false);

                return blurredBitmap;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BlurBitmap exception: {ex.Message}");
                return null;
            }
        }
    }
}
