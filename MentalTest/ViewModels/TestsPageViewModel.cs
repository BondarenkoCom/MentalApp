using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using SQLite;
using System;
using System.Threading.Tasks;
using MentalTest.Views;
using MentalTest.Service;
using MentalTest.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MentalTest.ViewModels
{
    // ViewModel for managing test items and user interaction in the Tests Page
    public class TestsPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly ApiService _apiService = new ApiService(); // Service for API requests
        private ObservableCollection<TestCardModel> _testItems = new ObservableCollection<TestCardModel>();

        private bool _isDataLoading;

        // Indicates whether data is loading
        public bool IsDataLoading
        {
            get => _isDataLoading;
            set
            {
                _isDataLoading = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsDataLoaded));
            }
        }

        // Collection of test items to display
        public ObservableCollection<TestCardModel> TestItems
        {
            get => _testItems;
            set
            {
                _testItems = value;
                OnPropertyChanged();
            }
        }

        // Indicates whether data loading is complete
        public bool IsDataLoaded => !IsDataLoading;

        // Stores the selected category name
        public string SelectedCategory { get; private set; }

        // Command for handling item taps in the UI
        public Command<TestCardModel> ItemTappedCommand { get; }

        public TestsPageViewModel(string categoryName)
        {
            SelectedCategory = categoryName; // Set the selected category
            ItemTappedCommand = new Command<TestCardModel>(async (testItem) => await OnTestItemTapped(testItem));
        }

        // Initializes data by fetching from the API and loading from the local store
        public async Task InitializeDataAsync()
        {
            IsDataLoading = true;
            try
            {
                await FetchDataFromApiAsync(SelectedCategory); // Fetch data from API
                LoadDataFromLocalStore(); // Load data from local storage
            }
            finally
            {
                IsDataLoading = false;
            }
        }

        // Fetches test items from the API and saves them to local storage
        private async Task FetchDataFromApiAsync(string category)
        {
            try
            {
                var testItems = await _apiService.GetTestItemsByCategoryAsync(category);
                if (testItems?.Any() == true)
                {
                    DataStore.Instance.SaveTestItems(testItems); // Save items to the local store
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data from API: {ex.Message}");
            }
        }

        // Loads test items from the local storage into the UI collection
        private void LoadDataFromLocalStore()
        {
            try
            {
                var items = DataStore.Instance.TestItems;
                if (items?.Any() == true)
                {
                    TestItems = new ObservableCollection<TestCardModel>(items);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data from local store: {ex.Message}");
            }
        }

        // Handles the event when a test item is tapped, navigating to a detailed page
        private async Task OnTestItemTapped(TestCardModel testItem)
        {
            if (testItem != null)
            {
                try
                {
                    await Application.Current.MainPage.Navigation.PushAsync(new NewSurveyPage(testItem)); // Navigate to survey page
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error navigating to survey page: {ex.Message}");
                }
            }
        }

        // Notifies the UI of property changes
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
