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
        private const string BaseUrl = "http://192.168.0.103:7098/api/TestItem/";
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
    }
}
