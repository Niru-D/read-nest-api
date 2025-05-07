using Microsoft.EntityFrameworkCore;
using ReadNest.Domain.Books.Entities;
using ReadNest.Domain.Books.Interfaces;
using ReadNest.Services;

namespace ReadNest.DataAccessLayer.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;

        public BookRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Book>, int)> GetBooksAsync(
            string? title,
            string? author,
            string? genre,
            string? isbn,
            bool? isAvailable,
            string? searchQuery,
            int pageNumber,
            int pageSize)
        {
            var collection = _context.Books as IQueryable<Book>;
            if (!string.IsNullOrWhiteSpace(title))
            {
                title = title.Trim();
                collection = collection.Where(c => c.Title == title);
            }
            if (!string.IsNullOrWhiteSpace(author))
            {
                author = author.Trim();
                collection = collection.Where(c => c.Author == author);
            }
            if (!string.IsNullOrWhiteSpace(genre))
            {
                genre = genre.Trim();
                collection = collection.Where(c => c.Genre == genre);
            }
            if (!string.IsNullOrWhiteSpace(isbn))
            {
                isbn = isbn.Trim();
                collection = collection.Where(c => c.ISBN == isbn);
            }
            if (isAvailable.HasValue)
            {
                collection = collection.Where(c => c.IsAvailable == isAvailable);
            }
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(c => 
                    c.Title.Contains(searchQuery) || 
                    c.Author.Contains(searchQuery) || 
                    c.Genre.Contains(searchQuery) ||
                    c.ISBN.Contains(searchQuery) || 
                    (c.Description != null && c.Description.Contains(searchQuery))
                );
            }

            var totalItemCount = await collection.CountAsync();
            var booksCollection = await collection.Skip(pageSize*(pageNumber-1)).Take(pageSize).ToListAsync();

            return (booksCollection, totalItemCount);
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _context.Books.Where(b => b.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> BookExistsAsync(int id)
        {
            return await _context.Books.AnyAsync(b => b.Id == id);
        }

        public async Task<bool> IsBookAvailable(int id)
        {
            return (bool)await _context.Books.Where(b => b.Id == id)
                .Select(b => b.IsAvailable)
                .FirstOrDefaultAsync();
        }

        public async Task CreateBookAsync(Book book)
        {
            await _context.Books.AddAsync(book);
        }

        public void UpdateBook(Book book)
        {
            _context.Books.Update(book);
        }

        public void UpdateAvailability(int id, bool isAvailable)
        {
            var book = _context.Books.Find(id);
            if (book != null)
            {
                book.IsAvailable = isAvailable;
            }
        }

        public void DeleteBook(Book book)
        {
            _context.Books.Remove(book);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
