using AutoMapper;
using PMTA.Core.Handler;
using PMTA.Domain.Aggregate;
using PMTA.Domain.Command;
using PMTA.Domain.Entity;
using PMTA.Infrastructure.Repository;

namespace PMTA.WebAPI.Command
{
    public class CommandHandler : ICommandHandler
    {
        private readonly IEventSourcingHandler<MemberAggregate> _eventSourcingHandler;
        private readonly IMapper _mapper;
        //private readonly IUserRepository _userRepository;

        public CommandHandler(IEventSourcingHandler<MemberAggregate> eventSourcingHandler, IMapper mapper)
        {
            _eventSourcingHandler = eventSourcingHandler;
            _mapper = mapper;
            //_userRepository = userRepository;
        }

        public async Task HandleAsync(CreateMemberCommand createMember)
        {
            var aggregate = new MemberAggregate(createMember);
            await _eventSourcingHandler.SaveAsync(aggregate);
        }

        public async Task HandleAsync(UpdateMemberCommand updateMember)
        {
            var aggregate = await _eventSourcingHandler.GetByIdAsync(updateMember.Id);
            aggregate.UpdateAllocationPercentage(updateMember);
            await _eventSourcingHandler.SaveAsync(aggregate);
        }

        public async Task HandleAsync(CreateTaskCommand createTask)
        {
            var aggregate = await _eventSourcingHandler.GetByIdAsync(createTask.Id);
            aggregate.AddTask(createTask);
            await _eventSourcingHandler.SaveAsync(aggregate);
        }

        //public async Task HandleAsync(CreateUserCommand createUser)
        //{            
        //    var user = _mapper.Map<UserEntity>(createUser);
        //    await _userRepository.RegisterAsync(user, createUser.Password);
        //}
    }
}
