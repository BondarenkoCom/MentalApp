using MentalTest.Models;
using MentalTest.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MentalTest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SurveyPage : ContentPage
    {
        private SurveyViewModel _viewModel;

        public SurveyPage(TestCardModel test)
        {
            InitializeComponent();
            Console.WriteLine("SurveyPage constructor started.");

            _viewModel = new SurveyViewModel(test.id);
            BindingContext = _viewModel;

            Device.BeginInvokeOnMainThread(async () => {
                Console.WriteLine("BeginInvokeOnMainThread started.");
                await _viewModel.InitializeAsync(test.id);
                Console.WriteLine("InitializeAsync completed.");
            });

            Console.WriteLine("SurveyPage constructor completed.");
        }
    }
}
