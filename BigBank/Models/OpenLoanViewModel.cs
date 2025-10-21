using System;
using System.ComponentModel.DataAnnotations;

namespace BigBank.Models
{
    public class OpenLoanViewModel
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        [Range(10000, double.MaxValue, ErrorMessage = "Minimum Loan amount is 10000.")]
        public decimal LoanAmount { get; set; }

        [Required]
        public decimal InterestRate { get; set; }

        [Required]
        public int Tenure { get; set; }
    }
}
