using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using MentalTest.Service;

namespace MentalTest.ViewModels
{
    // Category page ViewModel
    public class NewTestSelectionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<TestType> TestTypes { get; set; }

        private ApiService _apiService;

        public NewTestSelectionViewModel()
        {
            TestTypes = new ObservableCollection<TestType>
            {
                new TestType { Name = "Personality", AccentColor = "#9C27B0", Icon = "pers.png" },
                new TestType { Name = "Job", AccentColor = "#2196F3", Icon = "job.png" },
                new TestType { Name = "Love & Sex", AccentColor = "#E91E63", Icon = "love.png" },
                new TestType { Name = "Career", AccentColor = "#4CAF50", Icon = "career.png" },
                new TestType { Name = "Fun", AccentColor = "#FF9800", Icon = "fun.png" },
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
        private string _accentColor;
        private string _icon;

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

        public string AccentColor
        {
            get => _accentColor;
            set
            {
                _accentColor = value;
                OnPropertyChanged();
            }
        }

        public string Icon
        {
            get => _icon;
            set
            {
                _icon = value;
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
