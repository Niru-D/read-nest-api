namespace ReadNest.Contracts.Users.Requests
{
    /// <summary>
    /// User login DTO
    /// </summary>
    public class UserLoginDto
    {
        /// <summary>
        /// The primary email of the user
        /// </summary>
        public string Email { get; set; } = String.Empty;

        /// <summary>
        /// The password of the user
        /// </summary>
        public string Password { get; set; } = String.Empty;
    }
}
