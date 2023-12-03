using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using SQLite;
using System.Linq;

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
                    // Обновление стилей кнопок
                    //UpdateAnswerStyles();
                }
            }
        }

        //public SurveyViewModel(TestItem test)
        //{
        //    Questions = new ObservableCollection<Question>();
        //    AnswerCommand = new Command<string>(OnAnswerSelected);
        //    ContinueCommand = new Command(OnContinue);
        //    LoadQuestions(test);
        //    CurrentQuestionIndex = 0; // Инициализация текущего индекса вопроса
        //}
        public SurveyViewModel(int testId)
        {
            Questions = new ObservableCollection<Question>();
            AnswerCommand = new Command<string>(OnAnswerSelected);
            ContinueCommand = new Command(OnContinue);
            LoadQuestions(testId); // Load questions based on testId
            CurrentQuestionIndex = 0;
        }


        private void LoadQuestions(int testId)
        {
            // Проверка существования таблицы Questions и её создание, если необходимо
            string createTableQuery = @"
        CREATE TABLE IF NOT EXISTS Questions (
            Id INTEGER NOT NULL PRIMARY KEY,
            TestId INTEGER NOT NULL,
            QuestionText TEXT NOT NULL,
            Answers TEXT NOT NULL
        )";

            // надо чинить базу данных
            _database.Execute(createTableQuery);

            // Проверка наличия вопросов для данного теста
            var existingQuestions = _database.Table<Question>().Where(q => q.TestId == testId).ToList();
            if (!existingQuestions.Any())
            {
                // Если вопросов ещё нет, добавляем их в базу данных
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

            // Загрузка вопросов из базы данных для текущего теста
            Questions = new ObservableCollection<Question>(_database.Table<Question>().Where(q => q.TestId == testId).ToList());
        }


        /*
        private void SeedDatabaseWithQuestionsForTest(int testId)
        {
            // Список новых вопросов для теста "How well do you know Mercy?"
            var newQuestions = new List<Question>
    {
        new Question {
            TestId = testId,
            QuestionText = "Какое умение Мерси позволяет ей воскрешать павших союзников?",
            Answers = new ObservableCollection<string> { "Guardian Angel", "Resurrect", "Angelic Descent", "Caduceus Staff" }
        },
        new Question {
            TestId = testId,
            QuestionText = "Как называется главный лечащий поток Мерси?",
            Answers = new ObservableCollection<string> { "Healing Stream", "Health Flow", "Caduceus Beam", "Regeneration" }
        },
        new Question {
            TestId = testId,
            QuestionText = "Какой эффект даёт ультимативная способность Мерси Valkyrie?",
            Answers = new ObservableCollection<string> { "Увеличенное лечение", "Мгновенное воскрешение", "Бессмертие", "Усиление всех способностей" }
        },
        new Question {
            TestId = testId,
            QuestionText = "Какую роль играет Мерси в команде Overwatch?",
            Answers = new ObservableCollection<string> { "Танк", "Поддержка", "Атака", "Оборона" }
        },
        new Question {
            TestId = testId,
            QuestionText = "Какой пассивный навык позволяет Мерси замедлять своё падение?",
            Answers = new ObservableCollection<string> { "Hover", "Glide", "Angelic Descent", "Feather Fall" }
        }
    };

            // Перебираем вопросы и добавляем их в базу данных
            foreach (var question in newQuestions)
            {
                var existingQuestion = _database.Table<Question>().FirstOrDefault(q => q.TestId == question.TestId && q.QuestionText == question.QuestionText);
                if (existingQuestion == null)
                {
                    _database.Insert(question);
                    Console.WriteLine($"Вопрос '{question.QuestionText}' добавлен для теста с ID '{question.TestId}'.");
                }
                else
                {
                    Console.WriteLine($"Вопрос '{question.QuestionText}' уже существует для теста с ID '{question.TestId}'.");
                }
            }
        }
        */

        /*
        private void LoadQuestions(TestItem test)
        {
            // Добавление вопросов в тест
            Questions.Add(new Question
            {
                QuestionText = "You're faced with a tough choice in a high-stakes battle, what's your approach?",
                Answers = new ObservableCollection<string> { "Stand my ground", "Call for backup", "Look for a strategic advantage" }
            });
            Questions.Add(new Question
            {
                QuestionText = "A teammate is in trouble, and you're the only one who can help, what do you do?",
                Answers = new ObservableCollection<string> { "Dive into the fray", "Assess and act accordingly", "Provide long-range support" }
            });
            Questions.Add(new Question
            {
                QuestionText = "Your strategy in team fights tends to be more about...",
                Answers = new ObservableCollection<string> { "Direct confrontation", "Adapting to changes", "Supporting the team" }
            });
            Questions.Add(new Question
            {
                QuestionText = "When the pressure is on, do you prefer to take charge or offer support?",
                Answers = new ObservableCollection<string> { "Lead the charge", "Support from behind", "Balance between the two" }
            });
            Questions.Add(new Question
            {
                QuestionText = "What's your preference when it comes to dealing with obstacles?",
                Answers = new ObservableCollection<string> { "Face them head-on", "Find a way around", "Help others through" }
            });
        }
        */

        //private void LoadQuestions(int testId)
        //{
        //    // Здесь код, который извлекает вопросы из базы данных для данного testId
        //    var questionsFromDb = GetQuestionsFromDatabase(testId); // Это псевдокод
        //    foreach (var question in questionsFromDb)
        //    {
        //        Questions.Add(question);
        //    }
        //}

        //private ObservableCollection<Question> GetQuestionsFromDatabase(int testId)
        //{
        //    // Здесь код для запроса к базе данных
        //    // Возвращает список вопросов для данного testId
        //}


        //private void OnAnswerSelected(string answer)
        //{
        //    SelectedAnswer = answer;
        //    // Обновляем UI
        //    OnPropertyChanged(nameof(IsFirstAnswerSelected));
        //    OnPropertyChanged(nameof(IsSecondAnswerSelected));
        //    OnPropertyChanged(nameof(IsThirdAnswerSelected));
        //    OnPropertyChanged(nameof(IsFourthAnswerSelected));
        //    // Возможно, обновление стилей кнопок здесь не нужно
        //    // UpdateAnswerStyles(); // Эту строку можно убрать
        //}


        private void OnContinue()
        {
            if (CurrentQuestionIndex < Questions.Count - 1)
            {
                CurrentQuestionIndex++;
                SelectedAnswer = null; // Сброс выбранного ответа
        
                // Обновляем UI для нового вопроса и сброса выбранных ответов
                OnPropertyChanged(nameof(CurrentQuestion));
                OnPropertyChanged(nameof(AnswerFirst));
                OnPropertyChanged(nameof(AnswerSecond));
                OnPropertyChanged(nameof(AnswerThird));
                OnPropertyChanged(nameof(AnswerFourth));
        
                // Обновляем UI для булевых свойств, чтобы отменить подсветку кнопок
                OnPropertyChanged(nameof(IsFirstAnswerSelected));
                OnPropertyChanged(nameof(IsSecondAnswerSelected));
                OnPropertyChanged(nameof(IsThirdAnswerSelected));
                OnPropertyChanged(nameof(IsFourthAnswerSelected));
            }
            else
            {
                // Завершаем тест и выводим результаты
                FinishTest(); // Метод, который необходимо реализовать для обработки конца теста
            }
        }



        //временные ответы и тд
        private void FinishTest()
        {
            // Здесь код для обработки конца теста и показа результатов
            // Например, вы можете суммировать результаты и выбрать соответствующий вывод
            // Это просто пример логики, которую вы можете реализовать

            // Подсчет результатов
            int scoreDva = 0;
            int scoreMercy = 0;
            foreach (var answer in SelectedAnswers)
            {
                if (answer == "Dva") scoreDva++;
                if (answer == "Mercy") scoreMercy++;
            }

            // Определение итогового результата
            string finalResult;
            if (scoreDva > scoreMercy)
                finalResult = "Вы больше подходите для персонажа Dva.";
            else if (scoreMercy > scoreDva)
                finalResult = "Вы больше подходите для персонажа Mercy.";
            else
                finalResult = "Вы одинаково хорошо подходите для обоих персонажей.";

            // Показать результаты
            // Здесь вы можете вызвать метод навигации, чтобы перейти на новую страницу с этими результатами
            // Например, можно использовать MessagingCenter для отправки сообщения и перехода на страницу результатов
            MessagingCenter.Send(this, "FinishTest", finalResult);
        }

        // Не забудьте реализовать OnPropertyChanged для обновления UI при изменении свойств
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


    // Вспомогательный класс для вопросов
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
