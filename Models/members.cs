using System.ComponentModel.DataAnnotations;

namespace Library_Project_Management.Models
{
    public class members
    {
        [Key ]
            public int MemberId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
           public string Email { get; set; }
           public string Phone { get; set; }
    }
}
