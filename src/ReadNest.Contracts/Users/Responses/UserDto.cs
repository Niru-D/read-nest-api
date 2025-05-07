namespace ReadNest.Contracts.Users.Responses
{
    /// <summary>
    /// User DTO
    /// </summary>
    public class UserDto : BaseUserDto
    {
        /// <summary>
        /// The id of the user
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The added date of the user
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The updated date of the user
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
