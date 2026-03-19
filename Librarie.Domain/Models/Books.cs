using System.ComponentModel.DataAnnotations;

namespace Library.Domain.Models;

    public class Book
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty; 

        [Required]
        [StringLength(100)]
        public string Author { get; set; } = string.Empty; 

        [Required]
        public string Isbn { get; set; } = string.Empty; 

        [Required]
        public string Category { get; set; } = string.Empty; 

        public bool IsAvailable { get; set; } = true; 

        
        public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
