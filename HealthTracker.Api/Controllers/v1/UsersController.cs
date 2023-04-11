using HealthTracker.DataService.DataService;
using HealthTracker.Entities.Dbset;
using Microsoft.AspNetCore.Mvc;
using HealthTracker.Entities.Dtos.Incoming;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using HealthTracker.DataService.IConfiguration;

namespace HealthTracker.Api.Controllers.v1
{

    public class UsersController : BaseController
    {
        public UsersController(IUnitOfWork unitOfwork) : base(unitOfwork)
        {
        }


        // Get
        [HttpGet]
        [HttpHead]
        [Route("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _unitOfWork.Users.All();
            return Ok(users);
        }

        // Post
        [HttpPost]
        public async Task<IActionResult> AddUser(UserDto user)
        {
            var _user = new User();
            _user.FirstName = user.FirstName;
            _user.LastName = user.LastName;
            _user.Email = user.Email;
            _user.DateOfBirth = user.DateOfBirth;
            _user.Country = user.Country;
            _user.Phone = user.Phone;
            _user.Status = 1;

            await _unitOfWork.Users.Add(_user);
            await _unitOfWork.CompleteAsync();

            return CreatedAtRoute("GetUser", new { id = _user.Id }, user);


        }

        // Get
        [HttpGet]
        [Route("GetUser", Name = "GetUser")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _unitOfWork.Users.GetById(id);
            return Ok(user);
        }


    }
}
