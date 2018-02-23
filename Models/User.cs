using System;
using System.Collections.Generic;

namespace sellwalker.Models
{
    public class User : BaseEntity
    {   
    
        public int UserId{get;set;}
        public int ReviewedId{get;set;}
        public string FirstName{get;set;}
        public string LastName{get;set;}
        public string Email{get;set;}
        public string Password{get;set;}
        public string Status{get;set;}
        public string ProfilePic {get;set;}

        public List<Order> Orders {get;set;}
        public List<Product> products {get;set;}
        public List<Review> Reviews {get;set;}
        public User()
        {
            Orders = new List<Order>();
            products = new List<Product>();
            Reviews = new List<Review>();
        }

        
    }
}