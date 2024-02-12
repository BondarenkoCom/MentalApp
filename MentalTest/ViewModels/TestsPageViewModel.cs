using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using SQLite;
using MentalTest.Interfaces;
using System;
using System.Threading.Tasks;
using MentalTest.Views;
using System.Net.Http;
using Newtonsoft.Json;
using MentalTest.Service;

namespace MentalTest.ViewModels
{
    public class TestsPageViewModel
    {
        private readonly SQLiteConnection _database;
        public ObservableCollection<testCard> TestItems { get; set; }
        public string SelectedCategory { get; set; }
        public Command<testCard> ItemTappedCommand { get; }
        private ApiService _apiService;


        //public TestsPageViewModel(string categoryName)
        //{
        //    ItemTappedCommand = new Command<testCard>(async (testItem) => await OnTestItemTapped(testItem));
        //    _apiService = new ApiService();
        //    LoadTestItemsAsync();
        //
        //}

        public TestsPageViewModel(string categoryName)
        {
            SelectedCategory = categoryName; // Добавьте эту строку для инициализации SelectedCategory
            ItemTappedCommand = new Command<testCard>(async (testItem) => await OnTestItemTapped(testItem));
            _apiService = new ApiService();
            LoadTestItemsAsync();
        }

        //            string apiUrl = "https://192.168.1.2:7098/api/testcards";


        private async void LoadTestItemsAsync()
        {
            var testItems = await _apiService.GetTestItemsByCategoryAsync(SelectedCategory); // Используйте свойство `SelectedCategory`
            TestItems = new ObservableCollection<testCard>(testItems);
        }

        private void LoadTestsByCategory()
        {
            try
            {
                Console.WriteLine($"Attempting to load tests for category: {SelectedCategory}");

                var testQuery = _database.Table<testCard>().Where(test => test.category == SelectedCategory);
                var filteredTests = testQuery.ToList();
                Console.WriteLine($"Query executed. Number of tests retrieved: {filteredTests.Count}");

                if (filteredTests.Any())
                {
                    Console.WriteLine($"Found {filteredTests.Count} tests for category: {SelectedCategory}");
                    foreach (var test in filteredTests)
                    {
                        Console.WriteLine($"Test ID: {test.id}, Title: {test.title}, Description: {test.description}");
                    }
                }
                else
                {
                    Console.WriteLine($"No tests found for category: {SelectedCategory}");
                }

                TestItems = new ObservableCollection<testCard>(filteredTests);
                Console.WriteLine($"Observable collection initialized with {TestItems.Count} items.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while loading tests by category: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }

        private async Task OnTestItemTapped(testCard testItem)
        {
            if (testItem != null)
            {
                await Application.Current.MainPage.Navigation.PushAsync(new SurveyPage(testItem));
            }
        }   

    }

    public class testCard
    {
        [PrimaryKey]
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string questions_status { get; set; }
        public bool is_starred { get; set; }
        public string category { get; set; }
    }
}