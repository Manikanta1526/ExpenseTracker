using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.ViewModels;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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
                .Include(e => e.Category)
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.Date)
                .ToListAsync();

            var incomes = await _context.Incomes
                .Where(i => i.UserId == userId)
                .OrderByDescending(i => i.Date)
                .ToListAsync();

            var budget = await GetCurrentBudgetAsync(userId);



return new DashboardViewModel
{
    TotalIncome = incomes.Sum(i => i.Amount),
    TotalExpense = expenses.Sum(e => e.Amount),
    TotalTransactions = incomes.Count + expenses.Count,
    RecentExpenses = expenses.Take(5).ToList(),
    RecentIncomes = incomes.Take(5).ToList(),
    MonthlyExpenseChart = await GetMonthlyExpenseChartAsync(userId),
    ExpenseCategoryChart = await GetExpenseCategoryChartAsync(userId),
    MonthlyBudget = budget?.MonthlyBudget ?? 0,

    HighestExpense = expenses.Any()
    ? expenses.Max(e => e.Amount)
    : 0,

    AverageExpense = expenses.Any()
    ? expenses.Average(e => e.Amount)
    : 0,

    TopCategory = expenses.Any()
    ? expenses
        .GroupBy(e => e.Category!.Name)
        .OrderByDescending(g => g.Sum(x => x.Amount))
        .Select(g => g.Key)
        .FirstOrDefault() ?? "N/A"
    : "N/A",
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

        public async Task<Budget?> GetCurrentBudgetAsync(string userId)
        {
            return await _context.Budgets.FirstOrDefaultAsync(b =>
                b.UserId == userId &&
                b.Month == DateTime.Now.Month &&
                b.Year == DateTime.Now.Year);
        }

        public async Task SaveBudgetAsync(Budget budget)
        {
            var existing = await _context.Budgets.FirstOrDefaultAsync(b =>
                b.UserId == budget.UserId &&
                b.Month == budget.Month &&
                b.Year == budget.Year);

            if (existing == null)
            {
                _context.Budgets.Add(budget);
            }
            else
            {
                existing.MonthlyBudget = budget.MonthlyBudget;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<ExpenseFilterViewModel> GetFilteredExpensesAsync(
    string userId,
    ExpenseFilterViewModel filter)
        {
            var query = _context.Expenses
                .Include(e => e.Category)
                .Where(e => e.UserId == userId);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                query = query.Where(e =>
                    e.Title.Contains(filter.SearchTerm));
            }

            if (filter.CategoryId.HasValue)
            {
                query = query.Where(e =>
                    e.CategoryId == filter.CategoryId);
            }

            if (filter.StartDate.HasValue)
            {
                query = query.Where(e =>
                    e.Date >= filter.StartDate);
            }

            if (filter.EndDate.HasValue)
            {
                query = query.Where(e =>
                    e.Date <= filter.EndDate);
            }

            switch (filter.SortBy)
            {
                case "Amount":
                    query = query.OrderByDescending(e => e.Amount);
                    break;

                case "Date":
                default:
                    query = query.OrderByDescending(e => e.Date);
                    break;
            }

            var totalRecords = await query.CountAsync();

            filter.TotalPages = (int)Math.Ceiling(
                totalRecords / (double)filter.PageSize);

            filter.Expenses = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            filter.Categories = await GetCategoriesAsync();

            return filter;
        }

        public async Task<List<MonthlyExpenseChartViewModel>> GetMonthlyExpenseChartAsync(string userId)
        {
            var data = await _context.Expenses
                .Where(e => e.UserId == userId)
                .GroupBy(e => new { e.Date.Year, e.Date.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    TotalExpense = g.Sum(x => x.Amount)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync();

            return data.Select(x => new MonthlyExpenseChartViewModel
            {
                Month = new DateTime(x.Year, x.Month, 1).ToString("MMM yyyy"),
                TotalExpense = x.TotalExpense
            }).ToList();
        }



        public async Task<byte[]> ExportDashboardToExcelAsync(string userId)
        {
            var dashboard = await GetDashboardDataAsync(userId);

            using var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Dashboard Report");

            // ==========================
            // Report Title
            // ==========================
            worksheet.Cell("A1").Value = "Expense Tracker Dashboard Report";

            var title = worksheet.Range("A1:D1");
            title.Merge();
            title.Style.Font.Bold = true;
            title.Style.Font.FontSize = 20;
            title.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            title.Style.Fill.BackgroundColor = XLColor.LightBlue;
            title.Style.Font.FontColor = XLColor.DarkBlue;

            // ==========================
            // Summary Section
            // ==========================
            worksheet.Cell("A3").Value = "Total Income";
            worksheet.Cell("B3").Value = dashboard.TotalIncome;

            worksheet.Cell("A4").Value = "Total Expense";
            worksheet.Cell("B4").Value = dashboard.TotalExpense;

            worksheet.Cell("A5").Value = "Savings";
            worksheet.Cell("B5").Value = dashboard.Savings;

            worksheet.Cell("A6").Value = "Highest Expense";
            worksheet.Cell("B6").Value = dashboard.HighestExpense;

            worksheet.Cell("A7").Value = "Average Expense";
            worksheet.Cell("B7").Value = dashboard.AverageExpense;

            worksheet.Cell("A8").Value = "Top Category";
            worksheet.Cell("B8").Value = dashboard.TopCategory;

            // Summary Formatting
            worksheet.Range("A3:A8").Style.Font.Bold = true;
            worksheet.Range("B3:B7").Style.NumberFormat.Format = "₹#,##0.00";

            // ==========================
            // Category Breakdown Header
            // ==========================
            worksheet.Cell("A10").Value = "Category";
            worksheet.Cell("B10").Value = "Amount";

            var header = worksheet.Range("A10:B10");
            header.Style.Fill.BackgroundColor = XLColor.DarkBlue;
            header.Style.Font.FontColor = XLColor.White;
            header.Style.Font.Bold = true;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // ==========================
            // Category Breakdown Data
            // ==========================
            int row = 11;

            foreach (var item in dashboard.ExpenseCategoryChart)
            {
                worksheet.Cell(row, 1).Value = item.Category;
                worksheet.Cell(row, 2).Value = item.TotalAmount;
                row++;
            }

            // Currency Formatting
            worksheet.Range($"B11:B{row - 1}")
                     .Style.NumberFormat.Format = "₹#,##0.00";

            // Borders
            var tableRange = worksheet.Range($"A10:B{row - 1}");
            tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // Alignment
            worksheet.Column(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            worksheet.Column(2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            // Auto-size Columns
            worksheet.Columns().AdjustToContents();

            if (worksheet.Column(1).Width < 25)
                worksheet.Column(1).Width = 25;

            if (worksheet.Column(2).Width < 18)
                worksheet.Column(2).Width = 18;

            using var stream = new MemoryStream();

            workbook.SaveAs(stream);

            return stream.ToArray();
        }

        public async Task<byte[]> ExportDashboardToPdfAsync(string userId)
        {
            var dashboard = await GetDashboardDataAsync(userId);

            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header()
                        .Text("Expense Tracker Report")
                        .SemiBold()
                        .FontSize(24);

                    page.Content().Column(col =>
                    {
                        col.Spacing(8);

                        col.Item().Text($"Total Income : {dashboard.TotalIncome:C}");
                        col.Item().Text($"Total Expense : {dashboard.TotalExpense:C}");
                        col.Item().Text($"Savings : {dashboard.Savings:C}");
                        col.Item().Text($"Highest Expense : {dashboard.HighestExpense:C}");
                        col.Item().Text($"Average Expense : {dashboard.AverageExpense:C}");
                        col.Item().Text($"Top Category : {dashboard.TopCategory}");

                        col.Item().PaddingTop(15);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Category");
                                header.Cell().Text("Amount");
                            });

                            foreach (var item in dashboard.ExpenseCategoryChart)
                            {
                                table.Cell().Text(item.Category);
                                table.Cell().Text(item.TotalAmount.ToString("C"));
                            }
                        });
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text($"Generated : {DateTime.Now:dd MMM yyyy}");
                });
            }).GeneratePdf();
        }
    }
}