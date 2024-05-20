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
        private const string BaseUrl = "http://167.71.109.199:5000/api/TestItem/";
        //http://192.168.0.104:5003
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
                Console.WriteLine($"Chosen category - {category}");

                var json = await _client.GetStringAsync($"{BaseUrl}testcards/category/{category}");
                Console.WriteLine("Response from API received.");
                Console.WriteLine($"Response JSON: {json}");

                var testItems = JsonConvert.DeserializeObject<List<TestCardModel>>(json);
                DataStore.Instance.SaveTestItems(testItems);
                Console.WriteLine($"Deserialized {testItems.Count} testCard objects and saved in DataStore.");

                return testItems;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during API request: {ex.Message}");
                throw;
            }
        }

        public async Task<List<QuestionModal>> GetQuestionsByTestIdAsync(int testId)
        {
            try
            {
                Console.WriteLine($"Fetching questions for test ID: {testId}");
                var response = await _client.GetAsync($"{BaseUrl}questions/testid/{testId}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response from API for questions: {json}");

                    var questions = JsonConvert.DeserializeObject<List<QuestionModal>>(json);
                    Console.WriteLine($"Successfully deserialized {questions.Count} questions for test ID {testId}.");

                    DataStore.Instance.SaveQuestionsForTest(questions, testId);
                    Console.WriteLine("Questions saved to DataStore.");

                    return questions;
                }
                else
                {
                    Console.WriteLine($"API call failed: {response.StatusCode}");
                    throw new HttpRequestException($"Request to API failed with status code {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching questions from API: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        public async Task<List<FinalAnswer>> GetFinalAnswersByTestIdAsync(int testId)
        {
            try
            {
                Console.WriteLine($"Fetching final answers for test ID: {testId}");
                var response = await _client.GetAsync($"{BaseUrl}finalanswers/{testId}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response from API for final answers: {json}");

                    var finalAnswers = JsonConvert.DeserializeObject<List<FinalAnswer>>(json);
                    Console.WriteLine($"Successfully deserialized {finalAnswers.Count} final answers for test ID {testId}.");

                    return finalAnswers;
                }
                else
                {
                    Console.WriteLine($"API call failed: {response.StatusCode}");
                    throw new HttpRequestException($"Request to API failed with status code {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching final answers from API: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }


        public void ClearDataStore()
        {
            DataStore.Instance.SaveTestItems(new List<TestCardModel>());

            var testIds = new List<int>(DataStore.Instance.TestQuestions.Keys);
            foreach (var testId in testIds)
            {
                DataStore.Instance.SaveQuestionsForTest(new List<QuestionModal>(), testId);
            }

            Console.WriteLine("DataStore has been cleared.");
        }

    }
}
