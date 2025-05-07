namespace ReadNest.Contracts.Users.Requests
{
    /// <summary>
    /// User Update DTO
    /// </summary>
    public class UserUpdateDto : BaseUserDto
    {
        /// <summary>
        /// The id of the user
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The password of the user
        /// </summary>
        public string Password { get; set; } = String.Empty;
    }
}
