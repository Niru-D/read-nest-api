using ReadNest.Contracts.BookLoans.Requests;
using ReadNest.Contracts.BookLoans.Responses;
using ReadNest.Domain.BookLoans.Entities;

namespace ReadNest.Tests.BookLoans.ServiceTestData
{
    /// <summary>
    /// Encapsulates all necessary test data for Book Loan Service tests.
    /// </summary>
    public class BookLoanServiceTestDataModel
    {
        public BookLoan BookLoanEntity { get; set; }

        public BookLoanWithBookDetailsDto BookLoanWithBookDetailsDto { get; set; }

        public BookLoanCreationDto BookLoanCreationDto { get; set; }

        public BookLoanDto BookLoanDto { get; set; }

        public BookReturnDto BookReturnDto { get; set; }

        /// <summary>
        /// Constructor to initialize all test data objects.
        /// </summary>
        public BookLoanServiceTestDataModel(BookLoan bookLoanEntity, BookLoanWithBookDetailsDto bookLoanWithBookDetailsDto, BookLoanCreationDto bookLoanCreationDto, BookLoanDto bookLoanDto, BookReturnDto bookReturnDto)
        {
            BookLoanEntity = bookLoanEntity;
            BookLoanWithBookDetailsDto = bookLoanWithBookDetailsDto;
            BookLoanCreationDto = bookLoanCreationDto;
            BookLoanDto = bookLoanDto;
            BookReturnDto = bookReturnDto;
        }
    }
}
