using ExpenseTracker.Models;
using ExpenseTracker.Services;
using ExpenseTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Differencing;
using System.Security.Claims;

namespace ExpenseTracker.Controllers
{
    [Authorize]
    public class ExpenseController : Controller
    {
        private readonly IExpenseService _expenseService;

        public ExpenseController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = new SelectList(
                await _expenseService.GetCategoriesAsync(),
                "CategoryId",
                "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpenseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(
                    await _expenseService.GetCategoriesAsync(),
                    "CategoryId",
                    "Name");

                return View(model);
            }

            var expense = new Expense
            {
                Title = model.Title,
                Amount = model.Amount,
                CategoryId = model.CategoryId,
                Date = model.Date,
                Notes = model.Notes,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            await _expenseService.AddExpenseAsync(expense);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var expense = await _expenseService.GetExpenseByIdAsync(id, userId);

            if (expense == null)
            {
                return NotFound();
            }

            return View(expense);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int expenseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var expense = await _expenseService.GetExpenseByIdAsync(expenseId, userId);

            if (expense == null)
            {
                return NotFound();
            }

            await _expenseService.DeleteExpenseAsync(expense);

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var expense = await _expenseService.GetExpenseByIdAsync(id, userId);

            if (expense == null)
            {
                return NotFound();
            }

            var model = new ExpenseViewModel
            {
                ExpenseId = expense.ExpenseId,
                Title = expense.Title,
                Amount = expense.Amount,
                CategoryId = expense.CategoryId,
                Date = expense.Date,
                Notes = expense.Notes
            };

            ViewBag.Categories = new SelectList(
                await _expenseService.GetCategoriesAsync(),
                "CategoryId",
                "Name",
                expense.CategoryId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ExpenseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(
                    await _expenseService.GetCategoriesAsync(),
                    "CategoryId",
                    "Name",
                    model.CategoryId);

                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var expense = await _expenseService.GetExpenseByIdAsync(model.ExpenseId, userId);

            if (expense == null)
            {
                return NotFound();
            }

            expense.Title = model.Title;
            expense.Amount = model.Amount;
            expense.CategoryId = model.CategoryId;
            expense.Date = model.Date;
            expense.Notes = model.Notes;

            await _expenseService.UpdateExpenseAsync(expense);

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Index(ExpenseFilterViewModel filter)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var model = await _expenseService.GetFilteredExpensesAsync(userId, filter);

            return View(model);
        }
    }
}