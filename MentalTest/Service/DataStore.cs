using MentalTest.Models;
using System.Collections.Generic;

namespace MentalTest.Service
{
    public class DataStore
    {
        public static DataStore Instance { get; } = new DataStore();

        public List<TestCardModel> TestItems { get; private set; }
        public Dictionary<int, List<QuestionModal>> TestQuestions { get; private set; } = new Dictionary<int, List<QuestionModal>>();

        private DataStore() { }

        public void SaveTestItems(List<TestCardModel> items)
        {
            TestItems = items;
        }

        public void SaveQuestionsForTest(List<QuestionModal> questions, int testId)
        {
            if (TestQuestions.ContainsKey(testId))
            {
                TestQuestions[testId] = questions;
            }
            else
            {
                TestQuestions.Add(testId, questions);
            }
        }
    }
}
