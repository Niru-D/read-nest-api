using ReadNest.Contracts.Books.Requests;
using ReadNest.Contracts.Books.Responses;
using ReadNest.Domain.Books.Entities;
using System.Collections;

namespace ReadNest.Tests.Books.ServiceTestData
{
    public class BookServiceTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            var bookEntity = new Book
            {
                Id = 1,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Title = "Percy Jackson and the Lightning Thief",
                Author = "Rick Riordan",
                Genre = "Mythology",
                ISBN = "978-0786838653",
                Description = "Test description",
                IsAvailable = true
            };
            var bookDto = new BookDto
            {
                Id = 1,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Title = "Percy Jackson and the Lightning Thief",
                Author = "Rick Riordan",
                Genre = "Mythology",
                ISBN = "978-0786838653",
                Description = "Test description",
                IsAvailable = true
            };
            var bookCreationDto = new BookCreationDto
            {
                Title = "Percy Jackson and the Lightning Thief",
                Author = "Rick Riordan",
                Genre = "Mythology",
                ISBN = "9780786838653",
                Description = "Test description"
            };
            var bookUpdateDto = new BookUpdateDto
            {
                Id = 1,
                Title = "Percy Jackson and the Lightning Thief - Updated Book",
                Author = "Rick Riordan - Updated",
                Genre = "Mythology - Updated",
                ISBN = "123456",
                Description = "Updated test description",
                IsAvailable = true
            };
            var updatedBookDto = new BookDto
            {
                Id = 1,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Title = "Percy Jackson and the Lightning Thief - Updated Book",
                Author = "Rick Riordan - Updated",
                Genre = "Mythology - Updated",
                ISBN = "123456",
                Description = "Updated test description",
                IsAvailable = true
            };
            yield return new object[] { new BookServiceTestDataModel(bookEntity, bookDto, bookCreationDto, bookUpdateDto, updatedBookDto) };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
