using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using SQLite;
using System.Linq;
using MentalTest.Interfaces;
using System.Threading.Tasks;

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

                //ExportDatabase(@"D:\MentalAppSqlDb\MentalTestDB.db").GetAwaiter().GetResult();


                var databaseAssetService = DependencyService.Get<IDatabaseAssetService>();
                if (databaseAssetService == null)
                    throw new InvalidOperationException("Failed to retrieve IDatabaseAssetService");
                //ERROR in here
                _database = new SQLiteConnection(dbPath);
                _database.CreateTable<Question>();
                _database.CreateTable<FinalAnswer>();

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
            try
            {
                //_database.Execute("DROP TABLE IF EXISTS Questions");

                var info = _database.GetTableInfo("Questions");
                if (!info.Any())
                {
                    // Если таблица не существует, создаем ее со всеми необходимыми столбцами
                    _database.Execute(@"
                        CREATE TABLE Questions (
                            Id INTEGER NOT NULL PRIMARY KEY,
                            TestId INTEGER NOT NULL,
                            QuestionText TEXT NOT NULL,
                            Answers TEXT NOT NULL,
                            CorrectAnswerIndex INTEGER NOT NULL
                        )");
                }

                // Загрузка существующих вопросов или вставка новых, если они не существуют
                var existingQuestions = _database.Table<Question>().Where(q => q.TestId == testId).ToList();
                if (!existingQuestions.Any())
                {
                    var questionsToInsert = new List<Question>
                     {
            new Question { Id = 1, TestId = 11, QuestionText = "What is the real name of Mercy?", Answers = "Angela Ziegler;Brigitte Lindholm;Fareeha Amari;Mei-Ling Zhou", CorrectAnswerIndex = 0 },
            new Question { Id = 2, TestId = 11, QuestionText = "What role does Mercy play in a team composition?", Answers = "Support;Tank;Damage;Flex", CorrectAnswerIndex = 0 },
            new Question { Id = 3, TestId = 11, QuestionText = "What is Mercy's ultimate ability called?", Answers = "Valkyrie;Resurrect;Guardian Angel;Caduceus", CorrectAnswerIndex = 0 },
            new Question { Id = 4, TestId = 11, QuestionText = "How does Mercy heal her teammates?", Answers = "Caduceus Staff;Biotic Rifle;Repair Pack;Biotic Orb", CorrectAnswerIndex = 0 },
            new Question { Id = 5, TestId = 11, QuestionText = "Which of these is not a Mercy skin?", Answers = "Witch;Devil;Archangel;Vampire", CorrectAnswerIndex = 3 }
        };
                    foreach (var question in questionsToInsert)
                    {
                        _database.Insert(question);
                    }
                }

                // Загрузка вопросов в ObservableCollection
                Questions = new ObservableCollection<Question>(_database.Table<Question>().Where(q => q.TestId == testId).ToList());
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
            // Создание таблицы для итоговых ответов, если она еще не существует
            string createFinalAnswersTableQuery = @"
                CREATE TABLE IF NOT EXISTS FinalAnswers (
                    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    TestId INTEGER NOT NULL,
                    ResultText TEXT NOT NULL,
                    ScoreRange TEXT NOT NULL
                )";
            _database.Execute(createFinalAnswersTableQuery);

            // Проверка наличия итоговых ответов для данного теста
            var existingFinalAnswers = _database.Table<FinalAnswer>().Where(fa => fa.TestId == testId).ToList();
            if (!existingFinalAnswers.Any())
            {
                var finalAnswersToInsert = new List<FinalAnswer>
        {
            new FinalAnswer { TestId = 11, ResultText = "Вы настоящий ангел на поле боя, как Mercy.", ScoreRange = "High" },
            new FinalAnswer { TestId = 11, ResultText = "Ваша забота и поддержка напоминают о Mercy.", ScoreRange = "Medium" },
            new FinalAnswer { TestId = 11, ResultText = "Да ты вообще не должен играть за Mercy, лузер.", ScoreRange = "Low" },

        };
                foreach (var finalAnswer in finalAnswersToInsert)
                {
                    _database.Insert(finalAnswer);
                }
            }
        }

        private void OnContinue()
        {
            if (CurrentQuestionIndex < Questions.Count - 1)
            {
                // Проверка, что ответ был выбран перед сохранением
                if (SelectedAnswer != null)
                {
                    SelectedAnswers.Add(SelectedAnswer);
                }

                CurrentQuestionIndex++;
                SelectedAnswer = null;

                // Обновление свойств для следующего вопроса
                OnPropertyChanged(nameof(CurrentQuestion));
                OnPropertyChanged(nameof(AnswerFirst));
                OnPropertyChanged(nameof(AnswerSecond));
                OnPropertyChanged(nameof(AnswerThird));
                OnPropertyChanged(nameof(AnswerFourth));

                // Обновление свойств для отображения выбранного ответа
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
            //TODO 5 вопросов надо добавить в поля, и вообще подумать над автоматизацией этого момента, потому что количетсво вопросов всегда будет разное
            var results = new Dictionary<string, int>();
            foreach (var answer in SelectedAnswers)
            {
                if (!results.ContainsKey(answer))
                    results[answer] = 0;
                results[answer]++;
            }

            // Вычисление процента правильных ответов
            var totalAnswers = Questions.Count;
            var correctAnswersCount = results.Values.Sum();
            var percentage = (correctAnswersCount / (float)totalAnswers) * 100;

            // Определение диапазона оценок
            string scoreRange = percentage >= 90 ? "High" : percentage >= 50 ? "Medium" : "Low";

            // Выбор итогового результата на основе диапазона оценок
            var finalResult = _database.Table<FinalAnswer>()
                                .FirstOrDefault(fa => fa.TestId == CurrentQuestion.TestId && fa.ScoreRange == scoreRange)?.ResultText;

            string finalResultMessage = finalResult ?? "Результат не найден.";
            Console.WriteLine($"Отправка результата теста: {finalResultMessage}");
            MessagingCenter.Send(this, "FinishTest", finalResultMessage);

        }


        private object SelectFinalResult(Dictionary<string, int> results, int testId)
        {
            throw new NotImplementedException();
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

        public async Task ExportDatabase(string destinationPath)
        {
            try
            {
                var fileHelper = DependencyService.Get<IFileHelper>();
                if (fileHelper == null)
                    throw new InvalidOperationException("FileHelper not found");

                var dbPath = fileHelper.GetLocalFilePath("MentalTestDB.db");
                // Копирование файла базы данных
                System.IO.File.Copy(dbPath, destinationPath, overwrite: true);
                Console.WriteLine($"Database copied to {destinationPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during database export: {ex.Message}");
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
        [NotNull]
        public int CorrectAnswerIndex { get; set; }
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
        public string ScoreRange { get; set; } // High, Medium, Low
    }
}