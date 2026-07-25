using ExpenseTracker.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    public class Expense
    {
        [Key]
        public int ExpenseId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Notes { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public string? ReceiptImage { get; set; }

        // Foreign Key
        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        // Identity User
        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
    }
}