using ReadNest.Domain.Books.Entities;
using ReadNest.Domain.Common;
using ReadNest.Domain.Users.Entities;

namespace ReadNest.Domain.BookLoans.Entities
{
    /// <summary>
    /// The entity to represent the relationship between users and books
    /// </summary>
    public class BookLoan : BaseEntity
    {
        /// <summary>
        /// The related user
        /// </summary>
        public int UserId { get; set; }
        public User User { get; set; }

        /// <summary>
        /// The related book
        /// </summary>
        public int BookId { get; set; }
        public Book Book { get; set; }

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
