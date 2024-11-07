using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MentalTest.Models;
using MentalTest.Service;
using System.Windows.Input;
using SQLite;

namespace MentalTest.ViewModels
{
    public class SurveyViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<AnswerModel> CurrentAnswers { get; set; }
        public ObservableCollection<QuestionModal> Questions { get; set; }
        public Command ContinueCommand { get; set; }
        public ICommand AnswerCommand { get; private set; }
        public ObservableCollection<string> SelectedAnswers { get; set; } = new ObservableCollection<string>();

        private AnswerModel _selectedAnswer;
        public AnswerModel SelectedAnswer
        {
            get => _selectedAnswer;
            set
            {
                if (_selectedAnswer != value)
                {
                    if (_selectedAnswer != null)
                        _selectedAnswer.IsSelected = false;

                    Console.WriteLine($"SelectedAnswer changing from '{_selectedAnswer?.Text}' to '{value?.Text}'");
                    _selectedAnswer = value;

                    if (_selectedAnswer != null)
                        _selectedAnswer.IsSelected = true;

                    OnPropertyChanged(nameof(SelectedAnswer));
                    OnPropertyChanged(nameof(IsAnswerSelected));
                }
            }
        }

        public bool IsAnswerSelected => SelectedAnswer != null;

        public int CurrentQuestionIndex { get; set; }
        public int CurrentTestId { get; private set; }

        public QuestionModal CurrentQuestion => Questions.Count > CurrentQuestionIndex ? Questions[CurrentQuestionIndex] : null;

        public string QuestionCounterText => $"Question {CurrentQuestionIndex + 1} of {Questions.Count}";

        public SurveyViewModel(int testId)
        {
            CurrentTestId = testId;
            Questions = new ObservableCollection<QuestionModal>();
            ContinueCommand = new Command(OnContinue);
            AnswerCommand = new Command<AnswerModel>(OnAnswerSelected);
            CurrentQuestionIndex = 0;
            InitializeAsync(testId).ConfigureAwait(false);
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
            if (CurrentQuestion != null)
            {
                CurrentAnswers = new ObservableCollection<AnswerModel>(
                    CurrentQuestion.Answers.Split(';').Select(a => new AnswerModel { Text = a })
                );
            }
            else
            {
                CurrentAnswers = new ObservableCollection<AnswerModel>();
            }

            OnPropertyChanged(nameof(CurrentAnswers));
            OnPropertyChanged(nameof(CurrentQuestion));
            OnPropertyChanged(nameof(QuestionCounterText));
            SelectedAnswer = null; // Reset selected answer
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
                SelectedAnswers.Add(SelectedAnswer.Text);

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
                var correctAnswer = Questions[i].Answers.Split(';')[Questions[i].CorrectAnswerIndex];
                if (SelectedAnswers[i] == correctAnswer)
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

        private void OnAnswerSelected(AnswerModel answer)
        {
            SelectedAnswer = answer;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
