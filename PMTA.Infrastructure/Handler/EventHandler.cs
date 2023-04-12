using PMTA.Domain.Entity;
using PMTA.Domain.Event;
using PMTA.Infrastructure.Repository;

namespace PMTA.Infrastructure.Handler
{
    public class EventHandler : IEventHandler
    {
        private readonly IMemberRepository _memberRepository;
        private readonly ITaskRepository _taskRepository;
        public EventHandler(IMemberRepository memberRepository, ITaskRepository taskRepository)
        {
            _memberRepository = memberRepository;
            _taskRepository = taskRepository;
        }
        public async Task On(MemberCreatedEvent memberCreatedEvent)
        {
            if (memberCreatedEvent is null) return;

            var memberEntity = new MemberEntity()
            {
                EventId = memberCreatedEvent.Id,
                MemberId = memberCreatedEvent.MemberId,
                Name = memberCreatedEvent.Name,
                AllocationPercentage = memberCreatedEvent.AllocationPercentage,
                Description = memberCreatedEvent.Description,
                Experience = memberCreatedEvent.Experience,
                ProjectEndDate = memberCreatedEvent.ProjectEndDate,
                ProjectStartDate = memberCreatedEvent.ProjectStartDate,
                Skillset = string.Join(',', memberCreatedEvent.Skillset),
                PasswordSalt= memberCreatedEvent.PasswordSalt,
                PasswordHash= memberCreatedEvent.PasswordHash,
                IsManager = memberCreatedEvent.IsManager
            };

            await _memberRepository.AddAsync(memberEntity);
        }

        public async Task On(TaskCreatedEvent taskCreatedEvent)
        {
            if (taskCreatedEvent is null) return;

            var taskEntity = new TaskEntity()
            {
                TaskId = taskCreatedEvent.Id,
                MemberId = taskCreatedEvent.MemberId,
                MemberName= taskCreatedEvent.MemberName,
                Delivarables = taskCreatedEvent.Delivarables,
                TaskName = taskCreatedEvent.TaskName,
                TaskStartDate = taskCreatedEvent.TaskStartDate,
                TaskEndDate = taskCreatedEvent.TaskEndDate
            };

            await _taskRepository.AddAsync(taskEntity);
        }

        public async Task On(MemberUpdatedEvent memberUpdatedEvent)
        {
            if (memberUpdatedEvent is null) return;

            var memberEntity = new MemberEntity()
            {
                MemberId = memberUpdatedEvent.MemberId,
                AllocationPercentage = memberUpdatedEvent.AllocationPercentage,
            };

            await _memberRepository.UpdateAsync(memberEntity);
        }
    }
}
