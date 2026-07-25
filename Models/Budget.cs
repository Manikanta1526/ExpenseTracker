using ExpenseTracker.Data;
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

        public int Month { get; set; }

        public int Year { get; set; }

        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }
    }
}