using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadNest.Contracts.Books
{
    /// <summary>
    /// Base Book DTO to include Book attributes
    /// </summary>
    public class BaseBookDto
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
        /// An optional description about the book
        /// </summary>
        public string? Description { get; set; }
    }
}
