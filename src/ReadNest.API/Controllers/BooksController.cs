using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReadNest.API.ExceptionHandlers;
using ReadNest.Contracts.Books.Requests;
using ReadNest.Contracts.Books.Responses;
using ReadNest.Contracts.Users;
using ReadNest.Domain.Books.Interfaces;
using System.Text.Json;

namespace ReadNest.API.Controllers
{
    /// <summary>
    /// This class defines the API endpoints related to Book
    /// </summary>
    /// <remarks>
    /// - Any user can view books
    /// - Only admins can create, update or delete books
    /// </remarks>
    /// <response code="400">If the request is invalid</response>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _service;
        private readonly ILogger<BooksController> _logger;
        const int maxPageSize = 10;

        /// <summary>
        /// Initializes a new instance of the <see cref="BooksController"/> class.
        /// </summary>
        /// <param name="service">The service responsible for handling book-related operations</param>
        /// <param name="logger">The logger instance for logging messages</param>
        public BooksController(IBookService service, ILogger<BooksController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Gets a paginated list of books with optional filtering and search capabilities.
        /// </summary>
        /// <param name="title">Filter books by title (optional)</param>
        /// <param name="author">Filter books by author name (optional)</param>
        /// <param name="genre">Filter books by genre (optional)</param>
        /// <param name="isbn">Filter books by ISBN number (optional)</param>
        /// <param name="isAvailable">Filter books by availability status (optional)</param>
        /// <param name="searchQuery">A general search query that looks for matches in the title, author, genre, ISBN or description(optional)</param>
        /// <param name="pageNumber">The page number for pagination (default is 1)</param>
        /// <param name="pageSize">The number of books per page (default is 5)</param>
        /// <returns>Returns a paginated list of books that match the given filters and search criteria.</returns>
        /// <response code="200">If successful</response>
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BookDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksAsync(
            string? title, 
            string? author, 
            string? genre, 
            string? isbn,
            bool? isAvailable,
            string? searchQuery,
            int pageNumber = 1,
            int pageSize = 5)
        {
            if (pageSize > maxPageSize)
            {
                _logger.LogInformation($"Allowed maximum page size is {maxPageSize}");
                pageSize = maxPageSize;
            }

            var (books, paginationMetadata) = await _service.GetBooksAsync(title, author, genre, isbn, isAvailable ,searchQuery, pageNumber, pageSize);
            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
            return Ok(books);
        }

        /// <summary>
        /// Gets a book by its unique identifier.
        /// </summary>
        /// <param name="id">The unique ID of the book</param>
        /// <returns>Returns the requested book if found; otherwise, returns a 404 Not Found response.</returns>
        /// <response code="200">If successful</response>
        /// <response code="404">If the book is not found</response>
        [AllowAnonymous]
        [HttpGet("{id}", Name = "GetBookById")]
        [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBookByIdAsync(int id)
        {
            var book = await _service.GetBookByIdAsync(id);
            if (book == null)
            {
                _logger.LogError($"No book found with ID {id}.");
                return this.ErrorResponse(StatusCodes.Status404NotFound, $"No book found with ID {id}.");
            }
            return Ok(book);
        }

        /// <summary>
        /// Creates a new book entry.
        /// </summary>
        /// <param name="book">The book details to create</param>
        /// <returns>Returns the created book along with a 201 Created response if successful.</returns>
        /// <response code="201">If successful</response>
        /// <response code="401">If the request is unauthenticated</response>
        /// <response code="403">If the request is forbidden</response>
        [Authorize(Roles = nameof(Role.Admin))]
        [HttpPost]
        [ProducesResponseType(typeof(BookDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<BookDto>> CreateBookAsync(BookCreationDto book)
        {
            var createdBook = await _service.CreateBookAsync(book);

            _logger.LogInformation($"A book with ID {createdBook.Id} has been successfully created.");
            return CreatedAtRoute("GetBookById", new {id = createdBook.Id}, createdBook);
        }

        /// <summary>
        /// Updates an existing book entry.
        /// </summary>
        /// <param name="id">The ID of the book to update</param>
        /// <param name="bookToUpdate">The updated book details</param>
        /// <returns>Returns the updated book if successful.</returns>
        /// <response code="200">If successful</response>
        /// <response code="401">If the request is unauthenticated</response>
        /// <response code="403">If the request is forbidden</response>
        /// <response code="404">If the book is not found</response>
        [Authorize(Roles = nameof(Role.Admin))]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BookDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateBookAsync(int id, BookUpdateDto bookToUpdate)
        {
            if (id != bookToUpdate.Id)
            {
                _logger.LogError($"The given book ID does not match with the ID of the book object.");
                return this.ErrorResponse(StatusCodes.Status400BadRequest, $"The given book ID does not match with the ID of the book object.");
            }
            var bookExist = await _service.BookExistsAsync(id);
            if (!bookExist)
            {
                _logger.LogError($"No book found with ID {id}.");
                return this.ErrorResponse(StatusCodes.Status404NotFound, $"No book found with ID {id}.");
            }
            var updatedBook = await _service.UpdateBookAsync(bookToUpdate);

            _logger.LogInformation($"The book with ID {id} has been successfully updated.");
            return Ok(updatedBook);
        }

        /// <summary>
        /// Deletes a book by its ID.
        /// </summary>
        /// <param name="id">The ID of the book to delete</param>
        /// <returns>Returns a 204 No Content response if the deletion is successful.</returns>
        /// <response code="204">If successful</response>
        /// <response code="401">If the request is unauthenticated</response>
        /// <response code="403">If the request is forbidden</response>
        /// <response code="404">If the book is not found</response>
        [Authorize(Roles = nameof(Role.Admin))]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBookAsync(int id)
        {
            var bookExist = await _service.BookExistsAsync(id);
            if (!bookExist)
            {
                _logger.LogError($"No book found with ID {id}.");
                return this.ErrorResponse(StatusCodes.Status404NotFound, $"No book found with ID {id}.");
            }
            await _service.DeleteBookAsync(id);
            _logger.LogInformation($"The book with ID {id} has been successfully deleted.");
            return NoContent();
        }
    }
}
