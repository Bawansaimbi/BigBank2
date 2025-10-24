using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using BigBank.Models;
using BigBank.Filters;

namespace BigBank.Controllers
{
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Login(string username, string password, string userType)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(userType))
            {
                ModelState.AddModelError("", "Please provide username, password and user type.");
                return View();
            }

            var hashed = HashPassword(password);

            using (var db = new BigBankEntities())
            {
                if (userType == "Customer")
                {
                    var cust = db.Customers.FirstOrDefault(c => c.Username == username);
                    if (cust != null && cust.Password != null && cust.Password.SequenceEqual(hashed))
                    {
                        Session["UserType"] = "Customer";
                        Session["UserID"] = cust.CustID;
                        Session["Username"] = cust.Username;
                        Session["UserAgent"] = Request.UserAgent ?? string.Empty;
                        Session["IP"] = Request.UserHostAddress ?? string.Empty;
                        Session["LastActivityUtc"] = DateTime.UtcNow;
                        return RedirectToAction("Index", "Customer");
                    }
                }
                else
                {
                    var emp = db.Employees.FirstOrDefault(e => e.Username == username);
                    if (emp != null && emp.Password != null && emp.Password.SequenceEqual(hashed))
                    {
                        if ((userType == "Employee" && emp.EmpType == "E") || (userType == "Manager" && emp.EmpType == "M"))
                        {
                            Session["UserType"] = emp.EmpType == "M" ? "Manager" : "Employee";
                            Session["UserID"] = emp.EmpID;
                            Session["Username"] = emp.Username;
                            Session["DeptID"] = emp.DeptID;
                            Session["UserAgent"] = Request.UserAgent ?? string.Empty;
                            Session["IP"] = Request.UserHostAddress ?? string.Empty;
                            Session["LastActivityUtc"] = DateTime.UtcNow;

                            if (emp.EmpType == "M")
                                return RedirectToAction("ManagerHome", "Manager");

                            var deptId = (emp.DeptID ?? "").Trim().ToUpper();
                            switch (deptId)
                            {
                                case "SB":
                                case "DEPT01":
                                    return RedirectToAction("SavingsEmployeeHome", "Employee");
                                case "FD":
                                    return RedirectToAction("FDEmployeeHome", "Employee");
                                case "LN":
                                    return RedirectToAction("LoanEmployeeHome", "Employee");
                                default:
                                    return RedirectToAction("Index", "Employee");
                            }
                        }
                    }
                }
            }

            ModelState.AddModelError("", "Invalid username, password or user type.");
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        // GET: Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Register(RegisterViewModel model)
        {
            // delegate to original HomeController logic for now to avoid duplication
            // Reuse HomeController via inheritance is not feasible, so call the same method body
            if (!ModelState.IsValid)
                return View(model);

            if (!System.Text.RegularExpressions.Regex.IsMatch(model.CustName ?? string.Empty, "^[A-Za-z ]+$$"))
            {
                ModelState.AddModelError("CustName", "Name must contain letters and spaces only.");
            }

            var minDob = DateTime.Today.AddYears(-18);
            if (model.DOB > minDob)
            {
                ModelState.AddModelError("DOB", "You must be at least 18 years old to register.");
            }

            var pan = (model.PAN ?? string.Empty).Trim().ToUpper();
            if (!System.Text.RegularExpressions.Regex.IsMatch(pan, "^[A-Z]{4}[0-9]{4}$"))
            {
                ModelState.AddModelError("PAN", "PAN must be in format ABCD1234 (4 letters followed by 4 digits).");
            }

            var phone = System.Text.RegularExpressions.Regex.Replace(model.PhoneNum ?? string.Empty, "\\s+", "");
            if (!System.Text.RegularExpressions.Regex.IsMatch(phone, "^[0-9]{10}$"))
            {
                ModelState.AddModelError("PhoneNum", "Phone number must be exactly 10 digits.");
            }

            using (var db = new BigBankEntities())
            {
                if (db.Customers.Any(c => c.Username == model.Username) || db.Employees.Any(e => e.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "Username already exists. Please choose a different username.");
                }

                if (db.Employees.Any(e => e.PAN == pan))
                {
                    ModelState.AddModelError("PAN", "PAN belongs to an employee or manager. Employees and managers cannot register as customers.");
                }

                if (db.Customers.Any(c => c.PAN == pan))
                {
                    ModelState.AddModelError("PAN", "A customer with this PAN already exists.");
                }

                if (db.Customers.Any(c => c.PhoneNum == phone))
                {
                    ModelState.AddModelError("PhoneNum", "Phone number is already registered.");
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var sql = @"INSERT INTO Customer (CustName, Gender, DOB, PAN, PhoneNum, Address, Username, Password, CreatedOn)
                            VALUES (@name, @gender, @dob, @pan, @phone, @addr, @username, @pwd, @created)";

                var pwd = HashPassword(model.Password);

                var parameters = new[] {
                    new System.Data.SqlClient.SqlParameter("@name", System.Data.SqlDbType.NVarChar, 50) { Value = (object)model.CustName ?? DBNull.Value },
                    new System.Data.SqlClient.SqlParameter("@gender", System.Data.SqlDbType.Char, 1) { Value = (object)model.Gender ?? DBNull.Value },
                    new System.Data.SqlClient.SqlParameter("@dob", System.Data.SqlDbType.Date) { Value = model.DOB },
                    new System.Data.SqlClient.SqlParameter("@pan", System.Data.SqlDbType.Char, 8) { Value = (object)pan ?? DBNull.Value },
                    new System.Data.SqlClient.SqlParameter("@phone", System.Data.SqlDbType.NVarChar, 15) { Value = (object)phone ?? DBNull.Value },
                    new System.Data.SqlClient.SqlParameter("@addr", System.Data.SqlDbType.NVarChar, 100) { Value = (object)model.Address ?? DBNull.Value },
                    new System.Data.SqlClient.SqlParameter("@username", System.Data.SqlDbType.NVarChar, 50) { Value = (object)model.Username ?? DBNull.Value },
                    new System.Data.SqlClient.SqlParameter("@pwd", System.Data.SqlDbType.VarBinary, 64) { Value = (object)pwd ?? DBNull.Value },
                    new System.Data.SqlClient.SqlParameter("@created", System.Data.SqlDbType.DateTime) { Value = DateTime.Now }
                };

                db.Database.ExecuteSqlCommand(sql, parameters);
            }

            TempData["Message"] = "Registration successful. You can now login.";
            return RedirectToAction("Login");
        }

        public ActionResult ChangePassword()
        {
            if (Session["UserType"] == null) return RedirectToAction("Login");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
            if (Session["UserType"] == null) return RedirectToAction("Login");

            if (string.IsNullOrWhiteSpace(CurrentPassword) || string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                ModelState.AddModelError("", "All fields are required.");
                return View();
            }

            if (NewPassword != ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
                return View();
            }

            if (NewPassword.Length < 6)
            {
                ModelState.AddModelError("NewPassword", "New password must be at least 6 characters.");
                return View();
            }

            var userType = Session["UserType"].ToString();
            var userId = Session["UserID"].ToString();
            var currentHash = HashPassword(CurrentPassword);
            var newHash = HashPassword(NewPassword);

            using (var db = new BigBankEntities())
            {
                if (userType == "Customer")
                {
                    var cust = db.Customers.FirstOrDefault(c => c.CustID == userId);
                    if (cust == null) return RedirectToAction("Login");
                    if (cust.Password == null || !cust.Password.SequenceEqual(currentHash))
                    {
                        ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                        return View();
                    }

                    db.Database.ExecuteSqlCommand("UPDATE Customer SET Password = @p0 WHERE CustID = @p1", newHash, userId);
                }
                else // Employee or Manager
                {
                    var emp = db.Employees.FirstOrDefault(e => e.EmpID == userId);
                    if (emp == null) return RedirectToAction("Login");
                    if (emp.Password == null || !emp.Password.SequenceEqual(currentHash))
                    {
                        ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                        return View();
                    }

                    db.Database.ExecuteSqlCommand("UPDATE Employee SET Password = @p0 WHERE EmpID = @p1", newHash, userId);
                }
            }

            TempData["Message"] = "Password changed successfully!";
            return RedirectToAction("ChangePassword");
        }

        public ActionResult Settings()
        {
            if (Session["UserType"] == null) return RedirectToAction("Login");
            return View();
        }

        private byte[] HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                return sha.ComputeHash(Encoding.UTF8.GetBytes(password ?? string.Empty));
            }
        }
    }
}
