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
            [FromQuery] decimal? diameterMax,
            [FromQuery] decimal? wallMin,
            [FromQuery] decimal? wallMax,
            [FromQuery] string? steelGrade,
            [FromQuery] string? gost,
            [FromQuery] string? productionType,
            [FromQuery] Guid? stockId)
        {
            var query = _db.Nomenclatures
                .Include(n => n.Prices)
                    .ThenInclude(p => p.Stock)
                .Include(n => n.Remnants)
                    .ThenInclude(r => r.Stock)
                .AsQueryable();

            // Фильтрация по диаметру
            if (diameterMin.HasValue)
                query = query.Where(x => x.Diameter >= diameterMin.Value);
            if (diameterMax.HasValue)
                query = query.Where(x => x.Diameter <= diameterMax.Value);

            // Фильтрация по толщине стенки
            if (wallMin.HasValue)
                query = query.Where(x => x.PipeWallThickness >= wallMin.Value);
            if (wallMax.HasValue)
                query = query.Where(x => x.PipeWallThickness <= wallMax.Value);

            // Фильтрация по марке стали
            if (!string.IsNullOrEmpty(steelGrade))
                query = query.Where(x => x.SteelGrade != null && x.SteelGrade.Contains(steelGrade));

            // Фильтрация по ГОСТ
            if (!string.IsNullOrEmpty(gost))
                query = query.Where(x => x.Gost != null && x.Gost.Contains(gost));

            // Фильтрация по виду продукции
            if (!string.IsNullOrEmpty(productionType))
                query = query.Where(x => x.ProductionType != null && x.ProductionType.Contains(productionType));

            // Фильтрация по складу
            if (stockId.HasValue)
            {
                query = query.Where(x => x.Prices.Any(p => p.IdStock == stockId.Value) ||
                                       x.Remnants.Any(r => r.IdStock == stockId.Value));
            }

            var result = await query.Take(50).ToListAsync();
            return Ok(result);
        }
    }
}