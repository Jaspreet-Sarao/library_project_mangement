using System.ComponentModel.DataAnnotations;

namespace Library_Project_Management.Models
{
    public class Member
    {
        [Key]
        public int MemberId { get; set; }
        [Required]
        public string FirstName { get; set; }=string.Empty;
        [Required]
        public string LastName { get; set; }=string.Empty;
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, Phone]
        public string Phone { get; set; }=string.Empty;

        // A member can have many borrowing records
         public ICollection<Borrowing_Record> Borrowing_Records { get; set; } = new List<Borrowing_Record>();

        public string FullName => $"{FirstName} {LastName}";

    }
    public class MemberDto
    {
        public int MemberId { get; set; }
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string Email { get; set; } =string.Empty;
        [Required, Phone]
        public string Phone { get; set; } =string.Empty;
    }
}
