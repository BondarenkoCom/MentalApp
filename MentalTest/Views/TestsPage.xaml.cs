using MentalTest.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MentalTest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestsPage : ContentPage
    {
        public TestsPage(string categoryName)
        {

            Console.WriteLine("TestsPage constructor started.");

            InitializeComponent();

            Console.WriteLine($"Initializing TestsPage for category: {categoryName}");

            var viewModel = new TestsPageViewModel(categoryName);

            Console.WriteLine("ViewModel created for TestsPage.");
            BindingContext = viewModel;

            Console.WriteLine("TestsPage constructor completed.");
        }

        private void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var viewModel = BindingContext as TestsPageViewModel;
                viewModel?.ItemTappedCommand.Execute(e.SelectedItem);

                ((ListView)sender).SelectedItem = null;
            }
        }
    }
}
