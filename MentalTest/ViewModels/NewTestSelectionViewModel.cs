using MentalTest.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace MentalTest.ViewModels
{
    public class NewTestSelectionViewModel
    {
        public ObservableCollection<TestType> TestTypes { get; set; }

        public INavigation Navigation { get; set; }

        public NewTestSelectionViewModel()
        {
            TestTypes = new ObservableCollection<TestType>
            {
                new TestType { Name = "Personality", TestCount = 5 },
                new TestType { Name = "Job", TestCount = 3 },
                new TestType { Name = "love & sex", TestCount = 5 },
                new TestType { Name = "Career", TestCount = 5 },
                new TestType { Name = "Fun", TestCount = 5 },
                // Добавьте другие типы тестов
            };
        }
    }

    public class TestType
    {
        public string Name { get; set; }
        public int TestCount { get; set; }
    }
}
