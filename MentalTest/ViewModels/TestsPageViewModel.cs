using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using SQLite;
using MentalTest.Interfaces;
using System.IO;
using System;

namespace MentalTest.ViewModels
{
    public class TestsPageViewModel
    {
        private readonly SQLiteConnection _database;
        public ObservableCollection<TestItem> TestItems { get; set; }
        public string SelectedCategory { get; set; }

        public TestsPageViewModel(string categoryName)
        {
            try
            {
                var dbName = "MentalTestDB.db";
                var dbPath = DependencyService.Get<IFileHelper>().GetLocalFilePath(dbName);
                Console.WriteLine($"Database path: {dbPath}");
                // Copying the database if it doesn't exist
                var databaseAssetService = DependencyService.Get<IDatabaseAssetService>();
                if (databaseAssetService == null)
                    throw new InvalidOperationException("Failed to retrieve IDatabaseAssetService");

                databaseAssetService.CopyDatabaseIfNotExists(dbName, dbPath);

                // Initializing the database connection
                _database = new SQLiteConnection(dbPath);

                // Creating the table if it doesn't exist
                _database.CreateTable<TestItem>();
                SeedDatabaseWithJobTests();

                Console.WriteLine($"Database path: {dbPath}");

                SelectedCategory = categoryName ?? "default category";
                Console.WriteLine($"Selected category: {SelectedCategory}");

                // Loading tests by category and printing to console
                LoadTestsByCategory();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred in the constructor of TestsPageViewModel: {ex.Message}");
                Console.WriteLine($"Call stack: {ex.StackTrace}");
                throw; // Rethrow the exception to preserve the original call stack.
            }
        }

        private void SeedDatabaseWithJobTests()
        {
            // List of new tests we want to add
            var newTestItems = new List<TestItem>
            {
                // More tests can be added here
                new TestItem { Title = "Mutant Workload Assessment", Description = "Analyzes Jean Grey's potential fatigue from her extensive commitments to the X-Men team and her psychic responsibilities.", QuestionsStatus = "2/3 questions", IsStarred = false, Category = "Job" },
                new TestItem { Title = "Cybernetic Operative Downtime Analysis", Description = "Evaluates Motoko Kusanagi's need for rest and recuperation from her demanding role in Section 9.", QuestionsStatus = "0/5 questions", IsStarred = false, Category = "Job" },
            };

            foreach (var newItem in newTestItems)
            {
                // Checking if a test with the same name already exists in this category
                var existingItem = _database.Table<TestItem>().FirstOrDefault(test => test.Title == newItem.Title && test.Category == newItem.Category);
                if (existingItem == null)
                {
                    // If it doesn't exist, add the new test to the database
                    _database.Insert(newItem);
                    Console.WriteLine($"New test '{newItem.Title}' added to category '{newItem.Category}'.");
                }
                else
                {
                    Console.WriteLine($"A test with the name '{newItem.Title}' already exists in category '{newItem.Category}', no addition necessary.");
                }
            }
        }


        private void LoadTestsByCategory()
        {
            try
            {
                Console.WriteLine($"Attempting to load tests for category: {SelectedCategory}");

                // Fetch tests from the database and convert to a list
                var testQuery = _database.Table<TestItem>().Where(test => test.Category == SelectedCategory);
                var filteredTests = testQuery.ToList();
                Console.WriteLine($"Query executed. Number of tests retrieved: {filteredTests.Count}");

                // Check if tests exist for the category
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

                // Initialize the observable collection with the retrieved tests
                TestItems = new ObservableCollection<TestItem>(filteredTests);
                Console.WriteLine($"Observable collection initialized with {TestItems.Count} items.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while loading tests by category: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }
    }

    public class TestItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string QuestionsStatus { get; set; }
        public bool IsStarred { get; set; }
        public string Category { get; set; }  // New field
    }
}
