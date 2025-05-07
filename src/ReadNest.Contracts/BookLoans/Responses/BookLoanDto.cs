namespace ReadNest.Contracts.BookLoans.Responses
{
    /// <summary>
    /// Book loan DTO
    /// </summary>
    public class BookLoanDto : BaseBookLoanDto
    {
        /// <summary>
        /// The book loan Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The book loan added date
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The book loan updated date
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// The borrowed date of the book
        /// </summary>
        public DateTime BorrowedDate { get; set; }

        /// <summary>
        /// The due date of the book
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// The returned date of the book, if the book is returned
        /// </summary>
        public DateTime? ReturnedDate { get; set; }
    }
}
