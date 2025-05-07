using AutoMapper;
using Moq;
using ReadNest.Contracts.Books.Responses;
using ReadNest.Domain.Books.Entities;
using ReadNest.Domain.Books.Interfaces;
using ReadNest.Services.Books;
using ReadNest.Tests.Books.ServiceTestData;

namespace ReadNest.Tests.Books.ServiceTests
{
    public class BookServiceTests
    {
        private readonly Mock<IBookRepository> _mockBookRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly BookService _bookService;

        public BookServiceTests()
        {
            _mockBookRepository = new Mock<IBookRepository>();
            _mockMapper = new Mock<IMapper>();
            _bookService = new BookService(_mockBookRepository.Object, _mockMapper.Object);
        }

        [Theory]
        [ClassData(typeof(BookServiceTestData))]
        public async Task GetBooksAsync_GetAllBooks_ShouldReturnBooksAndPaginationMetadata(BookServiceTestDataModel testData)
        {
            // Arrange
            var sampleBooks = new List<Book> { testData.BookEntity };
            var sampleBookDtos = new List<BookDto> { testData.BookDto };
            int totalItemCount = 1;
            int pageNumber = 1;
            int pageSize = 5;

            _mockBookRepository.Setup(r => r.GetBooksAsync(
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<bool?>(), It.IsAny<string?>(), pageNumber, pageSize
            )).ReturnsAsync((sampleBooks, totalItemCount));

            _mockMapper.Setup(m => m.Map<IEnumerable<BookDto>>(sampleBooks)).Returns(sampleBookDtos);

            // Act
            var (resultBooks, paginationMetadata) = await _bookService.GetBooksAsync(null, null, null, null, null, null, pageNumber, pageSize);

            // Assert
            Assert.NotNull(resultBooks);
            Assert.Single(resultBooks);
            Assert.Equal(1, paginationMetadata.TotalItemCount);
            Assert.Equal(pageNumber, paginationMetadata.CurrentPage);
            Assert.Equal(pageSize, paginationMetadata.PageSize);

            _mockBookRepository.Verify(r => r.GetBooksAsync(null, null, null, null, null, null, pageNumber, pageSize), Times.Once);
            _mockMapper.Verify(m => m.Map<IEnumerable<BookDto>>(sampleBooks), Times.Once);
        }


        [Theory]
        [ClassData(typeof(BookServiceTestData))]
        public async Task GetBookByIdAsync_GetBookById_ShouldReturnBookOfGivenID(BookServiceTestDataModel testData)
        {
            // Arrange
            _mockBookRepository.Setup(r => r.GetBookByIdAsync(1)).ReturnsAsync(testData.BookEntity);
            _mockMapper.Setup(m => m.Map<BookDto>(testData.BookEntity)).Returns(testData.BookDto);

            // Act
            var book = await _bookService.GetBookByIdAsync(1);

            // Assert
            Assert.NotNull(book);
            Assert.Equal(1, book.Id);

            _mockBookRepository.Verify(r => r.GetBookByIdAsync(1), Times.Once);
            _mockMapper.Verify(m => m.Map<BookDto>(testData.BookEntity), Times.Once);
        }

        [Theory]
        [ClassData(typeof(BookServiceTestData))]
        public async Task CreateBookAsync_CreateBook_ShouldCreateBookAndReturnBookDto(BookServiceTestDataModel testData)
        {
            // Arrange
            _mockMapper.Setup(m => m.Map<Book>(testData.BookCreationDto)).Returns(testData.BookEntity);
            _mockBookRepository.Setup(r => r.CreateBookAsync(testData.BookEntity)).Returns(Task.CompletedTask);
            _mockBookRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
            _mockMapper.Setup(m => m.Map<BookDto>(testData.BookEntity)).Returns(testData.BookDto);

            // Act
            var result = await _bookService.CreateBookAsync(testData.BookCreationDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testData.BookDto.Id, result.Id);
            Assert.Equal(testData.BookDto.Title, result.Title);
            Assert.Equal(testData.BookDto.Author, result.Author);
            Assert.Equal(testData.BookDto.Genre, result.Genre);
            Assert.Equal(testData.BookDto.ISBN, result.ISBN);
            Assert.True(result.IsAvailable);

            _mockMapper.Verify(m => m.Map<Book>(testData.BookCreationDto), Times.Once);
            _mockBookRepository.Verify(r => r.CreateBookAsync(testData.BookEntity), Times.Once);
            _mockBookRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockMapper.Verify(m => m.Map<BookDto>(testData.BookEntity), Times.Once);
        }

        [Theory]
        [ClassData(typeof(BookServiceTestData))]
        public async Task UpdateBookAsync_UpdateGivenBook_ShouldReturnUpdatedBook_WhenBookExists(BookServiceTestDataModel testData)
        {
            // Arrange
            _mockBookRepository.Setup(r => r.GetBookByIdAsync(testData.BookUpdateDto.Id)).ReturnsAsync(testData.BookEntity);
            _mockBookRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
            _mockMapper.Setup(mapper => mapper.Map<BookDto>(testData.BookEntity)).Returns(testData.UpdatedBookDto);

            // Act
            var result = await _bookService.UpdateBookAsync(testData.BookUpdateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testData.UpdatedBookDto.Title, result.Title);
            Assert.Equal(testData.UpdatedBookDto.Author, result.Author);
            Assert.Equal(testData.UpdatedBookDto.Genre, result.Genre);
            Assert.Equal(testData.UpdatedBookDto.ISBN, result.ISBN);
            Assert.Equal(testData.UpdatedBookDto.Description, result.Description);
            Assert.True(result.IsAvailable);

            _mockBookRepository.Verify(r => r.GetBookByIdAsync(testData.BookUpdateDto.Id), Times.Once);
            _mockBookRepository.Verify(r => r.UpdateBook(testData.BookEntity), Times.Once);
            _mockBookRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Theory]
        [ClassData(typeof(BookServiceTestData))]
        public async Task UpdateBookAsync_UpdateGivenBook_ShouldReturnNull_WhenBookDoesNotExist(BookServiceTestDataModel testData)
        {
            // Arrange

            _mockBookRepository.Setup(r => r.GetBookByIdAsync(testData.BookUpdateDto.Id)).ReturnsAsync((Book)null);

            // Act
            var result = await _bookService.UpdateBookAsync(testData.BookUpdateDto);

            // Assert
            Assert.Null(result);

            _mockBookRepository.Verify(r => r.GetBookByIdAsync(testData.BookUpdateDto.Id), Times.Once);
            _mockBookRepository.Verify(r => r.UpdateBook(It.IsAny<Book>()), Times.Never);
            _mockBookRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Theory]
        [ClassData(typeof(BookServiceTestData))]
        public async Task DeleteBookAsync_DeleteGivenBook_ShouldReturnTrue_WhenBookExists(BookServiceTestDataModel testData)
        {
            // Arrange
            _mockBookRepository.Setup(r => r.GetBookByIdAsync(testData.BookEntity.Id)).ReturnsAsync(testData.BookEntity);
            _mockBookRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            var result = await _bookService.DeleteBookAsync(testData.BookEntity.Id);

            // Assert
            Assert.True(result);

            _mockBookRepository.Verify(r => r.GetBookByIdAsync(testData.BookEntity.Id), Times.Once);
            _mockBookRepository.Verify(r => r.DeleteBook(testData.BookEntity), Times.Once);
            _mockBookRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteBookAsync_DeleteGivenBook_ShouldReturnFalse_WhenBookDoesNotExist()
        {
            // Arrange
            _mockBookRepository.Setup(r => r.GetBookByIdAsync(It.IsAny<int>())).ReturnsAsync((Book)null);

            // Act
            var result = await _bookService.DeleteBookAsync(1);

            // Assert
            Assert.False(result);

            _mockBookRepository.Verify(r => r.GetBookByIdAsync(1), Times.Once);
            _mockBookRepository.Verify(r => r.DeleteBook(It.IsAny<Book>()), Times.Never);
            _mockBookRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
}
