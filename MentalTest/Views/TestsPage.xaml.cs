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
            InitializeComponent();
            InitializeViewModelAsync(categoryName);
        }

        private async void InitializeViewModelAsync(string categoryName)
        {
            Console.WriteLine("TestsPage constructor started.");
            Console.WriteLine($"Initializing TestsPage for category: {categoryName}");

            _viewModel = new TestsPageViewModel(categoryName);
            BindingContext = _viewModel;

            await _viewModel.InitializeDataAsync(); 
            Console.WriteLine("ViewModel created for TestsPage.");
            Console.WriteLine("TestsPage constructor completed.");
        }

        private void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                _viewModel.ItemTappedCommand.Execute(e.SelectedItem);
                ((ListView)sender).SelectedItem = null;
            }
        }
    }
}
