﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AIMS.Server.Models;

namespace AIMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PricesController : ControllerBase
    {
        private readonly AMDBContext _context;

        public PricesController(AMDBContext context)
        {
            _context = context;
        }

        // GET: api/Prices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Prices>>> GetPrices()
        {
            return await _context.Prices.ToListAsync();
        }

        // GET: api/Prices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Prices>> GetPrices(int id)
        {
            var prices = await _context.Prices.FindAsync(id);

            if (prices == null)
            {
                return NotFound();
            }

            return prices;
        }

        // PUT: api/Prices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrices(int id, Prices prices)
        {
            if (id != prices.Id)
            {
                return BadRequest();
            }

            _context.Entry(prices).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PricesExists(id))
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

        // POST: api/Prices
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Prices>> PostPrices(Prices prices)
        {
            _context.Prices.Add(prices);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPrices", new { id = prices.Id }, prices);
        }

        // DELETE: api/Prices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrices(int id)
        {
            var prices = await _context.Prices.FindAsync(id);
            if (prices == null)
            {
                return NotFound();
            }

            _context.Prices.Remove(prices);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PricesExists(int id)
        {
            return _context.Prices.Any(e => e.Id == id);
        }
    }
}
