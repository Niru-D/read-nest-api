using ReadNest.Domain.Common;


namespace ReadNest.Domain.Books.Entities
{
    /// <summary>
    /// The book entity
    /// </summary>
    public class Book : BaseEntity
    {
        /// <summary>
        /// The title of the book
        /// </summary>
        public string Title { get; set; } = String.Empty;

        /// <summary>
        /// The author of the book
        /// </summary>
        public string Author { get; set; } = String.Empty;

        /// <summary>
        /// The genre of the book
        /// </summary>
        public string Genre { get; set; } = String.Empty;

        /// <summary>
        /// The ISBN of the book
        /// </summary>
        public string ISBN { get; set; } = String.Empty;

        /// <summary>
        /// The availability of the book
        /// </summary>
        public bool? IsAvailable { get; set; }

        /// <summary>
        /// An optional description about the book
        /// </summary>
        public string? Description { get; set; }

    }
}
