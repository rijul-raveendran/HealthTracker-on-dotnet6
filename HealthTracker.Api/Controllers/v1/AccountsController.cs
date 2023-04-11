using HealthTracker.Authentication.Configuration;
using HealthTracker.Authentication.Models.DTO.Incoming;
using HealthTracker.Authentication.Models.DTO.Outgoing;
using HealthTracker.DataService.IConfiguration;
using HealthTracker.Entities.Dbset;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;

namespace HealthTracker.Api.Controllers.v1
{
    public class AccountsController : BaseController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;
        public AccountsController(IUnitOfWork unitOfWork, 
            UserManager<IdentityUser> userManager,IOptionsMonitor<JwtConfig> optionsMonitor) : base(unitOfWork)
        {
            _userManager = userManager;
            _jwtConfig = optionsMonitor.CurrentValue;
        }

        // Register Action
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto registrationDto)
        {
            // Check if object or model we are receivinbg is valid  
            if (ModelState.IsValid)
            {
                // check if already exists 
                var IsUserAvailable = await _userManager.FindByEmailAsync
                    (registrationDto.Email);
                if (IsUserAvailable != null)
                {
                    return BadRequest(new UserRegistrationResponseDto
                    {
                        IsSuccess = false,
                        Errors = new List<string>() { "Email already in use" }
                    });
                }
                // Add the user
                var newUser = new IdentityUser()
                {
                    Email = registrationDto.Email,
                    UserName = registrationDto.Email,
                    EmailConfirmed = true//todo build email address confirm functionality
                };

                // Add the user to the table
                var isCreated = await _userManager.CreateAsync(newUser, registrationDto.Password);

                if (isCreated.Succeeded)
                {
                    return BadRequest(new UserRegistrationResponseDto
                    {
                        IsSuccess = isCreated.Succeeded,
                        Errors = isCreated.Errors.Select(x => x.Description).ToList()
                    }); 
                }

                var _user = new User();
                _user.IdentityId = new Guid(newUser.Id);
                _user.FirstName = registrationDto.FirstName;
                _user.LastName = registrationDto.LastName;
                _user.Email = registrationDto.Email;
                _user.DateOfBirth = DateTime.Now.ToString();
                _user.Country = string.Empty;
                _user.Phone = string.Empty;
                _user.Status = 1;

                await _unitOfWork.Users.Add(_user);
                await _unitOfWork.CompleteAsync();

                // Create Jwt Token
                var jwtToken = GenerateJwtToken(newUser);

                //return back to the user
                return Ok(new UserRegistrationResponseDto
                {
                    IsSuccess = true,
                    Token = jwtToken

                });



            }
            else
            {
                return BadRequest(new UserRegistrationResponseDto
                {
                    IsSuccess = false,
                    Errors = new List<string>() { "Invalid payload" }
                });
            }
            }

        private string GenerateJwtToken(IdentityUser user)
        {
            // handler for creating token
            var jwthandler = new JwtSecurityTokenHandler();

            // Get the security key
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email), //Sub is unique id
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) //used by the refresh token
                }),

                Expires = DateTime.UtcNow.AddHours(3),

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature
                )
            };

            // generate security object token
            var token = jwthandler.CreateToken(tokenDescriptor);

            // convert security object token to string
            var jwtToken = jwthandler.WriteToken(token);

            return jwtToken;

        }
    }

}

