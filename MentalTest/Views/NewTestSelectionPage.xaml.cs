using MentalTest.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MentalTest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewTestSelectionPage : ContentPage
    {
        private NewTestSelectionViewModel _viewModel;

        public NewTestSelectionPage()
        {
            InitializeComponent();

            _viewModel = new NewTestSelectionViewModel();
            _viewModel.Navigation = Navigation;
            BindingContext = _viewModel;
        }

        protected override bool OnBackButtonPressed()
        {
            Navigation.PushAsync(new TestsPage("null"));
            return true; 
        }

        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            var selectedTestType = e.Item as TestType;

            if (selectedTestType != null)
            {
                await Navigation.PushAsync(new TestsPage(selectedTestType.Name));

                ((ListView)sender).SelectedItem = null;

                Application.Current.Properties["selectedTestName"] = selectedTestType.Name;
                await Application.Current.SavePropertiesAsync();

                System.Diagnostics.Debug.WriteLine($"Selected Test Name: {selectedTestType.Name}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("OnItemTapped was triggered but no valid TestType was selected.");
            }
        }
    }
}
