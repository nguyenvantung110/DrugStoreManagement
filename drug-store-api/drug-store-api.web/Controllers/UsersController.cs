using drug_store_api.dtos.Users;
using drug_store_api.services.IF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace drug_store_api.web.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            this._userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var res = await _userService.GetUsersAsync();
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(Guid id)
        {
            var customer = await _userService.GetUsersByIdAsync(id);
            if (customer == null) return NotFound();
            return customer;
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateCustomer(UserDto userDto)
        {
            await _userService.CreateUser(userDto);
            return NoContent();
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateCustomer(UserUpdateDto userDto)
        {
            await _userService.UpdateUser(userDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            await _userService.DeleteUser(id);
            return NoContent();
        }
    }
}
