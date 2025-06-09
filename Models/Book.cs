using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.Connections;

namespace Library_Project_Management.Models
{
    public class Book
    {
        [Key]

        public int BookId { get; set; }
        [Required]
        public string Title { get; set; }=string.Empty;
        [Required]
        public string Author { get; set; }= string.Empty;
        [Required]
        public string Genre { get; set; } = string.Empty;
        public  bool  Available { get; set;} =true;

        
            // A book can appear in many borrowing records
            public ICollection<Borrowing_Record> Borrowing_Records { get; set; } = new List<Borrowing_Record>();
    }
        public class BookDto
        {
        public int BookId { get; set; }
        [Required]
        public string Title { get; set; } =string.Empty;
        [Required]
        public string Author { get; set; } =string.Empty ;
        [Required]
        public string Status { get; set; } = string.Empty;
        
    }

}
