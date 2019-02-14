using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebAPI.Models
{
	public class EmployeeModel
	{

		[Display(Name = "Employee ID")]
		[Range(100000, 999999, ErrorMessage = "You need to enter valid EmployeeId")]
		public int EmployeeId { get; set; }

		[Display(Name = "First Name")]
		[Required(ErrorMessage = "You need to enter your First Name")]
		public string FirstName { get; set; }

		[Display(Name = "Last Name")]
		[Required(ErrorMessage = "You need to enter your Last Name")]
		public string LastName { get; set; }

		[DataType(DataType.EmailAddress)]
		[Display(Name = "Email Address")]
		[Required(ErrorMessage = "You need to enter your Email Address")]
		public string EmailAddress { get; set; }

		[Display(Name = "Confirm Email")]
		[Compare("EmailAddress", ErrorMessage = "The emails don't match")]
		public string ConfirmEmail { get; set; }

		[Display(Name = "Password")]
		[Required(ErrorMessage = "You must enter a password")]
		[DataType(DataType.Password)]
		[StringLength(100, MinimumLength = 8, ErrorMessage = "You must enter password longer then 8 symbols")]
		public string Password { get; set; }

		[Display(Name = "Confirm Password")]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "The passwords don't match")]
		public string ConfirmPassword { get; set; }
	}
}