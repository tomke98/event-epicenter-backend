using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using Service;

namespace WebAPI.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService userService;

        public UserController()
        {
           userService = new UserService();
        }

        [Route("users")]
        [HttpGet]
        public async Task<IActionResult> GetUsersAsync()
        {
            var users = await userService.GetAllUsersAsync();

            if(users != null && users.Any())
            {
                return Ok(users);
            }

            return BadRequest("No users found.");
        }

        [Route("users/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetUserByIdAsync(string id)
        {
            var user = await userService.GetUserByIdAsync(id);

            if (user != null)
            {
                return Ok(user);
            }

            return BadRequest("User not found.");
        }

        [Authorize(Roles = "user, admin")]
        [Route("users/{id}")]
        [HttpPut]
        public async Task<IActionResult> UpdateUserAsync(string id, UserREST user)
        {
            var existingUser = await userService.GetUserByIdAsync(id);

            if (existingUser == null)
            {
                return BadRequest("User not found.");
            }

            var result = await userService.UpdateUserAsync(new User(id, user.City, user.Dob, user.FirstName, user.LastName, user.EventTypeIds));

            if (result)
            {
                return Ok(user);
            }

            return BadRequest("User not updated.");
        }
    }

    public class UserREST
    {
        public string City { get; set; }

        public DateTime Dob { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public List<string> EventTypeIds { get; set; }
    }
}