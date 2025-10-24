using System;
using System.Linq;
using System.Web.Mvc;
using BigBank.Models;
using BigBank.Filters;

namespace BigBank.Controllers
{
    [SessionAuthorize(RolesCsv = "Manager,Employee")]
    [NoCache]
    public class TransactionsController : Controller
    {
        // GET: DepositWithdraw
        public ActionResult DepositWithdraw(string AccountID = null)
        {
            if (!string.IsNullOrEmpty(AccountID))
            {
                ViewBag.AccountID = AccountID;
                using (var db = new BigBankEntities())
                {
                    var acc = db.SavingsAccounts.FirstOrDefault(s => s.SBAccountID == AccountID);
                    if (acc != null)
                    {
                        var cust = db.Customers.FirstOrDefault(c => c.CustID == acc.CustID);
                        ViewBag.AccountName = cust != null ? cust.CustName : "";
                        ViewBag.AccountBalance = acc.Balance;
                    }
                }
            }
            return View("~/Views/Home/DepositWithdraw.cshtml");
        }

        // Lookup savings account name and balance for manager/employee UI
        public ActionResult LookupAccountName(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return Json(new { name = "", balance = (decimal?)null }, JsonRequestBehavior.AllowGet);

            using (var db = new BigBankEntities())
            {
                var acc = db.SavingsAccounts.FirstOrDefault(s => s.SBAccountID == id);
                if (acc == null) return Json(new { name = "", balance = (decimal?)null }, JsonRequestBehavior.AllowGet);
                var cust = db.Customers.FirstOrDefault(c => c.CustID == acc.CustID);
                return Json(new { name = cust != null ? cust.CustName : "", balance = acc.Balance }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DepositWithdraw(string AccountID, decimal? Amount, string Mode)
        {
            if (string.IsNullOrWhiteSpace(AccountID) || !Amount.HasValue || string.IsNullOrWhiteSpace(Mode))
            {
                TempData["Error"] = "Account, amount and mode are required.";
                return View("~/Views/Home/DepositWithdraw.cshtml");
            }

            if (Amount.Value < 100)
            {
                TempData["Error"] = "Minimum amount is Rs 100.";
                return View("~/Views/Home/DepositWithdraw.cshtml");
            }

            if (Amount.Value <= 0)
            {
                TempData["Error"] = "Amount must be greater than zero.";
                return View("~/Views/Home/DepositWithdraw.cshtml");
            }

            using (var db = new BigBankEntities())
            {
                var acc = db.SavingsAccounts.FirstOrDefault(s => s.SBAccountID == AccountID);
                if (acc == null)
                {
                    TempData["Error"] = "Account not found.";
                    return View("~/Views/Home/DepositWithdraw.cshtml");
                }

                var now = DateTime.Now;

                try
                {
                    if (string.Equals(Mode, "Deposit", StringComparison.OrdinalIgnoreCase))
                    {
                        var newBal = acc.Balance + Amount.Value;
                        db.Database.ExecuteSqlCommand("UPDATE SavingsAccount SET Balance = @p0 WHERE SBAccountID = @p1", newBal, AccountID);
                        db.Database.ExecuteSqlCommand("INSERT INTO SavingsTransaction (SBAccountID, TransactionDate, TransactionType, Amount, Remarks) VALUES (@p0,@p1,@p2,@p3,@p4)", AccountID, now, "D", Amount.Value, "Deposit by staff");
                        TempData["Success"] = $"Deposit successful. New balance: Rs {newBal:N2}";
                    }
                    else if (string.Equals(Mode, "Withdraw", StringComparison.OrdinalIgnoreCase) || string.Equals(Mode, "Withdrawal", StringComparison.OrdinalIgnoreCase))
                    {
                        if (acc.Balance < Amount.Value)
                        {
                            TempData["Error"] = "Insufficient balance.";
                            return View("~/Views/Home/DepositWithdraw.cshtml");
                        }

                        if (acc.Balance - Amount.Value < 1000m)
                        {
                            TempData["Error"] = "Cannot withdraw: account must maintain minimum balance of 1000.";
                            return View("~/Views/Home/DepositWithdraw.cshtml");
                        }

                        var newBal = acc.Balance - Amount.Value;
                        db.Database.ExecuteSqlCommand("UPDATE SavingsAccount SET Balance = @p0 WHERE SBAccountID = @p1", newBal, AccountID);
                        db.Database.ExecuteSqlCommand("INSERT INTO SavingsTransaction (SBAccountID, TransactionDate, TransactionType, Amount, Remarks) VALUES (@p0,@p1,@p2,@p3,@p4)", AccountID, now, "W", Amount.Value, "Withdrawal by staff");
                        TempData["Success"] = $"Withdrawal successful. New balance: Rs {newBal:N2}";
                    }
                    else
                    {
                        TempData["Error"] = "Unknown mode selected.";
                        return View("~/Views/Home/DepositWithdraw.cshtml");
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Operation failed: " + ex.Message;
                    return View("~/Views/Home/DepositWithdraw.cshtml");
                }
            }

            return View("~/Views/Home/DepositWithdraw.cshtml");
        }
    }
}
