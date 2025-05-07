using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReadNest.Domain.Books.Entities;

namespace ReadNest.DataAccessLayer.EntityConfigurations
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("Books");
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Title).IsRequired();
            builder.Property(b => b.Author).IsRequired();
            builder.Property(b => b.Genre).IsRequired();
            builder.Property(b => b.ISBN).IsRequired();
            builder.Property(b => b.IsAvailable).HasDefaultValue(true);
            builder.Property(b => b.Description).HasMaxLength(500);
        } 
    }
}
