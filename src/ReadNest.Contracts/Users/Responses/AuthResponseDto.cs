namespace ReadNest.Contracts.Users.Responses
{
    /// <summary>
    /// The authentication response DTO
    /// </summary>
    public class AuthResponseDto
    {
        /// <summary>
        /// The generated access token
        /// </summary>
        public string Token { get; set; } = String.Empty;

        /// <summary>
        /// The generated refresh token
        /// </summary>
        public string RefreshToken { get; set; } = String.Empty;

        /// <summary>
        /// The user ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The primary email of the user
        /// </summary>
        public string Email { get; set; } = String.Empty;
    }
}
