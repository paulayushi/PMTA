using PMTA.Core.Aggregate;
using PMTA.Domain.Command;
using PMTA.Domain.Event;

namespace PMTA.Domain.Aggregate
{
    public class MemberAggregate : AggregateRoot
    {
        private string _name;
        private readonly Dictionary<Guid, Tuple<int, string>> _tasks = new();

        public MemberAggregate()
        {
        }

        public MemberAggregate(CreateMemberCommand createMemberCommand)
        {
            RaiseEvent(new MemberCreatedEvent
            {
                Id = Guid.NewGuid(),
                MemberId= createMemberCommand.MemberId,
                Name = createMemberCommand.Name,
                Experience = createMemberCommand.Experience,
                Skillset = createMemberCommand.Skillset,
                ProjectStartDate = createMemberCommand.ProjectStartDate,
                ProjectEndDate = createMemberCommand.ProjectEndDate,
                AllocationPercentage = createMemberCommand.AllocationPercentage,
                DateCreated = DateTime.Now,
                Description = createMemberCommand.Description
            });
        }

        public void Apply(MemberCreatedEvent @event)
        {
            _id = @event.Id;
            _name = @event.Name;
        }

        public void UpdateAllocationPercentage(UpdateMemberCommand updateMemberCommand)
        {
            RaiseEvent(new MemberUpdatedEvent()
            {
                Id = updateMemberCommand.Id,
                MemberId = updateMemberCommand.MemberId,
                AllocationPercentage= updateMemberCommand.ProjectEndDate > DateTime.Now ? 100 : 0,
            });
        }
        public void Apply(MemberUpdatedEvent @event)
        {
            _id = @event.Id;
        }

        public void AddTask(CreateTaskCommand createTaskCommand)
        {
            RaiseEvent(new TaskCreatedEvent()
            {
                Id = Guid.NewGuid(),
                MemberId = createTaskCommand.MemberId,
                MemberName = createTaskCommand.MemberName,
                TaskName = createTaskCommand.TaskName,
                Delivarables = createTaskCommand.Delivarables,
                TaskStartDate = createTaskCommand.TaskStartDate,
                TaskEndDate = createTaskCommand.TaskEndDate,
                DateCreated = DateTime.Now
            });
        }
        public void Apply(TaskCreatedEvent @event)
        {
            _id = @event.Id;
            _tasks.Add(@event.Id, new Tuple<int, string>(@event.MemberId, @event.TaskName));
        }
    }
}
