using System;
using System.Collections.Generic;

namespace sellwalker.Models
{
    public class Review : BaseEntity
    {
        public int ReviewId{get;set;}
        public int ReviewedId{get;set;}
        public string Content{get;set;}
        public DateTime CreatedAt{get;set;}
        public int UserId{get;set;}
        public User Reviewer {get;set;} 
       
        // public User Reviewed{get;set;}
        
    }
}