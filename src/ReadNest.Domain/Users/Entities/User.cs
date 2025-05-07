using ReadNest.Contracts.Users;
using ReadNest.Domain.Common;

namespace ReadNest.Domain.Users.Entities
{
    /// <summary>
    /// The user entity
    /// </summary>
    public class User : BaseEntity
    {
        /// <summary>
        /// The firstname of the user
        /// </summary>
        public string FirstName { get; set; } = String.Empty;

        /// <summary>
        /// The lastname of the user
        /// </summary>
        public string LastName { get; set; } = String.Empty;

        /// <summary>
        /// The primary email of the user
        /// </summary>
        public string Email { get; set; } = String.Empty;

        /// <summary>
        /// The address of the user
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// The contact number of the user
        /// </summary>
        public string? ContactNumber { get; set; }

        /// <summary>
        /// The role of the user
        /// </summary>
        public Role Role { get; set; }

        /// <summary>
        /// The hashed password of the user
        /// </summary>
        public string Password { get; set; } = String.Empty;
    }
}
