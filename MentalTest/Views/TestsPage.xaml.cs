using MentalTest.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MentalTest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestsPage : ContentPage
    {
        private TestsPageViewModel _viewModel;

        public TestsPage(string categoryName)
        {
            Console.WriteLine($"TestsPage constructor called with category: {categoryName}");
            InitializeComponent();
            InitializeViewModelAsync(categoryName);
        }


        private async void InitializeViewModelAsync(string categoryName)
        {
            Console.WriteLine("Initializing TestsPage ViewModel...");
            _viewModel = new TestsPageViewModel(categoryName);
            BindingContext = _viewModel;

            await _viewModel.InitializeDataAsync();
            Console.WriteLine("TestsPage ViewModel initialized.");
        }

        private void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                Console.WriteLine($"Item selected: {e.SelectedItem}");
                _viewModel.ItemTappedCommand.Execute(e.SelectedItem);
                ((ListView)sender).SelectedItem = null;
            }
            else
            {
                Console.WriteLine("Item selection cleared.");
            }
        }
    }
}
