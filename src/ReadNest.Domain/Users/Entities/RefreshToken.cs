namespace ReadNest.Domain.Users.Entities
{
    /// <summary>
    /// The refresh token entity
    /// </summary>
    public class RefreshToken
    {
        /// <summary>
        /// The token ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The related user's ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The refresh token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// The expiry date of the token
        /// </summary>
        public DateTime ExpiryDate { get; set; }

        /// <summary>
        /// Whether the token is revoked or not
        /// </summary>
        public bool IsRevoked { get; set; }

        /// <summary>
        /// Whether the token is used or not
        /// </summary>
        public bool IsUsed { get; set; }

        /// <summary>
        /// The related user
        /// </summary>
        public User User { get; set; }
    }
}
