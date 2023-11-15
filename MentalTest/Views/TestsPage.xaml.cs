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
    public partial class TestsPage : ContentPage
    {
        public TestsPage(string categoryName)
        {

            Console.WriteLine("TestsPage constructor started."); // Beginning of constructor

            InitializeComponent();

            Console.WriteLine($"Initializing TestsPage for category: {categoryName}"); // After InitializeComponent and before ViewModel creation

            // Создаем экземпляр ViewModel и передаем имя категории в его конструктор
            var viewModel = new TestsPageViewModel(categoryName);

            Console.WriteLine("ViewModel created for TestsPage."); // After ViewModel creation

            // Привязываем контекст данных к нашей ViewModel
            BindingContext = viewModel;

            Console.WriteLine("TestsPage constructor completed."); // End of constructor
        }

        private void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var viewModel = BindingContext as TestsPageViewModel;
                viewModel?.ItemTappedCommand.Execute(e.SelectedItem);
                // Очищаем выбор, чтобы элемент не оставался выделенным
                ((ListView)sender).SelectedItem = null;
            }
        }
    }
}
