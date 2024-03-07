using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MentalTest.Models;
using MentalTest.ViewModels;
using Newtonsoft.Json;

namespace MentalTest.Service
{
    public class ApiService
    {
        private HttpClient _client;
        private const string BaseUrl = "http://192.168.0.104:7098/api/TestItem/";
        //http://192.168.0.104:7098
        // TODO: Parse API response to model objects
        // TODO: Store parsed data in application memory (consider using a singleton or static class for data storage)
        // TODO: Display stored data in XAML views (use data binding to link your view model with your XAML UI elements)
        // TODO: Implement data refresh logic to check for new data in the database and update local storage and UI accordingly

        public ApiService()
        {
            _client = new HttpClient();
            // Configure the client (e.g., timeout, headers) here if needed
        }

        public async Task<List<TestCardModel>> GetTestItemsByCategoryAsync(string category)
        {
            try
            {
                Console.WriteLine("Initiating request to API...");
                Console.WriteLine($"chosed category - {category}");
        
                var json = await _client.GetStringAsync($"{BaseUrl}testcards/category/{category}");
                Console.WriteLine("Response from API received.");
                Console.WriteLine($"Response JSON: {json}"); // Outputting the received JSON to the console
        
                var testItems = JsonConvert.DeserializeObject<List<TestCardModel>>(json);
                DataStore.Instance.SaveTestItems(testItems);
                Console.WriteLine($"Deserialized {testItems.Count} testCard objects."); // Outputting the number of objects after deserialization
                return testItems;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during API request: {ex.Message}");
                throw;
            }
        }
        //TODO понять что не так с этим методом, кажется что он пытается подцепить вопросы с testid 14  и запросить их, и поле этого валится все.ну карточки пустые
        //public async Task<List<TestCardModel>> GetTestItemsByCategoryAsync(string category)
        //{
        //    try
        //    {
        //        Console.WriteLine("Initiating request to API...");
        //        Console.WriteLine($"Chosen category - {category}");
        //
        //        var json = await _client.GetStringAsync($"{BaseUrl}testcards/category/{category}");
        //        Console.WriteLine("Response from API received.");
        //        Console.WriteLine($"Response JSON: {json}");
        //
        //        var testItems = JsonConvert.DeserializeObject<List<TestCardModel>>(json);
        //        foreach (var testItem in testItems)
        //        {
        //            var questions = await GetQuestionsByTestIdAsync(testItem.id);
        //            DataStore.Instance.SaveQuestionsForTest(questions, testItem.id);
        //        }
        //        DataStore.Instance.SaveTestItems(testItems);
        //
        //        Console.WriteLine("Test items stored in DataStore:");
        //        foreach (var testItem in DataStore.Instance.TestItems)
        //        {
        //            Console.WriteLine($"ID: {testItem.id}, Title: {testItem.title}, Description: {testItem.description}");
        //        }
        //
        //        Console.WriteLine("Questions for each test stored in DataStore:");
        //        foreach (var kvp in DataStore.Instance.TestQuestions)
        //        {
        //            Console.WriteLine($"Test ID: {kvp.Key}");
        //            foreach (var question in kvp.Value)
        //            {
        //                Console.WriteLine($"Question ID: {question.Id}, Text: {question.QuestionText}");
        //            }
        //        }
        //
        //        Console.WriteLine($"Deserialized {testItems.Count} testCard objects.");
        //        return testItems;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error during API request: {ex.Message}");
        //        throw;
        //    }
        //}



        public async Task<List<QuestionModal>> GetQuestionsByTestIdAsync(int testId)
        {
            try
            {
                Console.WriteLine($"Fetching questions for test ID: {testId}");

                var json = await _client.GetStringAsync($"{BaseUrl}questions/testid/{testId}");
                Console.WriteLine($"Response from API for questions: {json}");

                var questions = JsonConvert.DeserializeObject<List<QuestionModal>>(json);

                DataStore.Instance.SaveQuestionsForTest(questions, testId);
                return questions;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching questions from API: {ex.Message}");
                throw;
            }
        }


    }
}
