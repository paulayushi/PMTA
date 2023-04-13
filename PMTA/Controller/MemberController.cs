using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PMTA.Domain.Command;
using PMTA.Domain.DTO;
using PMTA.Domain.Entity;
using PMTA.Domain.Query;
using PMTA.Infrastructure.Mediator;
using PMTA.Infrastructure.Mediator.Query;
using PMTA.Infrastructure.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PMTA.WebAPI.Controller
{
    [ApiController]
    [Route("projectmgmt/api/v1/manager/")]
    [Authorize(Policy = "OnlyManager")]
    public class MemberController : ControllerBase
    {
        private readonly ILogger<MemberController> _logger;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher<MemberEntity> _quoteDispatcher;
        private readonly IMemberRepository _repository;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public MemberController(ILogger<MemberController> logger, ICommandDispatcher commandDispatcher, 
            IQueryDispatcher<MemberEntity> quoteDispatcher, IMemberRepository repository, IConfiguration config, IMapper mapper)
        {
            _logger = logger;
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
            return CreatedAtAction(nameof(AddMember), createMember);
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

            if(members == null || !members.Any())
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
            return Ok($"Allocation percentage for member = {updateMember.MemberId} is successfully updated.");
        }

        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginCommand userToLogin)
        {
            var userFromRepo = await _repository.Login(userToLogin.UserId, userToLogin.Password);

            if (userFromRepo == null)
                return Unauthorized();

            var claims = new[]{
                        new Claim(ClaimTypes.NameIdentifier, userFromRepo.MemberId.ToString()),
                        new Claim(ClaimTypes.Name, userFromRepo.Name),
                        new Claim( type: "IsManager", value: userFromRepo.IsManager.ToString(), valueType: ClaimValueTypes.Boolean)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("JwtToken:SecretKey").Value));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = cred
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                userFromRepo.IsManager
            });
        }
    }
}
