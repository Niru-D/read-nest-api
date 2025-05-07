using AutoMapper;
using ReadNest.Contracts;
using ReadNest.Contracts.BookLoans.Requests;
using ReadNest.Contracts.BookLoans.Responses;
using ReadNest.Domain.BookLoans.Entities;
using ReadNest.Domain.BookLoans.Interfaces;
using ReadNest.Domain.Books.Interfaces;

namespace ReadNest.Services.BookLoans
{
    public class BookLoanService : IBookLoanService
    {
        private readonly IBookLoanRepository _bookLoanRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public BookLoanService(IBookLoanRepository bookLoanRepository, IBookRepository bookRepository, IMapper mapper)
        {
            _bookLoanRepository = bookLoanRepository;
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public async Task<(IEnumerable<BookLoanWithBookDetailsDto>, PaginationMetaData)> GetBookLoansAsync(int? userId, int? bookId, bool? isDue, bool? isOverdue, string? searchQuery, int pageNumber, int pageSize)
        {
            var (bookLoans, totalItemCount) = await _bookLoanRepository.GetBookLoansAsync(userId, bookId, isDue, isOverdue, searchQuery, pageNumber, pageSize);
            Console.WriteLine(bookLoans);
            var paginationMetadata = new PaginationMetaData(totalItemCount, pageSize, pageNumber);

            return (_mapper.Map<IEnumerable<BookLoanWithBookDetailsDto>>(bookLoans), paginationMetadata);
        }

        public async Task<BookLoanDto> CreateBookLoanAsync(BookLoanCreationDto bookLoan)
        {
            var bookLoanEntity = _mapper.Map<BookLoan>(bookLoan);

            await _bookLoanRepository.CreateBookLoanAsync(bookLoanEntity);
            await _bookLoanRepository.SaveChangesAsync();

            _bookRepository.UpdateAvailability(bookLoan.BookId, false);
            await _bookRepository.SaveChangesAsync();

            var createdBookLoan = _mapper.Map<BookLoanDto>(bookLoanEntity);
            return createdBookLoan;
        }

        public async Task ReturnBookAsync(int id, BookReturnDto bookReturn)
        {
            _bookLoanRepository.ReturnBookAsync(id, bookReturn);
            await _bookLoanRepository.SaveChangesAsync();

            _bookRepository.UpdateAvailability(bookReturn.BookId, true);
            await _bookRepository.SaveChangesAsync();
        }
    }
}
