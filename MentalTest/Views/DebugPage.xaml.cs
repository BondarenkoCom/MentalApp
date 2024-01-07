using MentalTest.Interfaces;
using MentalTest.ViewModels;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace MentalTest.Views
{
    public partial class DebugPage : ContentPage
    {
        private SQLiteConnection _database;

        public DebugPage()
        {
            InitializeComponent();
            var fileHelper = DependencyService.Get<IFileHelper>();
            var dbPath = fileHelper.GetLocalFilePath("MentalTestDB.db");
            _database = new SQLiteConnection(dbPath);

            // Инициализация элементов выпадающего списка
            TablePicker.ItemsSource = new List<string> { "Questions", "FinalAnswers", "TestItems" };
            TablePicker.SelectedIndexChanged += OnTablePickerSelected;
        }

        private void OnTablePickerSelected(object sender, EventArgs e)
        {
            switch (TablePicker.SelectedItem.ToString())
            {
                case "Questions":
                    LoadData<Question>();
                    break;
                case "FinalAnswers":
                    LoadData<FinalAnswer>();
                    break;
                case "TestItems":
                    LoadData<TestItem>();
                    break;
            }
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                // Обработка выбранного элемента
                // Например, сохраните выбранный элемент в переменную класса для дальнейшего использования
                // Или обновите интерфейс, чтобы показать опции для выбранного элемента
                // Пример: _selectedItem = e.SelectedItem;

                // Здесь код для обработки выбранного элемента
            }
        }

        private void OnEditClicked(object sender, EventArgs e)
        {
           
        }

        private void OnDeleteClicked(object sender, EventArgs e)
        {
            var item = (sender as Button)?.CommandParameter;

            if (item is Question question)
            {
                _database.Delete(question);
            }
            else if (item is FinalAnswer finalAnswer)
            {
                _database.Delete(finalAnswer);
            }
            else if (item is TestItem testItem)
            {
                _database.Delete(testItem);
            }

            OnTablePickerSelected(this, EventArgs.Empty);
        }


        private void OnSaveClicked(object sender, EventArgs e)
        { 
        
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {

        }

        private void LoadData<T>() where T : new()
        {
            var data = _database.Table<T>().ToList();
            DataListView.ItemsSource = data;

            if (typeof(T) == typeof(Question))
            {
                // Устанавливаем DataTemplate для Question
                DataListView.ItemTemplate = (DataTemplate)this.Resources["QuestionTemplate"];
            }
            else if (typeof(T) == typeof(FinalAnswer))
            {
                // Устанавливаем DataTemplate для FinalAnswer
                DataListView.ItemTemplate = (DataTemplate)this.Resources["FinalAnswerTemplate"];
            }
            else if (typeof(T) == typeof(TestItem))
            {
                // Устанавливаем DataTemplate для TestItem
                DataListView.ItemTemplate = (DataTemplate)this.Resources["TestItemTemplate"];
            }
        }

    }
}
