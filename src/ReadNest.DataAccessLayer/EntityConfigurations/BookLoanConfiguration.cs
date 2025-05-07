using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReadNest.Domain.BookLoans.Entities;

namespace ReadNest.DataAccessLayer.EntityConfigurations
{
    public class BookLoanConfiguration : IEntityTypeConfiguration<BookLoan>
    {
        public void Configure(EntityTypeBuilder<BookLoan> builder)
        {
            builder.ToTable("BookLoans");
            builder.HasKey(bl => bl.Id);

            builder.HasOne(bl => bl.User)
                .WithMany()
                .HasForeignKey(bl => bl.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(bl => bl.Book)
                .WithMany()
                .HasForeignKey(bl => bl.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(bl => bl.BorrowedDate)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(bl => bl.DueDate)
                .IsRequired()
                .HasComputedColumnSql("DATEADD(week, 1, BorrowedDate)");

            builder.Property(bl => bl.ReturnedDate)
                .IsRequired(false);
        }
    }
}
