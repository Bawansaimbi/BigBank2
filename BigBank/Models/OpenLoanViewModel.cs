using System;
using System.ComponentModel.DataAnnotations;

namespace BigBank.Models
{
    public class OpenLoanViewModel
    {
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Range(10000, double.MaxValue, ErrorMessage = "Minimum Loan amount is Rs. 10,000.")]
        [Display(Name = "Loan Amount")]
        public decimal LoanAmount { get; set; }

        [Required]
        [Range(0.01, 99.99, ErrorMessage = "Interest rate must be between 0.01 and 99.99%")]
        [Display(Name = "Interest Rate")]
        public decimal InterestRate { get; set; }

        [Required]
        [Range(1, 30, ErrorMessage = "Tenure must be between 1 and 30 years")]
        [Display(Name = "Tenure (Years)")]
        public int Tenure { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Monthly income is required and must be greater than 0.")]
        [Display(Name = "Monthly Take Home")]
        public decimal MonthlyIncome { get; set; }

        // For pre-filling customer DOB to check senior citizen status
        public DateTime? CustomerDOB { get; set; }

        // Calculated fields (not input by user)
        public DateTime EndDate { get; set; }
        public decimal EMI { get; set; }
        public decimal TotalPayable { get; set; }
    }
}
