
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Domain.Models;

    public class Loan
    {
        [Key]
        public int Id { get; set; }
        

        [Required]
        public int BookId { get; set; }
        

        [ForeignKey("BookId")]
        public virtual Book? Book { get; set; }
       

        [Required]
        public int MemberId { get; set; }
       

        [ForeignKey("MemberId")]
        public virtual Member? Member { get; set; }
       

        [Required]
        [Display(Name = "Loan Date")]
        public DateTime LoanDate { get; set; } = DateTime.Now; 

        [Required]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }
        

        [Display(Name = "Returned Date")]
        public DateTime? ReturnedDate { get; set; }

    public bool IsReturned { get; set; } = false; // Add this line

}
