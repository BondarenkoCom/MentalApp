using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using System.IO;
using Xamarin.Forms;
using Rg.Plugins.Popup;
using Android;

[assembly: Dependency(typeof(MentalTest.Droid.Services.DatabaseAssetService))]
namespace MentalTest.Droid
{
    [Activity(Label = "MentalTest", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Forms.Init(this, savedInstanceState);
            Popup.Init(this); // Initialize Popup plugin

            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if ((int)Build.VERSION.SdkInt >= 23)
            {
                if (CheckSelfPermission(Manifest.Permission.WriteExternalStorage) != Permission.Granted)
                {
                    RequestPermissions(new string[] { Manifest.Permission.WriteExternalStorage }, 1);
                }
            }
        }
    }
}
