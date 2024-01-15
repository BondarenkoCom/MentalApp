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
        private object _selectedItem; 

        public DebugPage()
        {
            InitializeComponent();
            var fileHelper = DependencyService.Get<IFileHelper>();
            var dbPath = fileHelper.GetLocalFilePath("MentalTestDB.db");
            _database = new SQLiteConnection(dbPath);

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
               
            }
        }

        private void OnEditClicked(object sender, EventArgs e)
        {
            var item = (sender as Button)?.CommandParameter;
            _selectedItem = item;

            EditLayout.IsVisible = true;

            if (item is Question question)
            {
                EditEntry.Text = question.QuestionText;
            }
            else if (item is FinalAnswer finalAnswer)
            {
                EditEntry.Text = finalAnswer.ResultText;
            }
            else if (item is TestItem testItem)
            {
                EditEntry.Text = testItem.Title;
            }
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
            if (_selectedItem == null) return;

            if (_selectedItem is Question question)
            {
                question.QuestionText = EditEntry.Text;
                _database.Update(question);
            }
            else if (_selectedItem is FinalAnswer finalAnswer)
            {
                finalAnswer.ResultText = EditEntry.Text;
                _database.Update(finalAnswer);
            }
            else if (_selectedItem is TestItem testItem)
            {
                testItem.Title = EditEntry.Text;
                _database.Update(testItem);
            }

            EditLayout.IsVisible = false;
            _selectedItem = null;

            OnTablePickerSelected(this, EventArgs.Empty);
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            EditLayout.IsVisible = false;
            _selectedItem = null;
        }

        private void LoadData<T>() where T : new()
        {
            var data = _database.Table<T>().ToList();
            DataListView.ItemsSource = data;

            if (typeof(T) == typeof(Question))
            {
                DataListView.ItemTemplate = (DataTemplate)this.Resources["QuestionTemplate"];
            }
            else if (typeof(T) == typeof(FinalAnswer))
            {
                DataListView.ItemTemplate = (DataTemplate)this.Resources["FinalAnswerTemplate"];
            }
            else if (typeof(T) == typeof(TestItem))
            {
                DataListView.ItemTemplate = (DataTemplate)this.Resources["TestItemTemplate"];
            }
        }

    }
}
