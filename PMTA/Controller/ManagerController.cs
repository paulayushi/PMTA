using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMTA.Domain.Command;
using PMTA.Domain.DTO;
using PMTA.Domain.Entity;
using PMTA.Domain.Query;
using PMTA.Infrastructure.Mediator.Query;
using PMTA.Infrastructure.Mediator;
using PMTA.Infrastructure.Repository;

namespace PMTA.WebAPI.Controller
{
    [ApiController]
    [Route("projectmgmt/api/v1/manager/")]
    [Authorize(Policy = "OnlyManager")]
    public class ManagerController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher<MemberEntity> _quoteDispatcher;
        private readonly IMemberRepository _repository;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public ManagerController(ICommandDispatcher commandDispatcher,
            IQueryDispatcher<MemberEntity> quoteDispatcher, IMemberRepository repository, IConfiguration config, IMapper mapper)
        {
            _commandDispatcher = commandDispatcher;
            _quoteDispatcher = quoteDispatcher;
            _repository = repository;
            _config = config;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("add-member")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> AddMember([FromBody] CreateMemberCommand createMember)
        {
            var memberFromDB = await _repository.GetByIdAsync(createMember.MemberId);

            if (memberFromDB != null)
            {
                return BadRequest($"Member with user id = {createMember.MemberId} already exist in the system.");
            }

            await _commandDispatcher.SendAsync(createMember);
            var memberCreated = _mapper.Map<MemberCreatedDto>(createMember);
            return CreatedAtAction(nameof(AddMember), memberCreated );
        }

        [HttpGet]
        [Route("list/memberDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> FetchAllMemberDetails()
        {

            var members = await _quoteDispatcher.SendAsync(new GetAllMemberQuery());

            if (members == null || !members.Any())
            {
                return NoContent();
            }

            var sortMembersInDesc = members.OrderByDescending(m => m.Experience);
            var memberQueryResponses = _mapper.Map<List<MemberQueryResponse>>(sortMembersInDesc);
            return Ok(memberQueryResponses);
        }

        [HttpPatch]
        [Route("update/allocationPercentage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status304NotModified)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateAllocationPercentage(UpdateMemberCommand updateMember)
        {
            var memberFromDB = await _repository.GetByIdAsync(updateMember.MemberId);

            if (memberFromDB == null)
            {
                return BadRequest($"Member with Id = {updateMember.MemberId} does not exist in the system. Please provide correct member id.");
            }

            updateMember.Id = memberFromDB.EventId;
            updateMember.ProjectEndDate = memberFromDB.ProjectEndDate;
            await _commandDispatcher.SendAsync(updateMember);
            return Ok(updateMember);
        }

        [HttpPost]
        [Route("assign-task")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
    }
}
