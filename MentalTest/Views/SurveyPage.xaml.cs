using MentalTest.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MentalTest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SurveyPage : ContentPage
    {
        public SurveyPage(TestItem test)
        {
            InitializeComponent();
            // Устанавливаем ViewModel с данными теста
            BindingContext = new SurveyViewModel(test);
        }
    }
}