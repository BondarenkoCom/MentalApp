﻿using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using SQLite;
using System.Linq;
using MentalTest.Interfaces;
using System.Windows.Input;
using MentalTest.Models;

namespace MentalTest.ViewModels
{
    public class SurveyViewModel : INotifyPropertyChanged
    {
        private readonly SQLiteConnection _database;
        public ObservableCollection<string> CurrentAnswers =>
          new ObservableCollection<string>(CurrentQuestion?.Answers.Split(';') ?? new string[0]);
        public ObservableCollection<QuestionModal> Questions { get; set; }
        public Command ContinueCommand { get; set; }
        public int CurrentQuestionIndex { get; set; }
        public QuestionModal CurrentQuestion => Questions.Count > CurrentQuestionIndex ? Questions[CurrentQuestionIndex] : null;
        public ICommand AnswerCommand { get; private set; }

        public ObservableCollection<string> SelectedAnswers { get; set; } = new ObservableCollection<string>();

        private string _selectedAnswer;

        public string SelectedAnswer
        {
            get => _selectedAnswer;
            set
            {
                if (_selectedAnswer != value)
                {
                    _selectedAnswer = value;
                    OnPropertyChanged(nameof(IsFirstAnswerSelected));
                    OnPropertyChanged(nameof(IsSecondAnswerSelected));
                    OnPropertyChanged(nameof(IsThirdAnswerSelected));
                    OnPropertyChanged(nameof(IsFourthAnswerSelected));
                }
            }
        }

        public SurveyViewModel(int testId)
        {
            try
            {
                var fileHelper = DependencyService.Get<IFileHelper>();
                var dbPath = fileHelper.GetLocalFilePath("MentalTestDB.db");
                AnswerCommand = new Command<string>(OnAnswerSelected);

                //ExportDatabase(@"D:\MentalAppSqlDb\MentalTestDB.db").GetAwaiter().GetResult();

                var databaseAssetService = DependencyService.Get<IDatabaseAssetService>();
                if (databaseAssetService == null)
                    throw new InvalidOperationException("Failed to retrieve IDatabaseAssetService");

                _database = new SQLiteConnection(dbPath);
                _database.CreateTable<QuestionModal>();
                _database.CreateTable<FinalAnswer>();

                Questions = new ObservableCollection<QuestionModal>();
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
            try
            {
                //_database.Execute("DROP TABLE IF EXISTS Questions");
                //Console.WriteLine("Попытка удаления таблицы Questions выполнена.");



                var info = _database.GetTableInfo("Questions");
                if (!info.Any())
                {
                  //  Console.WriteLine("Таблица Questions успешно удалена.");

                    _database.Execute(@"
                        CREATE TABLE Questions (
                            Id INTEGER NOT NULL PRIMARY KEY,
                            TestId INTEGER NOT NULL,
                            QuestionText TEXT NOT NULL,
                            Answers TEXT NOT NULL,
                            CorrectAnswerIndex INTEGER NOT NULL
                        )");
                }

                var existingQuestions = _database.Table<QuestionModal>().Where(q => q.TestId == testId).ToList();
                if (!existingQuestions.Any())
                {
                    //var questionsToInsert = new List<Question>
                    // {
                    //     new Question { Id = 1, TestId = 11, QuestionText = "What is the real name of Mercy?", Answers = "Angela Ziegler;Brigitte Lindholm;Fareeha Amari;Mei-Ling Zhou", CorrectAnswerIndex = 0 },
                    //     new Question { Id = 2, TestId = 11, QuestionText = "What role does Mercy play in a team composition?", Answers = "Support;Tank;Damage;Flex", CorrectAnswerIndex = 0 },
                    //     new Question { Id = 3, TestId = 11, QuestionText = "What is Mercy's ultimate ability called?", Answers = "Valkyrie;Resurrect;Guardian Angel;Caduceus", CorrectAnswerIndex = 0 },
                    //     new Question { Id = 4, TestId = 11, QuestionText = "How does Mercy heal her teammates?", Answers = "Caduceus Staff;Biotic Rifle;Repair Pack;Biotic Orb", CorrectAnswerIndex = 0 },
                    //     new Question { Id = 5, TestId = 11, QuestionText = "Which of these is not a Mercy skin?", Answers = "Witch;Devil;Archangel;Vampire", CorrectAnswerIndex = 3 }
                    // };

                    var questionsToInsert = new List<QuestionModal>
                    {
                        new QuestionModal { Id = 1, TestId = 13, QuestionText = "Какие технологии использует Motoko для улучшения своих боевых способностей?", Answers = "Кибернетические имплантаты;Магия;Биологическое усовершенствование;Химические стимуляторы", CorrectAnswerIndex = 0 },
                        new QuestionModal { Id = 2, TestId = 13, QuestionText = "Какова философия Motoko относительно человеческого сознания?", Answers = "Оно уникально;Оно может быть цифровым;Оно второстепенно;Оно неизменно", CorrectAnswerIndex = 1 },
                        new QuestionModal { Id = 3, TestId = 13, QuestionText = "Как Motoko относится к своему кибернетическому телу?", Answers = "Как к инструменту;Как к проклятию;Как к подарку;Как к тюрьме", CorrectAnswerIndex = 0 },
                        new QuestionModal { Id = 4, TestId = 13, QuestionText = "Какие межличностные отношения у Motoko в Section 9?", Answers = "Строго профессиональные;Дружеские;Романтические;Враждебные", CorrectAnswerIndex = 1 },
                        new QuestionModal { Id = 5, TestId = 13, QuestionText = "Какова главная цель Motoko в ее борьбе?", Answers = "Защита личных данных;Разгадка корпоративных заговоров;Борьба за права киборгов;Разоблачение государственной коррупции", CorrectAnswerIndex = 3 }
                    };
                


                    foreach (var question in questionsToInsert)
                    {
                        _database.Insert(question);
                    }
                }

                Questions = new ObservableCollection<QuestionModal>(_database.Table<QuestionModal>().Where(q => q.TestId == testId).ToList());
                LoadFinalAnswers(testId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in LoadQuestions: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }
        private void LoadFinalAnswers(int testId)
        {
            string createFinalAnswersTableQuery = @"
                CREATE TABLE IF NOT EXISTS FinalAnswers (
                    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    TestId INTEGER NOT NULL,
                    ResultText TEXT NOT NULL,
                    ScoreRange TEXT NOT NULL
                )";
            _database.Execute(createFinalAnswersTableQuery);

            var existingFinalAnswers = _database.Table<FinalAnswer>().Where(fa => fa.TestId == testId).ToList();
            if (!existingFinalAnswers.Any())
            {
                //var finalAnswersToInsert = new List<FinalAnswer>
                //{
                //    new FinalAnswer { TestId = 11, ResultText = "Вы настоящий ангел на поле боя, как Mercy.", ScoreRange = "High" },
                //    new FinalAnswer { TestId = 11, ResultText = "Ваша забота и поддержка напоминают о Mercy.", ScoreRange = "Medium" },
                //    new FinalAnswer { TestId = 11, ResultText = "Да ты вообще не должен играть за Mercy, лузер.", ScoreRange = "Low" },
                //
                //};
                var finalAnswersToInsert = new List<FinalAnswer>
                {
                    new FinalAnswer { TestId = 13, ResultText = "Вы настоящий стратег и мыслитель, в духе Motoko Kusanagi.", ScoreRange = "High" },
                    new FinalAnswer { TestId = 13, ResultText = "Вы показали глубокое понимание сложных вопросов, как это делает Motoko.", ScoreRange = "Medium" },
                    new FinalAnswer { TestId = 13, ResultText = "Похоже, вам еще предстоит изучить мир Motoko Kusanagi.", ScoreRange = "Low" }
                };
                foreach (var finalAnswer in finalAnswersToInsert)
                {
                    _database.Insert(finalAnswer);
                }
            }
        }
        private void OnContinue()
        {
            if (SelectedAnswer != null)
            {
                SelectedAnswers.Add(SelectedAnswer);
                OnPropertyChanged(nameof(SelectedAnswers));
            }

            if (CurrentQuestionIndex < Questions.Count - 1)
            {
                CurrentQuestionIndex++;
                SelectedAnswer = null;
                OnPropertyChanged(nameof(CurrentQuestion));
                OnPropertyChanged(nameof(CurrentAnswers));

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
            // TODO: Необходимо изменить логику подсчёта правильных ответов, чтобы обрабатывать непоследовательные Id вопросов.
            // Рекомендуется использовать Dictionary<int, string> для SelectedAnswers вместо ObservableCollection<string>.
            var results = new Dictionary<string, int>();
            foreach (var answer in SelectedAnswers)
            {
                if (!results.ContainsKey(answer))
                    results[answer] = 0;
                results[answer]++;
            }

            var totalAnswers = Questions.Count;
            var correctAnswersCount = Questions.Where(q => SelectedAnswers[q.Id - 1] == q.Answers.Split(';')[q.CorrectAnswerIndex]).Count();

            var percentage = (correctAnswersCount / (float)totalAnswers) * 100;

            string scoreRange = percentage >= 90 ? "High" : percentage >= 50 ? "Medium" : "Low";

            var finalResult = _database.Table<FinalAnswer>()
                                .FirstOrDefault(fa => fa.TestId == CurrentQuestion.TestId && fa.ScoreRange == scoreRange)?.ResultText;

            string finalResultMessage = finalResult ?? "Result is not found.";
            MessagingCenter.Send(this, "FinishTest", finalResultMessage);
        }





        public bool IsFirstAnswerSelected => SelectedAnswer == CurrentAnswers[0];
        public bool IsSecondAnswerSelected => SelectedAnswer == CurrentAnswers[1];
        public bool IsThirdAnswerSelected => SelectedAnswer == CurrentAnswers[2];
        public bool IsFourthAnswerSelected => SelectedAnswer == CurrentAnswers[3];

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnAnswerSelected(string answer)
        {
            if (!string.IsNullOrEmpty(answer))
            {
                SelectedAnswer = answer;
            }
        }
    }

    public class FinalAnswer
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [NotNull]
        public int TestId { get; set; }
        [NotNull]
        public string ResultText { get; set; }
        [NotNull]
        public string ScoreRange { get; set; }
    }
}