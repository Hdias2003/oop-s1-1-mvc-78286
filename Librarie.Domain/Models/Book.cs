using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CommunityLibrary.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        

        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        

        [Required]
        [StringLength(100)]
        public string Author { get; set; }
      

        [Required]
        public string Isbn { get; set; }
        

        [Required]
        public string Category { get; set; }
       

        [Display(Name = "Available")]
        public bool IsAvailable { get; set; }
        

        // Relationship: Book 1-* Loan
        // Navigation property for related Loans
        public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}