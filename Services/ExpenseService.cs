using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.ViewModels;

namespace ExpenseTracker.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly ApplicationDbContext _context;

        public ExpenseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories
                                 .OrderBy(c => c.Name)
                                 .ToListAsync();
        }

        public async Task<List<Expense>> GetAllExpensesAsync(string userId)
        {
            return await _context.Expenses
                .Where(e => e.UserId == userId)
                .Include(e => e.Category)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        public async Task<DashboardViewModel> GetDashboardDataAsync(string userId)
        {
            var expenses = await _context.Expenses
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.Date)
                .ToListAsync();

            var incomes = await _context.Incomes
                .Where(i => i.UserId == userId)
                .OrderByDescending(i => i.Date)
                .ToListAsync();

            return new DashboardViewModel
            {
                TotalIncome = incomes.Sum(i => i.Amount),

                TotalExpense = expenses.Sum(e => e.Amount),

                TotalTransactions = incomes.Count + expenses.Count,

                RecentExpenses = expenses.Take(5).ToList(),

                RecentIncomes = incomes.Take(5).ToList(),

                ExpenseCategoryChart = await GetExpenseCategoryChartAsync(userId)
            };
        }

        public async Task<Expense?> GetExpenseByIdAsync(int id, string userId)
        {
            return await _context.Expenses
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e =>
                    e.ExpenseId == id &&
                    e.UserId == userId);
        }

        public async Task AddExpenseAsync(Expense expense)
        {
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateExpenseAsync(Expense expense)
        {
            _context.Expenses.Update(expense);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteExpenseAsync(Expense expense)
        {
            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
        }


        public async Task<List<Income>> GetAllIncomesAsync(string userId)
        {
            return await _context.Incomes
                .Where(i => i.UserId == userId)
                .OrderByDescending(i => i.Date)
                .ToListAsync();
        }

        public async Task AddIncomeAsync(Income income)
        {
            _context.Incomes.Add(income);

            await _context.SaveChangesAsync();
        }

        public async Task<Income?> GetIncomeByIdAsync(int id, string userId)
        {
            return await _context.Incomes
                .FirstOrDefaultAsync(i =>
                    i.IncomeId == id &&
                    i.UserId == userId);
        }

        public async Task UpdateIncomeAsync(Income income)
        {
            _context.Incomes.Update(income);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteIncomeAsync(Income income)
        {
            _context.Incomes.Remove(income);

            await _context.SaveChangesAsync();
        }

        public async Task<List<ExpenseCategoryChartViewModel>> GetExpenseCategoryChartAsync(string userId)
        {
            return await _context.Expenses
                .Where(e => e.UserId == userId)
                .GroupBy(e => e.Category.Name)
                .Select(g => new ExpenseCategoryChartViewModel
                {
                    Category = g.Key,
                    TotalAmount = g.Sum(x => x.Amount)
                })
                .ToListAsync();
        }
    }
}