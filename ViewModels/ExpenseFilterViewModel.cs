using ExpenseTracker.Models;

namespace ExpenseTracker.ViewModels
{
    public class ExpenseFilterViewModel
    {
        public string? SearchTerm { get; set; }

        public int? CategoryId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? SortBy { get; set; }

        public List<Category> Categories { get; set; } = new();

        public List<Expense> Expenses { get; set; } = new();

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int TotalPages { get; set; }
    }
}