using AutoMapper;
using PMTA.Domain.Command;
using PMTA.Domain.Entity;

namespace PMTA.Infrastructure.Helper
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserEntity, CreateUserCommand>().ReverseMap();
        }
    }
}
