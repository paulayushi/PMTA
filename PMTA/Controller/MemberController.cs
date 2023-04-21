using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PMTA.Domain.Command;
using PMTA.Domain.DTO;
using PMTA.Domain.Entity;
using PMTA.Domain.Query;
using PMTA.Infrastructure.Mediator.Query;
using PMTA.Infrastructure.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PMTA.WebAPI.Controller
{
    [ApiController]
    [Route("projectmgmt/api/v1/member/")]
    [Authorize]
    public class MemberController : ControllerBase
    {
        private readonly IQueryDispatcher<TaskEntity> _queryDispatcher;
        private readonly IMemberRepository _repository;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public MemberController(IQueryDispatcher<TaskEntity> queryDispatcher, IMemberRepository repository, IConfiguration config, IMapper mapper)
        {
            _queryDispatcher = queryDispatcher;
            _repository = repository;
            _config = config;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("list/{memberId}/taskDetails")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> FetchTaskDetailsByMemberId([FromRoute] int memberId)
        {
            var memberFromDB = await _repository.GetByIdAsync(memberId);

            if (memberFromDB == null)
            {
                return BadRequest($"Member with Id = {memberId} does not exist in the system. Please provide correct member id.");
            }
            var tasks = await _queryDispatcher.SendAsync(new GetTasksByMemberIdQuery()
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
