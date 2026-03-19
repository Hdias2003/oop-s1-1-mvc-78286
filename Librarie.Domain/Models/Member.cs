
using System.ComponentModel.DataAnnotations;

namespace Library.Domain.Models;

    public class Member
    {
        [Key]
        public int Id { get; set; }
        

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty; 

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty; 

        [Phone]
        public string Phone { get; set; } = string.Empty; 

        // Relationship: A member can have many loans over time
        public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
        public DateTime JoinDate { get; set; } = DateTime.Now;


}
