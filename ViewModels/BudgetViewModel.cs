using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.ViewModels
{
    public class BudgetViewModel
    {
        [Required]
        public decimal MonthlyBudget { get; set; }
    }
}