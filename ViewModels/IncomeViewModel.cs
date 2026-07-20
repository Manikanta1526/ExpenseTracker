using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.ViewModels
{
    public class IncomeViewModel
    {
        public int IncomeId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(100)]
        public string Source { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;
    }
}