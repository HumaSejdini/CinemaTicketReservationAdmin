using System;
using System.ComponentModel.DataAnnotations;

namespace LabAdminApplication.Models
{
    public class Movie
    {
        public int Id { get; set; }     
        public string MovieTitle { get; set; }      
        public string MovieDescription { get; set; }      
        public string MovieImage { get; set; }        
        public double MoviePrice { get; set; }      
        public int MovieRating { get; set; }
        public DateTime DateAndTime { get; set; }
    }
}
