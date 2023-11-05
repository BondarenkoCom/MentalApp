using Android.App;
using Android.Content;
using MentalTest.Interfaces;
using System;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(MentalTest.Droid.Services.DatabaseAssetService))]
namespace MentalTest.Droid.Services
{
    public class DatabaseAssetService : IDatabaseAssetService
    {
        public void CopyDatabaseIfNotExists(string dbName, string destinationPath)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Checking if database exists at path: {destinationPath}");

                if (!File.Exists(destinationPath))
                {
                    System.Diagnostics.Debug.WriteLine($"Database not found at {destinationPath}. Copying from assets...");

                    using (var assetStream = Application.Context.Assets.Open(dbName))
                    using (var fileStream = new FileStream(destinationPath, FileMode.Create))
                    {
                        assetStream.CopyTo(fileStream);
                    }

                    System.Diagnostics.Debug.WriteLine($"Database successfully copied to {destinationPath}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Database already exists at {destinationPath}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CopyDatabaseIfNotExists: {ex.Message}");
                // Depending on the architecture and error handling policy of your app,
                // you may want to handle this exception here or let it bubble up.
                throw;
            }
        }
    }
}
