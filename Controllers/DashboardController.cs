using ExpenseTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IExpenseService _expenseService;

        public DashboardController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var dashboard = await _expenseService.GetDashboardDataAsync(userId!);

            // TEMPORARY DEBUG
            foreach (var item in dashboard.MonthlyExpenseChart)
            {
                Console.WriteLine($"{item.Month} - {item.TotalExpense}");
            }

            return View(dashboard);
        }

        // Placeholder action for Excel Export
public async Task<IActionResult> ExportExcel()
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    var file = await _expenseService.ExportDashboardToExcelAsync(userId);

    return File(
        file,
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        $"DashboardReport_{DateTime.Now:yyyyMMdd}.xlsx");
}
        // Placeholder action for PDF Export
        public IActionResult ExportPdf()
        {
            return Content("PDF export coming next.");
        }
    }
}