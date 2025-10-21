using System;
using System.ComponentModel.DataAnnotations;

namespace BigBank.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string UserType { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        public string CustName { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }

        [Required]
        [RegularExpression("^[A-Z]{4}[0-9]{4}$", ErrorMessage = "PAN must be in format ABCD1234 (4 letters, 4 digits)")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "PAN must be 8 characters")]
        public string PAN { get; set; }

        [Required]
        [RegularExpression("^[0-9]{10}$", ErrorMessage = "Phone number must be 10 digits")]
        public string PhoneNum { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
