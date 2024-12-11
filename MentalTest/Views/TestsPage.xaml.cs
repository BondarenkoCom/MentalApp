using MentalTest.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Sharpnado.MaterialFrame; // Import for MaterialFrame

namespace MentalTest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestsPage : ContentPage
    {
        private TestsPageViewModel _viewModel;

        // Constructor initializing the page with a selected category
        public TestsPage(string categoryName)
        {
            Console.WriteLine($"TestsPage constructor called with category: {categoryName}");
            InitializeComponent();
            InitializeViewModelAsync(categoryName);
        }

        // Initialize the ViewModel with the selected category
        private async void InitializeViewModelAsync(string categoryName)
        {
            Console.WriteLine("Initializing TestsPage ViewModel...");
            _viewModel = new TestsPageViewModel(categoryName);
            BindingContext = _viewModel;

            await _viewModel.InitializeDataAsync(); // Load data into the client view
            Console.WriteLine("TestsPage ViewModel initialized.");
        }

        // Handle item selection in the ListView
        private void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                Console.WriteLine($"Item selected: {e.SelectedItem}");
                _viewModel.ItemTappedCommand.Execute(e.SelectedItem); // Trigger the command for item tap
                ((ListView)sender).SelectedItem = null; // Clear selection
            }
            else
            {
                Console.WriteLine("Item selection cleared.");
            }
        }

        // Add an animation effect when a card is tapped
        private async void OnCardTapped(object sender, EventArgs e)
        {
            if (sender is MaterialFrame frame) // Use MaterialFrame for better visuals
            {
                Console.WriteLine("Card tapped, starting animation.");
                await frame.ScaleTo(0.95, 50, Easing.CubicInOut); // Shrink effect
                await frame.ScaleTo(1, 50, Easing.CubicInOut); // Restore to original size
            }
        }

        // Navigate back to the previous page
        private void OnBackButtonClicked(object sender, EventArgs e)
        {
            Console.WriteLine("Back button clicked, navigating back.");
            Navigation.PopAsync(); // Return to the previous view
        }
    }
}
