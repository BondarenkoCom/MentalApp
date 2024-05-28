using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using MentalTest.Service; 
using System.Threading.Tasks;

namespace MentalTest.ViewModels
{
    //Category page
    public class NewTestSelectionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<TestType> TestTypes { get; set; }
        public INavigation Navigation { get; set; }

        private ApiService _apiService;

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
            _apiService = new ApiService();

            LoadTestCounts();
        }

        private async void LoadTestCounts()
        {
            foreach (var testType in TestTypes)
            {
                var testItems = await _apiService.GetTestItemsByCategoryAsync(testType.Name);
                testType.TestCount = testItems.Count;
            }

            OnPropertyChanged(nameof(TestTypes));
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class TestType : INotifyPropertyChanged
    {
        private string _name;
        private int _testCount;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public int TestCount
        {
            get => _testCount;
            set
            {
                _testCount = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
