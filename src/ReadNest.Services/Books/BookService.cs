using AutoMapper;
using ReadNest.Contracts;
using ReadNest.Contracts.Books.Requests;
using ReadNest.Contracts.Books.Responses;
using ReadNest.Domain.Books.Entities;
using ReadNest.Domain.Books.Interfaces;
using ReadNest.Services.Extensions;

namespace ReadNest.Services.Books
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _repository;
        private readonly IMapper _mapper;

        public BookService(IBookRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<(IEnumerable<BookDto>, PaginationMetaData)> GetBooksAsync(
            string? title,
            string? author,
            string? genre,
            string? isbn,
            bool? isAvailable,
            string? searchQuery,
            int pageNumber,
            int pageSize)
        {
            var (books, totalItemCount) = await _repository.GetBooksAsync(title, author, genre, isbn, isAvailable, searchQuery, pageNumber, pageSize);
            var paginationMetadata = new PaginationMetaData(totalItemCount, pageSize, pageNumber);

            return (_mapper.Map<IEnumerable<BookDto>>(books), paginationMetadata);
        }

        public async Task<BookDto?> GetBookByIdAsync(int id)
        {
            var book = await _repository.GetBookByIdAsync(id);
            return _mapper.Map<BookDto>(book);
        }

        public async Task<bool> BookExistsAsync(int id)
        {
            return await _repository.BookExistsAsync(id);
        }

        public async Task<bool> IsBookAvailable(int id)
        {
            return await _repository.IsBookAvailable(id);
        }

        public async Task<BookDto> CreateBookAsync(BookCreationDto book)
        {
            var bookEntity = _mapper.Map<Book>(book);
            bookEntity.ISBN = bookEntity.ISBN.FormatISBN();

            await _repository.CreateBookAsync(bookEntity);
            await _repository.SaveChangesAsync();

            var createdBook = _mapper.Map<BookDto>(bookEntity);
            return createdBook;
        }

        public async Task<BookDto?> UpdateBookAsync(BookUpdateDto bookToUpdate)
        {
            var existingBook = await _repository.GetBookByIdAsync(bookToUpdate.Id);
            
            if (existingBook == null)
            {
                return null;
            }

            existingBook.Title = bookToUpdate.Title;
            existingBook.Author = bookToUpdate.Author;
            existingBook.Genre = bookToUpdate.Genre;
            existingBook.ISBN = bookToUpdate.ISBN.FormatISBN();
            existingBook.Description = bookToUpdate.Description;
            existingBook.IsAvailable = bookToUpdate.IsAvailable ?? true;

            _repository.UpdateBook(existingBook);
            await _repository.SaveChangesAsync();

            return _mapper.Map<BookDto>(existingBook);
            
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _repository.GetBookByIdAsync(id);

            if (book == null)
            {
                return false;
            }
            _repository.DeleteBook(book);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
