using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReadNest.API.ExceptionHandlers;
using ReadNest.Contracts.Users.Requests;
using ReadNest.Contracts.Users.Responses;
using ReadNest.Domain.Users.Interfaces;
using System.Security.Claims;

namespace ReadNest.API.Controllers
{
    /// <summary>
    /// This class defines the API endpoints related to authentication
    /// </summary>
    /// <response code="400">If the request is invalid</response>
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="authService">The service responsible for handling authentication related operations</param>
        /// <param name="logger">The logger instance for logging messages</param>
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Registers users.
        /// </summary>
        /// <param name="userDto">The user details required for registration</param>
        /// <returns>Returns an HTTP 200 OK response with user details if successful.</returns>
        /// <response code="200">If successful</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> RegisterUserAsync(UserCreationDto userDto)
        {
            var result = await _authService.RegisterUserAsync(userDto);
            if (result == null)
            {
                _logger.LogError($"User registration failed : A user with email {userDto.Email} already exists.");
                return this.ErrorResponse(StatusCodes.Status400BadRequest, $"User registration failed : A user with email {userDto.Email} already exists.");
            }
                
            _logger.LogInformation($"A user with ID {result.Id} has been successfully registered.");
            return Ok(result);
        }

        /// <summary>
        /// Log in users.
        /// </summary>
        /// <param name="userDto">The user details required for login</param>
        /// <returns>Returns an HTTP 200 OK response with user details if successful.</returns>
        /// <response code="200">If successful</response>
        /// <response code="401">If the user credentials are invalid</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> LoginUserAsync(UserLoginDto userDto)
        {
            var result = await _authService.LoginUserAsync(userDto);
            if (result == null)
            {
                _logger.LogError("Invalid email or password.");
                return this.ErrorResponse(StatusCodes.Status401Unauthorized, "Invalid email or password.");
            }

            _logger.LogInformation($"The user with ID {result.Id} is successfully logged in.");
            return Ok(result);
        }

        /// <summary>
        /// Creates and returns a new access token and a new refresh token.
        /// </summary>
        /// <param name="refreshTokenRequest">Refresh token</param>
        /// <returns>Returns an HTTP 200 OK response with user details and tokens if successful.</returns>
        /// <response code="200">If successful</response>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> RefreshTokenAsync(RefreshTokenDto refreshTokenRequest)
        {
            var result = await _authService.RefreshTokenAsync(refreshTokenRequest.Token);

            if (result == null)
            {
                return this.ErrorResponse(StatusCodes.Status400BadRequest, "Invalid or expired refresh token.");
            }

            return Ok(result);
        }

        /// <summary>
        /// Log out users.
        /// </summary>
        /// <returns>Returns an HTTP 200 OK response, if successful.</returns>
        /// <response code="200">If successful</response>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> LogoutUserAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userIdClaim))
            {
                if (int.TryParse(userIdClaim, out int parsedId))
                {
                    await _authService.LogoutUserAsync(parsedId);
                    return Ok(new { message = "Logged out successfully." });
                }
                else
                {
                    return this.ErrorResponse(StatusCodes.Status400BadRequest, "Invalid user ID.");
                }
            }
            else
            {
                return this.ErrorResponse(StatusCodes.Status400BadRequest, "Invalid access token.");
            }
        }
    }
}
