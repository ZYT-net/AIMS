using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AIMS.Server.Models;
using AIMS.Server.Services;
using Microsoft.AspNetCore.Authorization;
using System.Text;

namespace AIMS.Server.Controllers
{
    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RealName { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AMDBContext _context;
        private readonly JwtService _jwtService;

        public UsersController(AMDBContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "用户名和密码不能为空" });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);
            if (user == null || user.Password != request.Password)
            {
                return Unauthorized(new { message = "用户名或密码错误" });
            }

            // Fix: Pass the `user` object instead of `user.UserName` to the `GenerateToken` method  
            var token = _jwtService.GenerateToken(user, user.Role);
            return Ok(new
            {
                token,
                user = new
                {
                    user.Id,
                    user.UserName,
                    user.Role,
                    user.RealName,
                    user.Phone
                }
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<Users>> PostUsers([FromBody] RegisterRequest request)
        {
            var newUser = new Users
            {
                UserName = request.UserName,
                Password = request.Password,
                RealName = request.RealName,
                Phone = request.Phone,
                Role = request.Role
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsers", new { id = newUser.Id }, newUser);
        }

        // 需要认证的接口加 [Authorize]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Users>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Users>> GetUsers(int id)
        {
            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }
            return users;
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutUsers(int id, Users users)
        {
            if (id != users.Id)
            {
                return BadRequest();
            }

            _context.Entry(users).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsersExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUsers(int id)
        {
            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }

            _context.Users.Remove(users);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("export")]
        [Authorize]
        public async Task<IActionResult> ExportUsers()
        {
            var users = await _context.Users.ToListAsync();
            var csv = new StringBuilder();
            csv.AppendLine("Id,UserName,Role,RealName,Phone");
            foreach (var user in users)
            {
                csv.AppendLine($"{user.Id},{user.UserName},{user.Role},{user.RealName},{user.Phone}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", "users.csv");
        }

        private bool UsersExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}