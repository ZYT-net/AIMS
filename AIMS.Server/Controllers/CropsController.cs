using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using AIMS.Server.Models;

namespace AIMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CropsController : ControllerBase
    {
        private readonly AMDBContext _context;

        public CropsController(AMDBContext context)
        {
            _context = context;
        }

        // 获取当前用户 id
        private int GetCurrentUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                return userId;

            var userName = User.Identity?.Name;
            if (userName == null)
                return 0;

            var user = _context.Users.FirstOrDefault(u => u.UserName == userName);
            return user?.Id ?? 0;
        }

        // GET: api/Crops
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Crops>>> GetCrops()
        {
            int userId = GetCurrentUserId();
            return await _context.Crops.Where(c => c.UserId == userId).ToListAsync();
        }

        // POST: api/Crops
        [HttpPost]
        public async Task<ActionResult<Crops>> PostCrops([FromBody] Crops crop)
        {
            int userId = GetCurrentUserId();
            var entity = new Crops
            {
                Name = crop.Name,
                UserId = userId
            };
            try
            {
                _context.Crops.Add(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"添加作物时发生错误: {ex.Message}");
            }

            return CreatedAtAction(nameof(GetCrops), new { id = entity.Id }, entity);
        }

        // PUT: api/Crops/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCrops(int id, [FromBody] Crops crop)
        {
            if (id != crop.Id)
                return BadRequest();

            int userId = GetCurrentUserId();
            var dbCrop = await _context.Crops.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
            if (dbCrop == null)
                return NotFound();

            dbCrop.Name = crop.Name; // 只允许修改名称

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"更新作物时发生错误: {ex.Message}");
            }

            return NoContent();
        }

        // DELETE: api/Crops/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCrops(int id)
        {
            int userId = GetCurrentUserId();
            var crop = await _context.Crops.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
            if (crop == null)
                return NotFound();

            try
            {
                _context.Crops.Remove(crop);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"删除作物时发生错误: {ex.Message}");
            }

            return NoContent();
        }
    }
}