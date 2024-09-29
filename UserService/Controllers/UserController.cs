using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedModels;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private static readonly List<User> Users = new List<User>
        {
            new User { Id = 1, Name = "John Doe", Email = "john@example.com" },
            new User { Id = 2, Name = "Jane Smith", Email = "jane@example.com" }
        };

        [HttpGet("{id}")]
        public ActionResult<User> GetUser(int id)
        {
            var user = Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        [HttpGet]
        [Authorize]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            return Ok(Users);
        }

        [HttpPost]
        public ActionResult<User> AddUser([FromBody] User newUser)
        {
            newUser.Id = Users.Max(u => u.Id) + 1;
            Users.Add(newUser);
            return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, newUser);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateUser(int id, [FromBody] User updatedUser)
        {
            var existingUser = Users.FirstOrDefault(u => u.Id == id);
            if (existingUser == null)
                return NotFound();

            // Оновлення полів
            existingUser.Name = updatedUser.Name;
            existingUser.Email = updatedUser.Email;

            return Ok(existingUser);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteOrder(int id)
        {
            var user = Users.FirstOrDefault(o => o.Id == id);
            if (user == null)
                return NotFound("User not found");

            Users.Remove(user);
            return Ok($"User with ID {id} deleted.");
        }

    }
}
