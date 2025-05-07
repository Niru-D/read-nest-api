
namespace ReadNest.Contracts.Books.Responses
{
    /// <summary>
    /// Book DTO
    /// </summary>
    public class BookDto : BaseBookDto
    {
        /// <summary>
        /// The id of the book
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The added date of the book
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The updated date of the book
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// The availability of the book
        /// </summary>
        public bool? IsAvailable { get; set; }
    }
}
