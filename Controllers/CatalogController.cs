using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TelegramBotApi.Data;

namespace TelegramBotApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogController : ControllerBase
    {
        private readonly TMKDbContext _db;
        public CatalogController(TMKDbContext db) => _db = db;

                [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] decimal? diameterMin,
            [FromQuery] decimal? diameterMax)
        {
            var query = _db.Nomenclatures
                .Include(n => n.Prices)
                    .ThenInclude(p => p.Stock)  // Добавляем склад для цен
                .Include(n => n.Remnants)
                    .ThenInclude(r => r.Stock)  // Добавляем склад для остатков
                .AsQueryable();

            if (diameterMin.HasValue)
                query = query.Where(x => x.Diameter >= diameterMin.Value);
            if (diameterMax.HasValue)
                query = query.Where(x => x.Diameter <= diameterMax.Value);

            var result = await query.Take(50).ToListAsync();
            return Ok(result);
        }
    }
} 