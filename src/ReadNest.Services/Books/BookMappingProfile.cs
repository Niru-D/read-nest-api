using AutoMapper;
using ReadNest.Contracts.Books.Requests;
using ReadNest.Contracts.Books.Responses;
using ReadNest.Domain.Books.Entities;

namespace ReadNest.Services.Books
{
    public class BookMappingProfile : Profile
    {
        public BookMappingProfile()
        {
            CreateMap<Book, BookDto>();
            CreateMap<BookCreationDto, Book>();
            CreateMap<BookUpdateDto, Book>();
            CreateMap<BookDto, Book>();
        }
    }
}
