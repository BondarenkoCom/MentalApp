using MentalTest.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MentalTest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FinalResultsPage : ContentPage
    {
        public FinalResultsPage(string resultText)
        {
            InitializeComponent();
            BindingContext = new FinalResultsViewModel(resultText);
        }
    }
}