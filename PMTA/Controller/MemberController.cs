using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PMTA.Domain.Command;
using PMTA.Domain.DTO;
using PMTA.Domain.Entity;
using PMTA.Domain.Query;
using PMTA.Infrastructure.Mediator;
using PMTA.Infrastructure.Mediator.Query;
using PMTA.Infrastructure.Repository;

namespace PMTA.WebAPI.Controller
{
    [ApiController]
    [Route("projectmgmt/api/v1/manager/")]
    [Authorize]
    public class MemberController : ControllerBase
    {
        private readonly ILogger<MemberController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher<MemberEntity> _quoteDispatcher;
        private readonly IMemberRepository _repository;

        public MemberController(ILogger<MemberController> logger, ICommandDispatcher commandDispatcher, 
            IQueryDispatcher<MemberEntity> quoteDispatcher, IMemberRepository repository)
        {
            _logger = logger;
            _commandDispatcher = commandDispatcher;
            _quoteDispatcher = quoteDispatcher;
            _repository = repository;
        }

        [HttpPost]
        [Route("add-member")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddMember([FromBody] CreateMemberCommand createMember)
        {
            var memberFromDB = await _repository.GetByIdAsync(createMember.MemberId);

            if (memberFromDB != null)
            {
                return BadRequest($"Member with Id = {createMember.MemberId} already exist in the system.");
            }

            await _commandDispatcher.SendAsync(createMember);
            return CreatedAtAction(nameof(AddMember), createMember);
        }

        [HttpGet]
        [Route("list/memberDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> FetchAllMemberDetails()
        {

            var members = await _quoteDispatcher.SendAsync(new GetAllMemberQuery());

            if(members == null || !members.Any())
            {
                return NoContent();
            }

            var sortMembersInDesc = members.OrderByDescending(m => m.Experience);
            var memberQueryResponse = new MemberQueryResponse()
            {
                Members = sortMembersInDesc.ToList()
            };
            return Ok(memberQueryResponse);
        }        

        [HttpPatch]
        [Route("update/allocationPercentage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status304NotModified)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
            return Ok();
        }
    }
}
