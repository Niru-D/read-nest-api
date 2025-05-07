using ReadNest.Contracts;
using ReadNest.Contracts.BookLoans.Requests;
using ReadNest.Contracts.BookLoans.Responses;

namespace ReadNest.Domain.BookLoans.Interfaces
{
    public interface IBookLoanService
    {
        Task<(IEnumerable<BookLoanWithBookDetailsDto>, PaginationMetaData)> GetBookLoansAsync(
            int? userId,
            int? bookId,
            bool? isDue,
            bool? isOverdue,
            string? searchQuery,
            int pageNumber,
            int pageSize);

        Task<BookLoanDto> CreateBookLoanAsync(BookLoanCreationDto bookLoan);

        Task ReturnBookAsync(int id, BookReturnDto bookReturn);
    }
}
