using System.Collections.Generic;

namespace BigBank.Models
{
    public class CustomerDashboardViewModel
    {
        public string SavingsAccountID { get; set; }
        public decimal? SavingsBalance { get; set; }
        public List<string> FDAccountIDs { get; set; }
        public List<string> LoanAccountIDs { get; set; }
        public decimal TotalBalance { get; set; }

        public CustomerDashboardViewModel()
        {
            FDAccountIDs = new List<string>();
            LoanAccountIDs = new List<string>();
        }
    }
}
