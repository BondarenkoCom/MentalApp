using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using SQLite;
using MentalTest.Interfaces;
using System.IO;
using System;
using System.Threading.Tasks;
using MentalTest.Views;

namespace MentalTest.ViewModels
{
    public class TestsPageViewModel
    {
        private readonly SQLiteConnection _database;
        public ObservableCollection<TestItem> TestItems { get; set; }
        public string SelectedCategory { get; set; }
        public Command<TestItem> ItemTappedCommand { get; }

        public TestsPageViewModel(string categoryName)
        {
            ItemTappedCommand = new Command<TestItem>(async (testItem) => await OnTestItemTapped(testItem));
            try
            {
                var dbName = "MentalTestDB.db";
                var dbPath = DependencyService.Get<IFileHelper>().GetLocalFilePath(dbName);
                Console.WriteLine($"Database path: {dbPath}");

                var databaseAssetService = DependencyService.Get<IDatabaseAssetService>();
                if (databaseAssetService == null)
                    throw new InvalidOperationException("Failed to retrieve IDatabaseAssetService");

                databaseAssetService.CopyDatabaseIfNotExists(dbName, dbPath);

                _database = new SQLiteConnection(dbPath);

                _database.CreateTable<TestItem>();
                SeedDatabaseWithJobTests();

                Console.WriteLine($"Database path: {dbPath}");

                SelectedCategory = categoryName ?? "default category";
                Console.WriteLine($"Selected category: {SelectedCategory}");

                LoadTestsByCategory();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred in the constructor of TestsPageViewModel: {ex.Message}");
                Console.WriteLine($"Call stack: {ex.StackTrace}");
                throw;
            }
            
        }

        private void SeedDatabaseWithJobTests()
        {
            var newTestItems = new List<TestItem>
            {
                //new TestItem { Title = "Mutant Workload Assessment", Description = "Analyzes Jean Grey's potential fatigue from her extensive commitments to the X-Men team and her psychic responsibilities.", QuestionsStatus = "2/3 questions", IsStarred = false, Category = "Job" },
                //new TestItem { Title = "Cybernetic Operative Downtime Analysis", Description = "Evaluates Motoko Kusanagi's need for rest and recuperation from her demanding role in Section 9.", QuestionsStatus = "0/5 questions", IsStarred = false, Category = "Job" },
                  new TestItem {
                      Id = 11,
                      Title = "How well do you know Mercy?",
                      Description = "Test your knowledge and skills about Mercy from Overwatch.",
                      QuestionsStatus = "0/5 questions",
                      IsStarred = false,
                      Category = "Job" },
            };

            /*
            foreach (var newItem in newTestItems)
            {
                var existingItem = _database.Table<TestItem>().FirstOrDefault(test => test.Title == newItem.Title && test.Category == newItem.Category);
                if (existingItem == null)
                {
                    _database.Insert(newItem);
                    Console.WriteLine($"New test '{newItem.Title}' added to category '{newItem.Category}'.");
                }
                else
                {
                    Console.WriteLine($"A test with the name '{newItem.Title}' already exists in category '{newItem.Category}', no addition necessary.");
                }
            }
             */

            foreach (var newItem in newTestItems)
            {
                var existingItem = _database.Table<TestItem>().FirstOrDefault(test => test.Id == newItem.Id);
                if (existingItem == null)
                {
                    _database.Insert(newItem);
                    Console.WriteLine($"New test '{newItem.Title}' with Id '{newItem.Id}' added to category '{newItem.Category}'.");
                }
                else
                {
                    Console.WriteLine($"A test with the Id '{newItem.Id}' already exists in category '{newItem.Category}', no addition necessary.");
                }
            }
        }


        private void LoadTestsByCategory()
        {
            try
            {
                Console.WriteLine($"Attempting to load tests for category: {SelectedCategory}");

                var testQuery = _database.Table<TestItem>().Where(test => test.Category == SelectedCategory);
                var filteredTests = testQuery.ToList();
                Console.WriteLine($"Query executed. Number of tests retrieved: {filteredTests.Count}");

                if (filteredTests.Any())
                {
                    Console.WriteLine($"Found {filteredTests.Count} tests for category: {SelectedCategory}");
                    foreach (var test in filteredTests)
                    {
                        Console.WriteLine($"Test ID: {test.Id}, Title: {test.Title}, Description: {test.Description}");
                    }
                }
                else
                {
                    Console.WriteLine($"No tests found for category: {SelectedCategory}");
                }

                TestItems = new ObservableCollection<TestItem>(filteredTests);
                Console.WriteLine($"Observable collection initialized with {TestItems.Count} items.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while loading tests by category: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }

        private async Task OnTestItemTapped(TestItem testItem)
        {
            if (testItem != null)
            {
                // Выполняем навигацию к SurveyPage, передавая testItem как параметр
                await Application.Current.MainPage.Navigation.PushAsync(new SurveyPage(testItem));
            }
        }

    }

    public class TestItem
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string QuestionsStatus { get; set; }
        public bool IsStarred { get; set; }
        public string Category { get; set; }  // New field
    }
}