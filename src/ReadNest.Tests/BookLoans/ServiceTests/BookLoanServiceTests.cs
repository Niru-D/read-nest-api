using AutoMapper;
using Moq;
using ReadNest.Contracts.BookLoans.Responses;
using ReadNest.Domain.BookLoans.Entities;
using ReadNest.Domain.BookLoans.Interfaces;
using ReadNest.Domain.Books.Interfaces;
using ReadNest.Services.BookLoans;
using ReadNest.Tests.BookLoans.ServiceTestData;

namespace ReadNest.Tests.BookLoans.ServiceTests
{
    public class BookLoanServiceTests
    {
        private readonly Mock<IBookLoanRepository> _mockBookLoanRepository;
        private readonly Mock<IBookRepository> _mockBookRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly BookLoanService _bookLoanService;

        public BookLoanServiceTests()
        {
            _mockBookLoanRepository = new Mock<IBookLoanRepository>();
            _mockBookRepository = new Mock<IBookRepository>();
            _mockMapper = new Mock<IMapper>();
            _bookLoanService = new BookLoanService(_mockBookLoanRepository.Object, _mockBookRepository.Object, _mockMapper.Object);
        }

        [Theory]
        [ClassData(typeof(BookLoanServiceTestData))]
        public async Task GetBookLoansAsync_GetAllBookLoans_ShouldReturnBookLoansWithPagination(BookLoanServiceTestDataModel testData)
        {
            // Arrange
            var bookLoans = new List<BookLoan> { testData.BookLoanEntity };
            var totalItemCount = 1;
            var bookLoanDtos = new List<BookLoanWithBookDetailsDto> { testData.BookLoanWithBookDetailsDto };

            _mockBookLoanRepository.Setup(repo => repo.GetBookLoansAsync(null, null, null, null, null, 1, 10))
                                    .ReturnsAsync((bookLoans, totalItemCount));

            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<BookLoanWithBookDetailsDto>>(bookLoans))
                       .Returns(bookLoanDtos);

            // Act
            var result = await _bookLoanService.GetBookLoansAsync(null, null, null, null, null, 1, 10);

            // Assert
            Assert.Equal(totalItemCount, result.Item2.TotalItemCount);
            Assert.Equal(bookLoanDtos, result.Item1);

            _mockBookLoanRepository.Verify(r => r.GetBookLoansAsync(null, null, null, null, null, 1, 10), Times.Once);
            _mockMapper.Verify(m => m.Map<IEnumerable<BookLoanWithBookDetailsDto>>(bookLoans), Times.Once);
        }

        [Theory]
        [ClassData(typeof(BookLoanServiceTestData))]
        public async Task CreateBookLoanAsync_BorrowBook_ShouldCreateAndReturnBookLoan(BookLoanServiceTestDataModel testData)
        {
            // Arrange
            _mockMapper.Setup(mapper => mapper.Map<BookLoan>(testData.BookLoanCreationDto)).Returns(testData.BookLoanEntity);

            _mockBookLoanRepository.Setup(repo => repo.CreateBookLoanAsync(testData.BookLoanEntity)).Returns(Task.CompletedTask);
            _mockBookLoanRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

            _mockBookRepository.Setup(repo => repo.UpdateAvailability(testData.BookLoanCreationDto.BookId, false));
            _mockBookRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

            _mockMapper.Setup(mapper => mapper.Map<BookLoanDto>(testData.BookLoanEntity)).Returns(testData.BookLoanDto);

            // Act
            var result = await _bookLoanService.CreateBookLoanAsync(testData.BookLoanCreationDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testData.BookLoanDto.Id, result.Id);
            _mockBookLoanRepository.Verify(repo => repo.CreateBookLoanAsync(testData.BookLoanEntity), Times.Once);
            _mockBookLoanRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            _mockBookRepository.Verify(repo => repo.UpdateAvailability(testData.BookLoanCreationDto.BookId, false), Times.Once);
            _mockBookRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Theory]
        [ClassData(typeof(BookLoanServiceTestData))]
        public async Task ReturnBookAsync_ReturnBook_ShouldUpdateAvailabilityAndSaveChanges(BookLoanServiceTestDataModel testData)
        {
            // Arrange
            _mockBookLoanRepository.Setup(repo => repo.ReturnBookAsync(1, testData.BookReturnDto));
            _mockBookLoanRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

            _mockBookRepository.Setup(repo => repo.UpdateAvailability(testData.BookReturnDto.BookId, true));
            _mockBookRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            await _bookLoanService.ReturnBookAsync(1, testData.BookReturnDto);

            // Assert
            _mockBookLoanRepository.Verify(repo => repo.ReturnBookAsync(1, testData.BookReturnDto), Times.Once);
            _mockBookLoanRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
            _mockBookRepository.Verify(repo => repo.UpdateAvailability(testData.BookReturnDto.BookId, true), Times.Once);
            _mockBookRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }
    }
}
