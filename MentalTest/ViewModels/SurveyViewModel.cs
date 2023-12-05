using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using SQLite;
using System.Linq;
using MentalTest.Interfaces;

namespace MentalTest.ViewModels
{
    public class SurveyViewModel : INotifyPropertyChanged
    {
        private readonly SQLiteConnection _database;

        public bool IsFirstAnswerSelected => SelectedAnswer == AnswerFirst;
        public bool IsSecondAnswerSelected => SelectedAnswer == AnswerSecond;
        public bool IsThirdAnswerSelected => SelectedAnswer == AnswerThird;
        public bool IsFourthAnswerSelected => SelectedAnswer == AnswerFourth;

        public ObservableCollection<Question> Questions { get; set; }
        public Command<string> AnswerCommand { get; set; }
        public Command ContinueCommand { get; set; }
        public int CurrentQuestionIndex { get; set; }
        public Question CurrentQuestion => Questions.Count > CurrentQuestionIndex ? Questions[CurrentQuestionIndex] : null;

        public ObservableCollection<string> SelectedAnswers { get; set; } = new ObservableCollection<string>();

        public string AnswerFirst => CurrentQuestion?.Answers.Split(';').ElementAtOrDefault(0);
        public string AnswerSecond => CurrentQuestion?.Answers.Split(';').ElementAtOrDefault(1);
        public string AnswerThird => CurrentQuestion?.Answers.Split(';').ElementAtOrDefault(2);
        public string AnswerFourth => CurrentQuestion?.Answers.Split(';').ElementAtOrDefault(3);

        private string _selectedAnswer;
        public string SelectedAnswer
        {
            get => _selectedAnswer;
            set
            {
                if (_selectedAnswer != value)
                {
                    _selectedAnswer = value;
                    OnPropertyChanged(nameof(SelectedAnswer));
                }
            }
        }

        public SurveyViewModel(int testId)
        {
            try
            {
                var fileHelper = DependencyService.Get<IFileHelper>();
                var dbPath = fileHelper.GetLocalFilePath("MentalTestDB.db");

                var databaseAssetService = DependencyService.Get<IDatabaseAssetService>();
                if (databaseAssetService == null)
                    throw new InvalidOperationException("Failed to retrieve IDatabaseAssetService");

                _database = new SQLiteConnection(dbPath);
                _database.CreateTable<Question>();

                Questions = new ObservableCollection<Question>();
                AnswerCommand = new Command<string>(OnAnswerSelected);
                ContinueCommand = new Command(OnContinue);
                LoadQuestions(testId);
                CurrentQuestionIndex = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred in the constructor of SurveyViewModel: {ex.Message}");
                Console.WriteLine($"Call stack: {ex.StackTrace}");
                throw;
            }
        }


        private void LoadQuestions(int testId)
        {
            string createTableQuery = @"
        CREATE TABLE IF NOT EXISTS Questions (
            Id INTEGER NOT NULL PRIMARY KEY,
            TestId INTEGER NOT NULL,
            QuestionText TEXT NOT NULL,
            Answers TEXT NOT NULL
        )";

            _database.Execute(createTableQuery);


            var existingQuestions = _database.Table<Question>().Where(q => q.TestId == testId).ToList();
            if (!existingQuestions.Any())
            {
                var questionsToInsert = new List<Question>
        {
            new Question { Id = 1, TestId = 11, QuestionText = "What is the real name of Mercy?", Answers = "Angela Ziegler;Brigitte Lindholm;Fareeha Amari;Mei-Ling Zhou" },
            new Question { Id = 2, TestId = 11, QuestionText = "What role does Mercy play in a team composition?", Answers = "Support;Tank;Damage;Flex" },
            new Question { Id = 3, TestId = 11, QuestionText = "What is Mercy's ultimate ability called?", Answers = "Valkyrie;Resurrect;Guardian Angel;Caduceus" },
            new Question { Id = 4, TestId = 11, QuestionText = "How does Mercy heal her teammates?", Answers = "Caduceus Staff;Biotic Rifle;Repair Pack;Biotic Orb" },
            new Question { Id = 5, TestId = 11, QuestionText = "Which of these is not a Mercy skin?", Answers = "Witch;Devil;Archangel;Vampire" }
        };
                foreach (var question in questionsToInsert)
                {
                    _database.Insert(question);
                }
            }
            Questions = new ObservableCollection<Question>(_database.Table<Question>().Where(q => q.TestId == testId).ToList());
        }

        private void OnContinue()
        {
            if (CurrentQuestionIndex < Questions.Count - 1)
            {
                CurrentQuestionIndex++;
                SelectedAnswer = null;
        
                OnPropertyChanged(nameof(CurrentQuestion));
                OnPropertyChanged(nameof(AnswerFirst));
                OnPropertyChanged(nameof(AnswerSecond));
                OnPropertyChanged(nameof(AnswerThird));
                OnPropertyChanged(nameof(AnswerFourth));
        
                OnPropertyChanged(nameof(IsFirstAnswerSelected));
                OnPropertyChanged(nameof(IsSecondAnswerSelected));
                OnPropertyChanged(nameof(IsThirdAnswerSelected));
                OnPropertyChanged(nameof(IsFourthAnswerSelected));
            }
            else
            {
                FinishTest();
            }
        }



        private void FinishTest()
        {
            //TODO  need make a final page and logic for check result
            int scoreDva = 0;
            int scoreMercy = 0;
            foreach (var answer in SelectedAnswers)
            {
                if (answer == "Dva") scoreDva++;
                if (answer == "Mercy") scoreMercy++;
            }

            string finalResult;
            if (scoreDva > scoreMercy)
                finalResult = "Вы больше подходите для персонажа Dva.";
            else if (scoreMercy > scoreDva)
                finalResult = "Вы больше подходите для персонажа Mercy.";
            else
                finalResult = "Вы одинаково хорошо подходите для обоих персонажей.";

            MessagingCenter.Send(this, "FinishTest", finalResult);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnAnswerSelected(string answer)
        {
            if (!string.IsNullOrEmpty(answer))
            {
                SelectedAnswer = answer;
                OnPropertyChanged(nameof(IsFirstAnswerSelected));
                OnPropertyChanged(nameof(IsSecondAnswerSelected));
                OnPropertyChanged(nameof(IsThirdAnswerSelected));
                OnPropertyChanged(nameof(IsFourthAnswerSelected));
            }
        }
    }

    public class Question
    {
        [PrimaryKey, NotNull]
        public int Id { get; set; }
        [NotNull]
        public int TestId { get; set; }
        [NotNull]
        public string QuestionText { get; set; }
        [NotNull]
        public string Answers { get; set; }
    }
}