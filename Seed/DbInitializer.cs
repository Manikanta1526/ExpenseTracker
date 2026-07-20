using ExpenseTracker.Data;
using ExpenseTracker.Models;

namespace ExpenseTracker.Seed
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Food", Icon = "fa-utensils" },
                    new Category { Name = "Transport", Icon = "fa-car" },
                    new Category { Name = "Shopping", Icon = "fa-cart-shopping" },
                    new Category { Name = "Bills", Icon = "fa-file-invoice" },
                    new Category { Name = "Education", Icon = "fa-book" },
                    new Category { Name = "Health", Icon = "fa-heart-pulse" },
                    new Category { Name = "Entertainment", Icon = "fa-film" },
                    new Category { Name = "Salary", Icon = "fa-money-bill" },
                    new Category { Name = "Other", Icon = "fa-list" }
                };

                context.Categories.AddRange(categories);
                context.SaveChanges();
            }
        }
    }
}