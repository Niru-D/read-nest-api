using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using ReadNest.API.ExceptionHandlers;
using ReadNest.Contracts.BookLoans.Requests;
using ReadNest.Contracts.BookLoans.Responses;
using ReadNest.Contracts.Users;
using ReadNest.Domain.BookLoans.Interfaces;
using ReadNest.Domain.Books.Interfaces;
using System.Security.Claims;
using System.Text.Json;

namespace ReadNest.API.Controllers
{
    /// <summary>
    /// This class defines the API endpoints related to Book Loans
    /// </summary>
    /// <remarks>
    /// - Admins can view all book transactions
    /// - Members can only view their book transactions
    /// - Only members can borrow and return books
    /// </remarks>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If the request is unauthenticated</response>
    /// <response code="403">If the request is forbidden</response>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class BookLoansController : ControllerBase
    {
        private readonly IBookLoanService _bookLoanService;
        private readonly IBookService _bookService;
        private readonly ILogger<BookLoansController> _logger;
        const int maxPageSize = 10;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookLoansController"/> class.
        /// </summary>
        /// <param name="bookLoanService">The service responsible for handling book loan related operations</param>
        /// <param name="bookService">The service responsible for handling book related operations</param>
        /// <param name="logger">The logger instance for logging messages</param>
        public BookLoansController(IBookLoanService bookLoanService, IBookService bookService, ILogger<BookLoansController> logger)
        {
            _bookLoanService = bookLoanService;
            _bookService = bookService;
            _logger = logger;
        }

        /// <summary>
        /// Gets a paginated list of book loans with optional filtering by user, book, due status and search query.
        /// </summary>
        /// <param name="userId">Optional filter to retrieve book loans for a specific user</param>
        /// <param name="bookId">Optional filter to retrieve book loans for a specific book</param>
        /// <param name="isDue">Optional filter to retrieve due book loans</param>
        /// <param name="isOverdue">Optional filter to retrieve overdue book loans</param>
        /// <param name="searchQuery">Optional search term to filter book loans based on book titles</param>
        /// <param name="pageNumber">The page number for pagination (default is 1)</param>
        /// <param name="pageSize">The number of books per page (default is 5)</param>
        /// <returns>Returns a paginated list of book loans including book details.</returns>
        /// <response code="200">If successful</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BookLoanWithBookDetailsDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBookLoansAsync(
            int? userId,
            int? bookId,
            bool? isDue,
            bool? isOverdue,
            string? searchQuery,
            int pageNumber = 1,
            int pageSize = 5)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (userRole == "LibraryMember")
            {
                if ((userId != null) && (userIdClaim != userId.ToString()))
                {
                    _logger.LogError("Details about other members' book transactions are not avialable to view.");
                    return this.ErrorResponse(StatusCodes.Status403Forbidden, "Details about other members' book transactions are not avialable to view.");
                }
                if ((userId == null) && !string.IsNullOrEmpty(userIdClaim))
                {
                    if (int.TryParse(userIdClaim, out int parsedId))
                    {
                        userId = parsedId;
                    }
                }
            }

            if (pageSize > maxPageSize)
            {
                _logger.LogInformation($"Allowed maximum page size is {maxPageSize}");
                pageSize = maxPageSize;
            }

            var (bookLoans, paginationMetadata) = await _bookLoanService.GetBookLoansAsync(userId, bookId, isDue, isOverdue, searchQuery, pageNumber, pageSize);
            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
            return Ok(bookLoans);
        }

        /// <summary>
        /// Creates a new book borrowing transaction
        /// </summary>
        /// <param name="bookLoan">The details of the book loan</param>
        /// <returns>Returns the created book loan transaction details.</returns>
        /// <response code="200">If successful</response>
        [Authorize(Roles = nameof(Role.LibraryMember))]
        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<BookLoanDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateBookLoanAsync(BookLoanCreationDto bookLoan)
        {
            var isAvailable = await _bookService.IsBookAvailable(bookLoan.BookId);
            if (!isAvailable)
            {
                _logger.LogError($"The book with ID {bookLoan.BookId} is not available to borrow.");
                return this.ErrorResponse(StatusCodes.Status400BadRequest, $"The book with ID {bookLoan.BookId} is not available to borrow.");
            }
            
            var createdBookLoan = await _bookLoanService.CreateBookLoanAsync(bookLoan);
            _logger.LogInformation($"The book with id {bookLoan.BookId} has been borrowed by user with id {bookLoan.UserId}");
            return Ok(createdBookLoan);
        }

        /// <summary>
        /// Returns a borrowed book.
        /// </summary>
        /// <param name="bookReturn">The details about the book loan</param>
        /// <returns>Returns a 204 No Content response if the return is successful.</returns>
        /// <response code="204">If successful</response>
        /// <response code="404">If the book loan is not found or already returned</response>
        [Authorize(Roles = nameof(Role.LibraryMember))]
        [HttpPost("return")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReturnBookAsync(BookReturnDto bookReturn)
        {
            var (dueBookLoan, paginationMetaData) = await _bookLoanService.GetBookLoansAsync(bookReturn.UserId, bookReturn.BookId, true, null, null, 1, 1);
            if (!dueBookLoan.Any())
            {
                _logger.LogError($"The book loan does not exist or already returned.");
                return this.ErrorResponse(StatusCodes.Status404NotFound, $"The book loan does not exist or already returned.");
            }
            await _bookLoanService.ReturnBookAsync(dueBookLoan.First().Id ,bookReturn);
            return NoContent();
        }
    }
}
