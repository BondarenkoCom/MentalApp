using MentalTest.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MentalTest.Service
{
    public class DataStore
    {
        public static DataStore Instance { get; } = new DataStore();
        public List<TestCardModel> TestItems { get; private set; }

        private DataStore() { }

        public void SaveTestItems(List<TestCardModel> items)
        {
            TestItems = items;
        }
    }
}
