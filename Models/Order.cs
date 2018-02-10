using System;
using System.Collections.Generic;

namespace sellwalker.Models
{
    public class Order : BaseEntity
    {   
    
        public int OrderId{get;set;}
        public int UserId{get;set;}
        public User Buyer{get;set;}
        public int ProductId{get;set;}
        public Product product{get;set;}
        public int Quantity{get;set;}
        public DateTime CreatedAt{get;set;}


        

        
    }
}