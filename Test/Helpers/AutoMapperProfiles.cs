using AutoMapper;
using Test.Dtos;
using Test.Model;

namespace Test.Helpers
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>();
            CreateMap<User, UserForDetailedDto>();
        }
    }
}