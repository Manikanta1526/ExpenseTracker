using ExpenseTracker.Models;
using ExpenseTracker.Services;
using ExpenseTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class IncomeController : Controller
    {
        private readonly IExpenseService _expenseService;

        public IncomeController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IncomeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var income = new Income
            {
                Source = model.Source,
                Amount = model.Amount,
                Date = model.Date,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            await _expenseService.AddIncomeAsync(income);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var income = await _expenseService.GetIncomeByIdAsync(id, userId);

            if (income == null)
            {
                return NotFound();
            }

            var model = new IncomeViewModel
            {
                IncomeId = income.IncomeId,
                Source = income.Source,
                Amount = income.Amount,
                Date = income.Date
            };

            return View(model);
        }
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var incomes = await _expenseService.GetAllIncomesAsync(userId);

            return View(incomes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IncomeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var income = await _expenseService.GetIncomeByIdAsync(model.IncomeId, userId);

            if (income == null)
            {
                return NotFound();
            }

            income.Source = model.Source;
            income.Amount = model.Amount;
            income.Date = model.Date;

            await _expenseService.UpdateIncomeAsync(income);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var income = await _expenseService.GetIncomeByIdAsync(id, userId);

            if (income == null)
                return NotFound();

            return View(income);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int incomeId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var income = await _expenseService.GetIncomeByIdAsync(incomeId, userId);

            if (income == null)
                return NotFound();

            await _expenseService.DeleteIncomeAsync(income);

            return RedirectToAction(nameof(Index));
        }
    }
}