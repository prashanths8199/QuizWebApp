using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace QuizWebApp.ViewModel
{
    public class RegisterUserViewModel
    {
        [Display(Name = "User Name")]
        [Required(ErrorMessage = "User Name is required.")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string UserPassword { get; set; }
        
        [Display(Name = "Confirm Password")]
        [Compare("UserPassword")]
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}