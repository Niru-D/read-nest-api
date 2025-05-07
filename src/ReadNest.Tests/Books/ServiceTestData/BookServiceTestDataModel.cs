using ReadNest.Contracts.Books.Requests;
using ReadNest.Contracts.Books.Responses;
using ReadNest.Domain.Books.Entities;

namespace ReadNest.Tests.Books.ServiceTestData
{
    /// <summary>
    /// Encapsulates all necessary test data for Book Service tests.
    /// </summary>
    public class BookServiceTestDataModel
    {
        public Book BookEntity { get; set; }

        public BookDto BookDto { get; set; }

        public BookCreationDto BookCreationDto { get; set; }

        public BookUpdateDto BookUpdateDto { get; set; }

        public BookDto UpdatedBookDto { get; set; }

        /// <summary>
        /// Constructor to initialize all test data objects.
        /// </summary>
        public BookServiceTestDataModel(Book bookEntity, BookDto bookDto, BookCreationDto bookCreationDto, BookUpdateDto bookUpdateDto, BookDto updatedBookDto)
        {
            BookEntity = bookEntity;
            BookDto = bookDto;
            BookCreationDto = bookCreationDto;
            BookUpdateDto = bookUpdateDto;
            UpdatedBookDto = updatedBookDto;
        }
    }
}
