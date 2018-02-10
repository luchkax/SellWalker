using System;
using System.Collections.Generic;

namespace sellwalker.Models
{
    public class Product : BaseEntity
    {   
    
        public int ProductId{get;set;}
        public string Condition{get;set;}
        public string Title{get;set;}
        public string Description{get;set;}
        public decimal Price{get;set;}
        public string Picture { get; set; }
        public DateTime CreatedAt{get;set;}
        public List<Order> Orders {get; set;}
        public Product()
        {
            Orders = new List<Order>();
        }
        public int UserId{get;set;}
        public User Seller{get;set;}


        

        
    }
}