using AutoMapper;
using ReadNest.Contracts.BookLoans.Requests;
using ReadNest.Contracts.BookLoans.Responses;
using ReadNest.Contracts.Books.Requests;
using ReadNest.Contracts.Books.Responses;
using ReadNest.Domain.BookLoans.Entities;
using ReadNest.Domain.Books.Entities;

namespace ReadNest.Services.BookLoans
{
    public class BookLoanMappingProfile : Profile
    {
        public BookLoanMappingProfile()
        {
            CreateMap<BookLoan, BookLoanDto>();
            CreateMap<BookLoanCreationDto, BookLoan>();
            CreateMap<BookLoan, BookLoanWithBookDetailsDto>();
            CreateMap<BookLoanWithBookDetailsDto, BookLoan>();
        }
    }
}
