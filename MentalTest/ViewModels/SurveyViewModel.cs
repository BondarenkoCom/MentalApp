using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using SQLite;
using System.Linq;
using System.Threading.Tasks;
using MentalTest.Interfaces;
using MentalTest.Models;
using MentalTest.Service;
using System.Windows.Input;

namespace MentalTest.ViewModels
{
    public class SurveyViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> CurrentAnswers =>
            new ObservableCollection<string>(CurrentQuestion?.Answers.Split(';') ?? new string[0]);

        public ObservableCollection<QuestionModal> Questions { get; set; }
        public Command ContinueCommand { get; set; }
        public int CurrentQuestionIndex { get; set; }
        public QuestionModal CurrentQuestion => Questions.Count > CurrentQuestionIndex ? Questions[CurrentQuestionIndex] : null;
        public ICommand AnswerCommand { get; private set; }
        public ObservableCollection<string> QuestionsAndAnswers { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> SelectedAnswers { get; set; } = new ObservableCollection<string>();

        private string _selectedAnswer;
        public int CurrentTestId { get; private set; }

        public string SelectedAnswer
        {
            get => _selectedAnswer;
            set
            {
                if (_selectedAnswer != value)
                {
                    Console.WriteLine($"SelectedAnswer changing from '{_selectedAnswer}' to '{value}'");
                    _selectedAnswer = value;
                    OnPropertyChanged(nameof(SelectedAnswer));
                    OnPropertyChanged(nameof(IsFirstAnswerSelected));
                    OnPropertyChanged(nameof(IsSecondAnswerSelected));
                    OnPropertyChanged(nameof(IsThirdAnswerSelected));
                    OnPropertyChanged(nameof(IsFourthAnswerSelected));
                }
            }
        }


        public bool IsFirstAnswerSelected => SelectedAnswer == CurrentAnswers[0];
        public bool IsSecondAnswerSelected => SelectedAnswer == CurrentAnswers[1];
        public bool IsThirdAnswerSelected => SelectedAnswer == CurrentAnswers[2];
        public bool IsFourthAnswerSelected => SelectedAnswer == CurrentAnswers[3];

        public string QuestionCounterText => $"Question {CurrentQuestionIndex + 1} of {Questions.Count}";

        public SurveyViewModel(int testId)
        {
            InitializeAsync(testId).ConfigureAwait(false);
            CurrentTestId = testId;

            try
            {
                AnswerCommand = new Command<string>(OnAnswerSelected);
                Questions = new ObservableCollection<QuestionModal>();
                ContinueCommand = new Command(OnContinue);
                CurrentQuestionIndex = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred in the constructor of SurveyViewModel: {ex.Message}");
                Console.WriteLine($"Call stack: {ex.StackTrace}");
                throw;
            }
        }

        public async Task InitializeAsync(int testId)
        {
            Console.WriteLine("InitializeAsync started.");
            try
            {
                var questionsFromApi = await LoadQuestionsFromApi(testId);
                if (questionsFromApi != null && questionsFromApi.Count > 0)
                {
                    Questions = new ObservableCollection<QuestionModal>(questionsFromApi);
                    CurrentQuestionIndex = 0;
                    UpdateUI();
                }
                else
                {
                    Console.WriteLine("No questions were loaded from the API.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in InitializeAsync: {ex.Message}");
            }
        }

        private void UpdateUI()
        {
            OnPropertyChanged(nameof(CurrentQuestion));
            OnPropertyChanged(nameof(CurrentAnswers));
            OnPropertyChanged(nameof(QuestionCounterText));
            Console.WriteLine("UI updated with current question and answers.");
        }

        private async Task<List<QuestionModal>> LoadQuestionsFromApi(int testId)
        {
            Console.WriteLine($"Starting to fetch questions for test ID: {testId}");
            List<QuestionModal> questions = null;

            try
            {
                var apiService = new ApiService();
                questions = await apiService.GetQuestionsByTestIdAsync(testId);

                if (questions != null && questions.Count > 0)
                {
                    Console.WriteLine($"Received {questions.Count} questions for test ID {testId}.");
                    DataStore.Instance.SaveQuestionsForTest(questions, testId);
                    Console.WriteLine($"Saved questions to DataStore for test ID {testId}.");
                }
                else
                {
                    Console.WriteLine("No questions were retrieved from the API.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in LoadQuestionsFromApi: {ex}");
            }

            return questions;
        }

        private void OnContinue()
        {
            if (SelectedAnswer != null)
            {
                SelectedAnswers.Add(SelectedAnswer);
                if (CurrentQuestionIndex < Questions.Count - 1)
                {
                    CurrentQuestionIndex++;
                    SelectedAnswer = null;
                    UpdateUI();
                }
                else
                {
                    FinishTest();
                }
            }
        }

        private async void FinishTest()
        {
            Console.WriteLine("FinishTest started.");

            var correctCount = CalculateCorrectAnswers();
            var totalQuestions = Questions.Count;
            var scorePercentage = (double)correctCount / totalQuestions * 100;
            string scoreRange = DetermineScoreRange(scorePercentage);

            var apiService = new ApiService();
            var finalAnswers = await apiService.GetFinalAnswersByTestIdAsync(CurrentTestId);
            var finalAnswer = finalAnswers.FirstOrDefault(fa => fa.TestId == CurrentTestId && fa.ScoreRange == scoreRange);

            string finalResultMessage = finalAnswer?.ResultText ?? "Result not found.";
            MessagingCenter.Send(this, "FinishTest", finalResultMessage);

            Console.WriteLine("FinishTest completed.");
        }

        private int CalculateCorrectAnswers()
        {
            int correctAnswers = 0;
            for (int i = 0; i < Questions.Count; i++)
            {
                if (SelectedAnswers[i] == Questions[i].Answers.Split(';')[Questions[i].CorrectAnswerIndex])
                    correctAnswers++;
            }
            return correctAnswers;
        }

        private string DetermineScoreRange(double scorePercentage)
        {
            if (scorePercentage >= 90)
                return "High";
            else if (scorePercentage >= 50)
                return "Medium";
            else
                return "Low";
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnAnswerSelected(string answer)
        {
            SelectedAnswer = answer;
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
