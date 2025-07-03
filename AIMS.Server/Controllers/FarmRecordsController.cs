using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using AIMS.Server.Models;
using System.Text;

namespace AIMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FarmRecordsController : ControllerBase
    {
        private readonly AMDBContext _context;

        public FarmRecordsController(AMDBContext context)
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

        // GET: api/FarmRecords
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FarmRecords>>> GetFarmRecords()
        {
            int userId = GetCurrentUserId();
            var cropIds = _context.Crops.Where(c => c.UserId == userId).Select(c => c.Id);
            return await _context.FarmRecords.Where(r => cropIds.Contains(r.CropId ?? 0)).ToListAsync();
        }

        // POST: api/FarmRecords
        [HttpPost]
        public async Task<ActionResult<FarmRecords>> PostFarmRecords([FromBody] FarmRecords farmRecords)
        {
            int userId = GetCurrentUserId();
            //var crop = await _context.Crops.FirstOrDefaultAsync(c => c.Id == farmRecords.CropId && c.UserId == userId);
            //if (crop == null)
            //    return Forbid();

            var entity = new FarmRecords
            {
                CropId = farmRecords.CropId,
                Action = farmRecords.Action,
                RecordDate = farmRecords.RecordDate,
                Description = farmRecords.Description
            };

            try
            {
                _context.FarmRecords.Add(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"添加农事记录时发生错误: {ex.Message}");
            }

            return CreatedAtAction(nameof(GetFarmRecords), new { id = entity.Id }, entity);
        }

        // PUT: api/FarmRecords/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFarmRecords(int id, [FromBody] FarmRecords farmRecords)
        {
            if (id != farmRecords.Id)
                return BadRequest();

            int userId = GetCurrentUserId();
            var dbRecord = await _context.FarmRecords
                .Include(r => r.Crop)
                .FirstOrDefaultAsync(r => r.Id == id && r.Crop.UserId == userId);

            if (dbRecord == null)
                return NotFound();

            dbRecord.Action = farmRecords.Action;
            dbRecord.RecordDate = farmRecords.RecordDate;
            dbRecord.Description = farmRecords.Description;

            if (farmRecords.CropId != dbRecord.CropId)
            {
                var newCrop = await _context.Crops.FirstOrDefaultAsync(c => c.Id == farmRecords.CropId && c.UserId == userId);
                if (newCrop == null)
                    return Forbid();
                dbRecord.CropId = farmRecords.CropId;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"更新农事记录时发生错误: {ex.Message}");
            }

            return NoContent();
        }

        // DELETE: api/FarmRecords/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFarmRecords(int id)
        {
            int userId = GetCurrentUserId();
            var dbRecord = await _context.FarmRecords
                .Include(r => r.Crop)
                .FirstOrDefaultAsync(r => r.Id == id && r.Crop.UserId == userId);

            if (dbRecord == null)
                return NotFound();

            try
            {
                _context.FarmRecords.Remove(dbRecord);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"删除农事记录时发生错误: {ex.Message}");
            }

            return NoContent();
        }

        [HttpGet("export")]
        [Authorize]
        public async Task<IActionResult> ExportFarmRecords()
        {
            var farmRecords = await _context.FarmRecords.ToListAsync();
            var csv = new StringBuilder();
            csv.AppendLine("Id,CropId,Action,RecordDate,Description");
            foreach (var record in farmRecords)
            {
                csv.AppendLine($"{record.Id},{record.CropId},{record.Action},{record.RecordDate},{record.Description}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", "farmrecords.csv");
        }
    }
}

