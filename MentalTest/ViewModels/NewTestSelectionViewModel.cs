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
                new TestType { Name = "Personality"},
                new TestType { Name = "Job"},
                new TestType { Name = "love & sex"},
                new TestType { Name = "Career"},
                new TestType { Name = "Fun"},
            };
        }
    }

    public class TestType
    {
        public string Name { get; set; }
        public int TestCount { get; set; }
    }
}
