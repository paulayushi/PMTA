using AutoMapper;
using PMTA.Domain.Command;
using PMTA.Domain.DTO;
using PMTA.Domain.Entity;

namespace PMTA.Infrastructure.Helper
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<MemberEntity, MemberQueryResponse>();
            CreateMap<TaskEntity, TaskDto>();
            CreateMap<TaskEntity, TaskQueryResponse>()
                .ForMember( dest => dest.ProjectStartDate, opt => opt.MapFrom(src => src.Member.ProjectStartDate))
                .ForMember(dest => dest.ProjectEndDate, opt => opt.MapFrom(src => src.Member.ProjectEndDate))
                .ForMember(dest => dest.AllocationPercentage, opt => opt.MapFrom(src => src.Member.AllocationPercentage));
            CreateMap<CreateMemberCommand, MemberCreatedDto>();
        }
    }
}
