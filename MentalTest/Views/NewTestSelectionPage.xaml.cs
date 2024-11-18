using MentalTest.ViewModels;
using System;
using System.Linq;
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
            //_viewModel.Navigation = Navigation;
            BindingContext = _viewModel;
        }

        //protected override bool OnBackButtonPressed()
        //{
        //    Navigation.PushAsync(new TestsPage("null"));
        //    return true; 
        //}

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private async void OnButtonTapped(object sender, EventArgs e)
        {
            var frame = (Frame)sender;

            // Animate the button (simulate press)
            await frame.ScaleTo(0.95, 50);
            await frame.ScaleTo(1, 50);

            // Get the bound TestType object
            var testType = frame.BindingContext as TestType;

            if (testType != null)
            {
                // Save the selected test name
                Application.Current.Properties["selectedTestName"] = testType.Name;
                await Application.Current.SavePropertiesAsync();

                System.Diagnostics.Debug.WriteLine($"Selected Test Name: {testType.Name}");

                // Navigate to the TestsPage
                await Navigation.PushAsync(new TestsPage(testType.Name));
            }
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

        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedTestType = e.CurrentSelection.FirstOrDefault() as TestType;

            if (selectedTestType != null)
            {
                await Navigation.PushAsync(new TestsPage(selectedTestType.Name));

                Application.Current.Properties["selectedTestName"] = selectedTestType.Name;
                await Application.Current.SavePropertiesAsync();

                System.Diagnostics.Debug.WriteLine($"Selected Test Name: {selectedTestType.Name}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("OnSelectionChanged was triggered but no valid TestType was selected.");
            }

    // Deselect the item to allow re-selection
    ((CollectionView)sender).SelectedItem = null;
        }

    }
}
