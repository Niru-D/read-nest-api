using ReadNest.Contracts.BookLoans.Requests;
using ReadNest.Contracts.BookLoans.Responses;
using ReadNest.Contracts.Books.Responses;
using ReadNest.Domain.BookLoans.Entities;
using System.Collections;

namespace ReadNest.Tests.BookLoans.ServiceTestData
{
    public class BookLoanServiceTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var bookLoanEntity = new BookLoan
            {
                Id = 1,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                UserId = 1,
                BookId = 2,
                BorrowedDate = DateTime.Now,
                DueDate = DateTime.Today.AddDays(14),
                ReturnedDate = null
            };
            var bookLoanWithBookDetailsDto = new BookLoanWithBookDetailsDto
            {
                Id = 1,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                UserId = 1,
                BookId = 2,
                BorrowedDate = DateTime.Now,
                DueDate = DateTime.Today.AddDays(14),
                ReturnedDate = null,
                Book = new BookDto
                {
                    Id = 1,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Title = "Percy Jackson and the Lightning Thief",
                    Author = "Rick Riordan",
                    Genre = "Mythology",
                    ISBN = "978-0786838653",
                    Description = "Test description",
                    IsAvailable = true
                }
            };
            var bookLoanCreationDto = new BookLoanCreationDto
            {
                UserId = 1,
                BookId = 2
            };
            var bookLoanDto = new BookLoanDto
            {
                Id = 1,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                UserId = 1,
                BookId = 2,
                BorrowedDate = DateTime.Now,
                DueDate = DateTime.Today.AddDays(14),
                ReturnedDate = null,
            };
            var bookReturnDto = new BookReturnDto
            {
                UserId = 1,
                BookId = 2
            };
            yield return new object[] { new BookLoanServiceTestDataModel(bookLoanEntity, bookLoanWithBookDetailsDto, bookLoanCreationDto, bookLoanDto, bookReturnDto) };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
