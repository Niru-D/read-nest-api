using ReadNest.Contracts;
using ReadNest.Contracts.Books.Requests;
using ReadNest.Contracts.Books.Responses;

namespace ReadNest.Domain.Books.Interfaces
{
    public interface IBookService
    {
        Task<(IEnumerable<BookDto>, PaginationMetaData)> GetBooksAsync(
            string? title,
            string? author,
            string? genre,
            string? isbn,
            bool? isAvailable,
            string? searchQuery,
            int pageNumber,
            int pageSize);

        Task<BookDto?> GetBookByIdAsync(int id);

        Task<bool> BookExistsAsync(int id);

        Task<bool> IsBookAvailable(int id);

        Task<BookDto> CreateBookAsync(BookCreationDto book);

        Task<BookDto?> UpdateBookAsync(BookUpdateDto bookToUpdate);

        Task<bool> DeleteBookAsync(int id);
    }
}
