namespace ReadNest.Contracts.BookLoans
{
    /// <summary>
    /// Base Book DTO to include book loan details
    /// </summary>
    public class BaseBookLoanDto
    {
        /// <summary>
        /// The related user
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The related book
        /// </summary>
        public int BookId { get; set; }

    }
}
