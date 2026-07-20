using ExpenseTracker.Models;

namespace ExpenseTracker.ViewModels
{
    public class DashboardViewModel
    {
        public decimal TotalIncome { get; set; }

        public decimal TotalExpense { get; set; }

        public decimal Savings => TotalIncome - TotalExpense;

        public int TotalTransactions { get; set; }

        public List<Expense> RecentExpenses { get; set; } = new();

        public List<Income> RecentIncomes { get; set; } = new();
    }
}