using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace Library_Project_Management.Models
{
    public class Borrowing_Record
    {
        [Key]
        public int RecordId { get; set; }
        public int BookId { get; set; }
        public int MemberId { get; set; }
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(15);
        public bool Returned { get; set; }=false;
        public DateTime BorrowDate { get; set; }=DateTime.Now;
        [Precision(10, 2)]
        public decimal LateFee { get; set; } = 0;


        public virtual Book  Book { get; set; }
        public virtual Member Member { get; set; }

    }
    public class BorrowingRecordDto
    {
        public int RecordId { get; set; }
        [Required]
        public string BookTitle { get; set; } = string.Empty;
        [Required]
        public string MemberName { get; set; } = string.Empty;
       
        public DateTime DueDate { get; set; }
        [Required]
        public string Status { get; set; }= string.Empty;
        public decimal LateFee { get; set; }

    }
}
