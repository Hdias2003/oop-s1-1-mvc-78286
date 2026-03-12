using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommunityLibrary.Models
{
    public class Loan
    {
        [Key]
        public int Id { get; set; }
        

        [Required]
        public int BookId { get; set; }
        

        [Required]
        public int MemberId { get; set; }
        

        [Required]
        [DataType(DataType.Date)]
        public DateTime LoanDate { get; set; }
        

        [Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }
       

        [DataType(DataType.Date)]
        public DateTime? ReturnedDate { get; set; }
       

        // Relationships
        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }
       

        [ForeignKey("MemberId")]
        public virtual Member Member { get; set; }
        
    }
}