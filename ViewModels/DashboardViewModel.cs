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

        // NEW
        public List<ExpenseCategoryChartViewModel> ExpenseCategoryChart { get; set; } = new();

        public List<MonthlyExpenseChartViewModel> MonthlyExpenseChart { get; set; } = new();

        public decimal MonthlyBudget { get; set; }

        public decimal RemainingBudget => MonthlyBudget - TotalExpense;

        public double BudgetUsedPercentage =>
            MonthlyBudget == 0 ? 0 :
            (double)(TotalExpense / MonthlyBudget * 100);

        public bool IsBudgetExceeded =>
    TotalExpense > MonthlyBudget && MonthlyBudget > 0;

        public decimal HighestExpense { get; set; }

        public decimal AverageExpense { get; set; }

        public string TopCategory { get; set; } = string.Empty;
    }
}