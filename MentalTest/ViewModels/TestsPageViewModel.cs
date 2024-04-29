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
    //Test description page
    public class TestsPageViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly SQLiteConnection _database;
        private ApiService _apiService = new ApiService();
        private string _debugDataStoreInfo;
        private ObservableCollection<TestCardModel> _testItems = new ObservableCollection<TestCardModel>();

        private bool _isDataLoading;
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

        public string DebugDataStoreInfo
        {
            get => _debugDataStoreInfo;
            set
            {
                _debugDataStoreInfo = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TestCardModel> TestItems
        {
            get => _testItems;
            set
            {
                _testItems = value;
                OnPropertyChanged();
            }
        }


        public bool IsDataLoaded => !IsDataLoading;
        public string SelectedCategory { get; set; }
        public Command<TestCardModel> ItemTappedCommand { get; }

        public TestsPageViewModel(string categoryName)
        {
            SelectedCategory = categoryName;
            ItemTappedCommand = new Command<TestCardModel>(async (testItem) => await OnTestItemTapped(testItem));
        }

        public async Task InitializeDataAsync()
        {
            IsDataLoading = true;
            try
            {
                await FetchDataFromApiAndSaveAsync(SelectedCategory);
                LoadDataFromDataStore();
            }
            finally
            {
                IsDataLoading = false;
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
           PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private async Task FetchDataFromApiAndSaveAsync(string category)
        {
            try
            {
                var testItems = await _apiService.GetTestItemsByCategoryAsync(category);
                if (testItems != null && testItems.Any())
                {
                    DataStore.Instance.SaveTestItems(testItems);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data from API: {ex.Message}");
            }
        }

        private void LoadDataFromDataStore()
        {
            var items = DataStore.Instance.TestItems;
            if (items != null && items.Any())
            {
                TestItems.Clear();
                foreach (var item in items)
                {
                    TestItems.Add(item);
                }

                DebugDataStoreInfo = $"Loaded items: {TestItems.Count}";
                OnPropertyChanged(nameof(DebugDataStoreInfo));
            }
        }

        private async Task OnTestItemTapped(TestCardModel testItem)
        {
            if (testItem != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(new SurveyPage(testItem));
            }
        }
    }
}