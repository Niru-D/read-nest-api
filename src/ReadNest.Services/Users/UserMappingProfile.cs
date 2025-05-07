using AutoMapper;
using ReadNest.Contracts.Users.Requests;
using ReadNest.Contracts.Users.Responses;
using ReadNest.Domain.Users.Entities;

namespace ReadNest.Services.Users
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserCreationDto, User>();
            CreateMap<UserUpdateDto, User>();
        }
    }
}
