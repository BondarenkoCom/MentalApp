using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace MentalTest.ViewModels
{
    public class SurveyViewModel
    {
        //сделать 
        public ObservableCollection<Question> Questions { get; set; }
        public Command SelectAnswerCommand { get; set; }

        public SurveyViewModel(TestItem test)
        {
            // Здесь вы можете инициализировать вопросы и команды в соответствии с переданным TestItem
            Questions = new ObservableCollection<Question>();
            SelectAnswerCommand = new Command(ExecuteSelectAnswer);

            // Загрузите вопросы и ответы для теста
            LoadQuestions(test);
        }

        private void LoadQuestions(TestItem test)
        {
            // Реализуйте логику загрузки вопросов для теста
        }

        private void ExecuteSelectAnswer(object answer)
        {
            // Реализуйте логику, которая выполняется при выборе ответа
        }
    }

    // Возможно, вам потребуется определить дополнительные классы или структуры для вопросов и ответов
    public class Question
    {
        public string QuestionText { get; set; }
        // Другие свойства, связанные с вопросами
    }
}
