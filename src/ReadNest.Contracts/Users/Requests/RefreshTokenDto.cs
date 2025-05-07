namespace ReadNest.Contracts.Users.Requests
{
    /// <summary>
    /// Refresh token DTO
    /// </summary>
    public class RefreshTokenDto
    {
        /// <summary>
        /// Refresh token
        /// </summary>
        public string Token { get; set; } = String.Empty;
    }
}
