using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    public class Budget
    {
        [Key]
        public int BudgetId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyBudget { get; set; }

        public string Month { get; set; } = string.Empty;

        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
    }
}