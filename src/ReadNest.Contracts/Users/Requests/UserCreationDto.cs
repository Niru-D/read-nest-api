namespace ReadNest.Contracts.Users.Requests
{
    /// <summary>
    /// User Creation DTO
    /// </summary>
    public class UserCreationDto : BaseUserDto
    {
        /// <summary>
        /// The password of the user
        /// </summary>
        public string Password { get; set; } = String.Empty;
    }
}
