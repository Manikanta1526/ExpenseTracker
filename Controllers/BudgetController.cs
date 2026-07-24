using ExpenseTracker.Models;
using ExpenseTracker.Services;
using ExpenseTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class BudgetController : Controller
    {
        private readonly IExpenseService _expenseService;

        public BudgetController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var budget = await _expenseService.GetCurrentBudgetAsync(userId);

            var model = new BudgetViewModel();

            if (budget != null)
            {
                model.MonthlyBudget = budget.MonthlyBudget;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(BudgetViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var budget = new Budget
            {
                MonthlyBudget = model.MonthlyBudget,
                Month = DateTime.Now.Month,
                Year = DateTime.Now.Year,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!
            };

            await _expenseService.SaveBudgetAsync(budget);

            TempData["Success"] = "Budget saved successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}