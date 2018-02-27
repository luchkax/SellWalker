using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace sellwalker.Models
{
    public abstract class BaseEntity{};

    public class Register : BaseEntity
    {   
        [Key]
        public int UserId{get;set;}

        [Required]
        [MinLength(2)]
        public string FirstName{get;set;}

        [Required]
        [MinLength(2)]
        public string LastName{get;set;}

        [Required]
        [EmailAddress]
        public string Email{get;set;}

        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password{get;set;}

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword{get;set;}

        public IFormFile ProfileImage {get;set;}




    }

    public class Login : BaseEntity
    {
        [Required]
        [EmailAddress]
        public string Email {get;set;}

        [Required]
        [MinLength(8, ErrorMessage="Passwords must be at least 8 characters")]
        [DataType(DataType.Password)]
        public string Password{get;set;}
    } 

    public class ProductCheck : BaseEntity
    {
        [Required]
        public string Condition{get;set;}

        [Required]
        [MinLength(2, ErrorMessage="Name of the product should be greater than 2 characters")]
        public string Title{get;set;}

        [Required]
        [MinLength(10, ErrorMessage="Description of the product should be greater than 10 characters")]
        public string Description{get;set;}

        [Required]
        public decimal Price{get;set;}

        public string Status {get;set;}
        public IFormFile Image { get; set; }
    }

    public class UpdateUser : BaseEntity
    {   
        [Key]
        public int UserId{get;set;}

        [Required]
        [MinLength(2)]
        public string FirstName{get;set;}

        [Required]
        [MinLength(2)]
        public string LastName{get;set;}

        [Required]
        [EmailAddress]
        public string Email{get;set;}

        public IFormFile ProfileImage {get;set;}

    }
    public class UpdatePass : BaseEntity
    {
        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password{get;set;}

        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string NewPassword{get;set;}

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword")]
        public string ConfirmNewPassword{get;set;}
    }
     public class UpdateUserInfo : BaseEntity
    {
        [Required]
        public string Status{get;set;}

        public int ReviewedId{get;set;}
    }
    public class ReviewCheck : BaseEntity
    {
        [Required]
        public string Content{get;set;}
    }
}