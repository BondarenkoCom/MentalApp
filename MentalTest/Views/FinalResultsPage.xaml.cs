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
    public partial class FinalResultsPage : ContentPage
    {
        // Измененный конструктор, принимающий результат теста
        public FinalResultsPage(string resultText)
        {
            InitializeComponent();
            BindingContext = new FinalResultsViewModel(resultText);
        }
    }
}