using System.ComponentModel.DataAnnotations;

namespace MentalTest.Models
{
    public class TestCardModel
    {
        [Key]
        public int id { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public string questionsStatus { get; set; }

        public bool isStarred { get; set; }

        public string category { get; set; }
    }
}
