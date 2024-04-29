using MentalTest.Models;
using System;
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
            DebugPrintAllData();
        }

        public void SaveQuestionsForTest(List<QuestionModal> questions, int testId)
        {
            Console.WriteLine($"Attempting to save {questions.Count} questions for test ID {testId}.");

            if (TestQuestions.ContainsKey(testId))
            {
                TestQuestions[testId] = questions;
                Console.WriteLine($"Updated existing questions for test ID {testId}.");
            }
            else
            {
                TestQuestions.Add(testId, questions);
                Console.WriteLine($"Added new questions for test ID {testId}.");
            }

            // Добавим дополнительное логирование для вывода информации о каждом вопросе и ответе
            foreach (var question in questions)
            {
                Console.WriteLine($"Question ID: {question.Id}, Question Text: {question.QuestionText}, Answers: {question.Answers}, Correct Answer Index: {question.CorrectAnswerIndex}");
            }
        }


        public List<QuestionModal> GetQuestionsForTest(int testId)
        {
            Console.WriteLine($"GetQuestionsForTest called for testId: {testId}");

            if (TestQuestions.ContainsKey(testId))
            {
                var questions = TestQuestions[testId];
                Console.WriteLine($"Found {questions.Count} questions for testId: {testId}");

                foreach (var question in questions)
                {
                    Console.WriteLine($"Question ID: {question.Id}, Text: {question.QuestionText}");
                    Console.WriteLine($"Answers: {question.Answers}");
                    Console.WriteLine($"Correct Answer Index: {question.CorrectAnswerIndex}");
                }

                return questions;
            }
            else
            {
                Console.WriteLine($"No questions found for testId: {testId}");
                return new List<QuestionModal>();
            }
        }



        public void DebugPrintAllData()
        {
            Console.WriteLine("DebugPrintAllData called.");

            if (TestItems != null)
            {
                Console.WriteLine($"Printing all TestItems, count: {TestItems.Count}");
                foreach (var item in TestItems)
                {
                    Console.WriteLine($"Test ID: {item.id}, Title: {item.title}, Description: {item.description}");
                }
            }
            else
            {
                Console.WriteLine("TestItems is null or empty.");
            }

            Console.WriteLine($"Printing all TestQuestions, count: {TestQuestions.Count}");
            foreach (var pair in TestQuestions)
            {
                Console.WriteLine($"Test ID: {pair.Key}, Questions count: {pair.Value.Count}");
                foreach (var question in pair.Value)
                {
                    Console.WriteLine($"Question ID: {question.Id}, Question Text: {question.QuestionText}, Answers: {question.Answers}, Correct Answer Index: {question.CorrectAnswerIndex}");
                }
            }
        }

    }
}
