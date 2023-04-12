using Microsoft.AspNetCore.Mvc;
using PMTA.Infrastructure.Mediator;
using PMTA.Infrastructure.Repository;
using PMTA.Domain.Command;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PMTA.WebAPI.Controller
{
    [ApiController]
    [Route("projectmgmt/api/v1/manager/")]
    public class UserController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;
        public UserController(ICommandDispatcher commandDispatcher, IUserRepository userRepository, IConfiguration config)
        {
            _commandDispatcher = commandDispatcher;
            _userRepository = userRepository;
            _config = config;
        }

        [HttpPost]
        [Route("register-member")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RegisterMember([FromBody] CreateUserCommand userToRegister)
        {
            userToRegister.Username = userToRegister.Username.ToLower();
            if (await _userRepository.UserExist(userToRegister.Username)) return BadRequest("User already exists!");

            await _commandDispatcher.SendAsync(userToRegister);
            return CreatedAtAction(nameof(RegisterMember), "Member is successfully registered.");
        }

        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Login(UserLoginCommand userToLogin)
        {
            if (userToLogin != null && !string.IsNullOrEmpty(userToLogin.Username))
            {
                var userFromRepo = await _userRepository.Login(userToLogin.Username.ToLower(), userToLogin.Password);

                if (userFromRepo == null)
                    return Unauthorized();

                var claims = new[]{
                        new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                        new Claim(ClaimTypes.Name, userFromRepo.Username)
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
            else
                return Unauthorized();
        }
    }
}
