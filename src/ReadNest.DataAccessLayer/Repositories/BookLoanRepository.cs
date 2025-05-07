using Microsoft.EntityFrameworkCore;
using ReadNest.Contracts.BookLoans.Requests;
using ReadNest.Domain.BookLoans.Entities;
using ReadNest.Domain.BookLoans.Interfaces;
using ReadNest.Domain.Books.Entities;

namespace ReadNest.DataAccessLayer.Repositories
{
    public class BookLoanRepository : IBookLoanRepository
    {
        private readonly ApplicationDbContext _context;

        public BookLoanRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<BookLoan>, int)> GetBookLoansAsync(int? userId, int? bookId, bool? isDue, bool? isOverdue, string? searchQuery, int pageNumber, int pageSize)
        {
            var collection = _context.BookLoans as IQueryable<BookLoan>;
            if (userId.HasValue && userId != null)
            {
                collection = collection.Where(b => b.UserId == userId);
            }
            if (bookId.HasValue && bookId != null)
            {
                collection = collection.Where(b => b.BookId == bookId);
            }
            if (isDue.HasValue)
            {
                if ((bool)isDue)
                {
                    collection = collection.Where(b => b.ReturnedDate == null);
                }
                else
                {
                    collection = collection.Where(b => b.ReturnedDate != null);
                }     
            }
            if (isOverdue.HasValue)
            {
                if ((bool)isOverdue)
                {
                    collection = collection.Where(b => b.ReturnedDate == null && (b.DueDate < DateTime.Now));
                }
                else
                {
                    collection = collection.Where(b => b.ReturnedDate == null && (b.DueDate > DateTime.Now));
                }
            }
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(b => b.Book.Title.Contains(searchQuery));
            }

            var totalItemCount = await collection.CountAsync();
            var bookLoansCollection = await collection.Skip(pageSize * (pageNumber - 1)).Take(pageSize).Include(b => b.Book).ToListAsync();

            return (bookLoansCollection, totalItemCount);
        }

        public async Task CreateBookLoanAsync(BookLoan bookLoan)
        {
            await _context.BookLoans.AddAsync(bookLoan);
        }

        public void ReturnBookAsync(int id, BookReturnDto bookReturn)
        {
            var bookLoan = _context.BookLoans.Find(id);
            if(bookLoan != null)
            {
                bookLoan.ReturnedDate = DateTime.Now;
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
