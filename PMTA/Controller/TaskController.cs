using Microsoft.AspNetCore.Mvc;
using PMTA.Domain.Entity;
using PMTA.Infrastructure.Mediator.Query;
using PMTA.Infrastructure.Mediator;
using PMTA.Infrastructure.Repository;
using PMTA.Domain.Command;
using PMTA.Domain.Query;
using PMTA.Domain.DTO;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;

namespace PMTA.WebAPI.Controller
{
    [ApiController]
    [Route("projectmgmt/api/v1/manager/")]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ILogger<TaskController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher<TaskEntity> _quoteDispatcher;
        private readonly IMemberRepository _repository;
        private readonly IMapper _mapper;
        public TaskController(ILogger<TaskController> logger, ICommandDispatcher commandDispatcher,
            IQueryDispatcher<TaskEntity> quoteDispatcher, IMemberRepository repository, IMapper mapper)
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
            _quoteDispatcher = quoteDispatcher;
            _repository = repository;
            _mapper = mapper;
        }
        [HttpPost]
        [Route("assign-task")]
        public async Task<IActionResult> AssignTask([FromBody] CreateTaskCommand createTask)
        {
            var memberFromDB = await _repository.GetByIdAsync(createTask.MemberId);

            if (memberFromDB == null)
            {
                return BadRequest($"Member with Id = {createTask.MemberId} does not exist in the system. Please provide correct member id.");
            }

            if (createTask.TaskEndDate > memberFromDB.ProjectEndDate)
            {
                return BadRequest($"{nameof(memberFromDB.ProjectEndDate)} must be greater than {nameof(createTask.TaskEndDate)}.");
            }

            createTask.MemberName = memberFromDB.Name;
            await _commandDispatcher.SendAsync(createTask);
            return CreatedAtAction(nameof(AssignTask), createTask);
        }

        [HttpGet]
        [Route("list/{memberId}/taskDetails")]
        public async Task<IActionResult> FetchTaskDetailsByMemberId([FromRoute] int memberId)
        {
            var memberFromDB = await _repository.GetByIdAsync(memberId);

            if (memberFromDB == null)
            {
                return BadRequest($"Member with Id = {memberId} does not exist in the system. Please provide correct member id.");
            }
            var tasks = await _quoteDispatcher.SendAsync(new GetTasksByMemberIdQuery()
            {
                MemberId = memberId
            });

            if (tasks == null || !tasks.Any())
            {
                return NoContent();
            }

            var taskList = _mapper.Map<List<TaskQueryResponse>>(tasks);            

            return Ok(taskList);            
        }
    }
}
