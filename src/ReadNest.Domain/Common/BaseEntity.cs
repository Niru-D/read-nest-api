namespace ReadNest.Domain.Common
{
    /// <summary>
    /// The Base Entity
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// The object Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The object added date
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The object updated date
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
