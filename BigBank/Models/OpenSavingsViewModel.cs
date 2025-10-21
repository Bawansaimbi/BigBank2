using System.ComponentModel.DataAnnotations;

namespace BigBank.Models
{
    public class OpenSavingsViewModel
    {
        [Required]
        [Range(1000, double.MaxValue, ErrorMessage = "Minimum opening balance is 1000.")]
        public decimal InitialBalance { get; set; }
    }
}
