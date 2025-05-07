using ReadNest.Domain.Books.Entities;

namespace ReadNest.Domain.Books.Interfaces
{
    public interface IBookRepository
    {
        Task<(IEnumerable<Book>, int)> GetBooksAsync(
            string? title,
            string? author,
            string? genre,
            string? isbn,
            bool? isAvailable,
            string? searchQuery,
            int pageNumber,
            int pageSize);

        Task<Book?> GetBookByIdAsync(int id);

        Task<bool> BookExistsAsync(int id);

        Task<bool> IsBookAvailable(int id);

        Task CreateBookAsync(Book book);

        void UpdateBook(Book book);

        void UpdateAvailability(int id, bool isAvailable);

        void DeleteBook(Book book);

        Task<bool> SaveChangesAsync();
    }
}
