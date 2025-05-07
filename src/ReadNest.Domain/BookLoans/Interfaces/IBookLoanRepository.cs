using ReadNest.Contracts.BookLoans.Requests;
using ReadNest.Domain.BookLoans.Entities;

namespace ReadNest.Domain.BookLoans.Interfaces
{
    public interface IBookLoanRepository
    {
        Task<(IEnumerable<BookLoan>, int)> GetBookLoansAsync(
            int? userId,
            int? bookId,
            bool? isDue,
            bool? isOverdue,
            string? searchQuery,
            int pageNumber,
            int pageSize);

        Task CreateBookLoanAsync(BookLoan bookLoan);

        void ReturnBookAsync(int id, BookReturnDto bookReturn);

        Task<bool> SaveChangesAsync();
    }
}
