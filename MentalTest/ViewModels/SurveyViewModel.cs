using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.ComponentModel;

namespace MentalTest.ViewModels
{
    public class SurveyViewModel : INotifyPropertyChanged
    {

        public bool IsFirstAnswerSelected => SelectedAnswer == AnswerFirst;
        public bool IsSecondAnswerSelected => SelectedAnswer == AnswerSecond;
        public bool IsThirdAnswerSelected => SelectedAnswer == AnswerThird;
        public bool IsFourthAnswerSelected => SelectedAnswer == AnswerFourth;

        public ObservableCollection<Question> Questions { get; set; }
        public Command<string> AnswerCommand { get; set; }
        public Command ContinueCommand { get; set; }
        public int CurrentQuestionIndex { get; set; }
        public Question CurrentQuestion => Questions.Count > CurrentQuestionIndex ? Questions[CurrentQuestionIndex] : null;

        public string AnswerFirst => CurrentQuestion?.Answers.Count > 0 ? CurrentQuestion?.Answers[0] : null;
        public string AnswerSecond => CurrentQuestion?.Answers.Count > 1 ? CurrentQuestion?.Answers[1] : null;
        public string AnswerThird => CurrentQuestion?.Answers.Count > 2 ? CurrentQuestion?.Answers[2] : null;
        public string AnswerFourth => CurrentQuestion?.Answers.Count > 3 ? CurrentQuestion?.Answers[3] : null;

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

        public SurveyViewModel(TestItem test)
        {
            Questions = new ObservableCollection<Question>();
            AnswerCommand = new Command<string>(OnAnswerSelected);
            ContinueCommand = new Command(OnContinue);
            LoadQuestions(test);
            CurrentQuestionIndex = 0; // Инициализация текущего индекса вопроса
        }

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

        private void OnAnswerSelected(string answer)
        {
            SelectedAnswer = answer;
            // Обновляем UI
            OnPropertyChanged(nameof(IsFirstAnswerSelected));
            OnPropertyChanged(nameof(IsSecondAnswerSelected));
            OnPropertyChanged(nameof(IsThirdAnswerSelected));
            OnPropertyChanged(nameof(IsFourthAnswerSelected));
            // Возможно, обновление стилей кнопок здесь не нужно
            // UpdateAnswerStyles(); // Эту строку можно убрать
        }


        private void OnContinue()
        {
            // Переход к следующему вопросу
            if (CurrentQuestionIndex < Questions.Count - 1)
            {
                CurrentQuestionIndex++;
                SelectedAnswer = null;

                OnPropertyChanged(nameof(IsFirstAnswerSelected));
                OnPropertyChanged(nameof(IsSecondAnswerSelected));
                OnPropertyChanged(nameof(IsThirdAnswerSelected));
                OnPropertyChanged(nameof(IsFourthAnswerSelected));
            }
            else
            {
                // Завершаем тест и выводим результаты
            }
        }

        // Не забудьте реализовать OnPropertyChanged для обновления UI при изменении свойств
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }


    public class Question
    {
        public string QuestionText { get; set; }
        public ObservableCollection<string> Answers { get; set; }
    }
}
