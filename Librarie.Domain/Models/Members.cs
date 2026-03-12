using System.ComponentModel.DataAnnotations;

namespace CommunityLibrary.Models
{
    public class Member
    {
        [Key]
        public int Id { get; set; }
        

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }
        

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        

       [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;
       

        // Relationship: Member 1-* Loan
        // Navigation property for loans associated with this member
        public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>(); 
    }
}