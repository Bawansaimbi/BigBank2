using System;
using System.Collections.Generic;

namespace BigBank.Models
{
    public class TransactionItem
    {
        public string Id { get; set; }
        public string AccountId { get; set; }
        public DateTime? Date { get; set; }
        public string Source { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
        // added for transactions listing
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string FromName { get; set; }
        public string ToName { get; set; }
    }

    public class ManagerDashboardViewModel
    {
        public List<Customer> Customers { get; set; }
        public List<Employee> Employees { get; set; }
        public List<TransactionItem> RecentTransactions { get; set; }

        // summary stats
        public int TotalCustomers { get; set; }
        public int TotalEmployees { get; set; }
        public int TotalAccounts { get; set; }
        public int SavingsCount { get; set; }
        public int FDCount { get; set; }
        public int LoanCount { get; set; }
        public int ActiveLoansCount { get; set; }
        public int ActiveUsers { get; set; }
        public decimal PendingLoanAmount { get; set; }
        public int RecentTransactionsCount { get; set; }
    }
}
