using System;
using System.IO;
using MentalTest;
using MentalTest.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(MentalTest.Droid.FileHelper))] // Register with DependencyService
namespace MentalTest.Droid
{
    internal class FileHelper : IFileHelper
    {
        public string GetLocalFilePath(string filename)
        {
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                string fullPath = Path.Combine(path, filename);
                Console.WriteLine($"GetLocalFilePath: Path for file '{filename}' is '{fullPath}'");
                return fullPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetLocalFilePath: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return string.Empty; // Return an empty string in case of error
            }
        }
    }
}
