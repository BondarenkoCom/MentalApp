using System.ComponentModel.DataAnnotations;

namespace MentalTest.Models
{
    public class QuestionModal
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TestId { get; set; }

        [Required]
        public string QuestionText { get; set; }

        [Required]
        public string Answers { get; set; }

        [Required]
        public int CorrectAnswerIndex { get; set; }
    }
}
