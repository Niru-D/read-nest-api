
namespace ReadNest.Contracts.Books.Requests
{
    /// <summary>
    /// Book Update DTO
    /// </summary>
    public class BookUpdateDto : BaseBookDto
    {
        /// <summary>
        /// The id of the book
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The availability of the book
        /// </summary>
        public bool? IsAvailable { get; set; }
    }
}
