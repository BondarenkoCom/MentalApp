using MentalTest.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MentalTest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SurveyPage : ContentPage
    {
        public SurveyPage(testCard test)
        {
            InitializeComponent();
            BindingContext = new SurveyViewModel(test.id);
        }
    }
}