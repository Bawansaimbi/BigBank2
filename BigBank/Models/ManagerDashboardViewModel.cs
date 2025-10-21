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
    }

    public class ManagerDashboardViewModel
    {
        public List<Customer> Customers { get; set; }
        public List<Employee> Employees { get; set; }
        public List<TransactionItem> RecentTransactions { get; set; }
    }
}
