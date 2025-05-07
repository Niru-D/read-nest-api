using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReadNest.API.ExceptionHandlers;
using ReadNest.Contracts.Users;
using ReadNest.Contracts.Users.Requests;
using ReadNest.Contracts.Users.Responses;
using ReadNest.Domain.Users.Interfaces;
using System.Security.Claims;
using System.Text.Json;

namespace ReadNest.API.Controllers
{
    /// <summary>
    /// This class defines the API endpoints related to User
    /// </summary>
    /// <remarks>
    /// - Admins can view, update and delete users
    /// - Members can only view and update their user details
    /// - Members can not delete user accounts
    /// </remarks>
    /// <response code="400">If the request is invalid</response>
    /// <response code="401">If the request is unauthenticated</response>
    /// <response code="403">If the request is forbidden</response>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly ILogger<UsersController> _logger;
        const int maxPageSize = 10;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="service">The service responsible for handling user-related operations</param>
        /// <param name="logger">The logger instance for logging messages</param>
        public UsersController(IUserService service, ILogger<UsersController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Gets a paginated list of users with optional filtering and search capabilities.
        /// </summary>
        /// <param name="firstName">Filter users by their first name (optional)</param>
        /// <param name="lastName">Filter users by their last name (optional)</param>
        /// <param name="email">Filter users by their email address (optional)</param>
        /// <param name="role">Filter users by their assigned role (optional)</param>
        /// <param name="searchQuery">A general search query that looks for matches in the first name, last name, email, address or contact number (optional)</param>
        /// <param name="pageNumber">The page number for pagination (default is 1)</param>
        /// <param name="pageSize">The number of users per page (default is 5)</param>
        /// <returns>Returns a paginated list of users that match the given filters and search criteria.</returns>
        /// <response code="200">If successful</response>
        [Authorize(Roles = nameof(Role.Admin))]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersAsync(
            string? firstName, 
            string? lastName, 
            string? email, 
            Role? role, 
            string? searchQuery, 
            int pageNumber = 1, 
            int pageSize = 5)
        {
            if (pageSize > maxPageSize)
            {
                _logger.LogInformation($"Allowed maximum page size is {maxPageSize}");
                pageSize = maxPageSize;
            }

            var (users, paginationMetadata) = await _service.GetUsersAsync(firstName, lastName, email, role, searchQuery, pageNumber, pageSize);
            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
            return Ok(users);
        }

        /// <summary>
        /// Gets a user by their unique identifier.
        /// </summary>
        /// <param name="id">The unique ID of the user</param>
        /// <returns>Returns the requested user if found; otherwise, returns a 404 Not Found response.</returns>
        /// <response code="200">If successful</response>
        /// <response code="404">If the user is not found</response>
        [HttpGet("{id}", Name = "GetUserById")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserByIdAsync(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if ((userRole == "LibraryMember") && (userIdClaim != id.ToString()))
            {
                _logger.LogError("Details about other members are not avialable to view.");
                return this.ErrorResponse(StatusCodes.Status403Forbidden, "Details about other members are not avialable to view.");
            }

            var user = await _service.GetUserByIdAsync(id);
            if (user == null)
            {
                _logger.LogError($"No user found with ID {id}.");
                return this.ErrorResponse(StatusCodes.Status404NotFound, $"No user found with ID {id}.");
            }
            return Ok(user);
        }

        /// <summary>
        /// Updates an existing user entry.
        /// </summary>
        /// <param name="id">The ID of the user to update</param>
        /// <param name="userToUpdate">The updated user details</param>
        /// <returns>Returns the updated user if successful.</returns>
        /// <response code="200">If successful</response>
        /// <response code="404">If the user is not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUserAsync(int id, UserUpdateDto userToUpdate)
        {
            if (id != userToUpdate.Id)
            {
                _logger.LogError($"The given user ID does not match with the ID of the user object.");
                return this.ErrorResponse(StatusCodes.Status400BadRequest, $"The given user ID does not match with the ID of the user object.");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if ((userRole == "LibraryMember") && (userIdClaim != id.ToString()))
            {
                _logger.LogError("Details about other members are not avialable to update.");
                return this.ErrorResponse(StatusCodes.Status403Forbidden, "Details about other members are not avialable to update.");
            }

            var userExist = await _service.UserExistsAsync(id);
            if (!userExist)
            {
                _logger.LogError($"No user found with ID {id}.");
                return this.ErrorResponse(StatusCodes.Status404NotFound, $"No user found with ID {id}.");
            }
            var updatedUser = await _service.UpdateUserAsync(userToUpdate);

            _logger.LogInformation($"The user with ID {id} has been successfully updated.");
            return Ok(updatedUser);
        }

        /// <summary>
        /// Deletes a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete</param>
        /// <returns>Returns a 204 No Content response if the deletion is successful.</returns>
        /// <response code="204">If successful</response>
        /// <response code="404">If the user is not found</response>
        [Authorize(Roles = nameof(Role.Admin))]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            var userExist = await _service.UserExistsAsync(id);
            if (!userExist)
            {
                _logger.LogError($"No user found with ID {id}.");
                return this.ErrorResponse(StatusCodes.Status404NotFound, $"No user found with ID {id}.");
            }
            await _service.DeleteUserAsync(id);
            _logger.LogInformation($"The user with ID {id} has been successfully deleted.");
            return NoContent();
        }
    }
}
