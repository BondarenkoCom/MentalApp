using MentalTest.ViewModels;
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
            _viewModel.Navigation = Navigation;  // передаем текущий объект Navigation в ViewModel
            BindingContext = _viewModel;

            System.Diagnostics.Debug.WriteLine("NewTestSelectionPage Initialized and ViewModel set.");
        }

        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            // Получение выбранного объекта (если вам это нужно)
            var selectedTestType = e.Item as TestType;

            if (selectedTestType != null)
            {
                // Переход на страницу с карточками тестов
                await Navigation.PushAsync(new TestsPage(selectedTestType.Name));

                // Сброс выбора (чтобы элемент не оставался выделенным)
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
