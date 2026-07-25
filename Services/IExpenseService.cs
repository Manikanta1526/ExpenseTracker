using ExpenseTracker.Models;
using ExpenseTracker.ViewModels;

namespace ExpenseTracker.Services
{
    public interface IExpenseService
    {
        Task<List<Expense>> GetAllExpensesAsync(string userId);

        Task<List<Category>> GetCategoriesAsync();

        Task AddExpenseAsync(Expense expense);

        Task<Expense?> GetExpenseByIdAsync(int id, string userId);

        Task UpdateExpenseAsync(Expense expense);

        Task DeleteExpenseAsync(Expense expense);

        Task<DashboardViewModel> GetDashboardDataAsync(string userId);

        Task<List<Income>> GetAllIncomesAsync(string userId);

        Task AddIncomeAsync(Income income);

        Task<Income?> GetIncomeByIdAsync(int id, string userId);

        Task UpdateIncomeAsync(Income income);

        Task DeleteIncomeAsync(Income income);

        Task<List<ExpenseCategoryChartViewModel>> GetExpenseCategoryChartAsync(string userId);

        Task<List<MonthlyExpenseChartViewModel>> GetMonthlyExpenseChartAsync(string userId);

        Task<Budget?> GetCurrentBudgetAsync(string userId);

        Task SaveBudgetAsync(Budget budget);

        Task<ExpenseFilterViewModel> GetFilteredExpensesAsync(
    string userId,
    ExpenseFilterViewModel filter);

       

        Task<byte[]> ExportDashboardToExcelAsync(string userId);

        Task<byte[]> ExportDashboardToPdfAsync(string userId);

    }
}