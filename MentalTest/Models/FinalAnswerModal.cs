using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MentalTest.Models
{
    public class FinalAnswerModal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }

        [Required]
        public int testId { get; set; }

        [Required]
        public string resultText { get; set; }

        [Required]
        public string scoreRange { get; set; }
    }
}
