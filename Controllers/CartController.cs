using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TelegramBotApi.Data;
using TelegramBotApi.Entities;
// using TelegramBotApi.Services; // временно закомментировать

namespace TelegramBotApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly TMKDbContext _db;
        // private readonly PriceCalculatorService _priceCalculator;

        public CartController(TMKDbContext db /*, PriceCalculatorService priceCalculator*/)
        {
            _db = db;
            // _priceCalculator = priceCalculator;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(long userId)
        {
            // временно возвращаем пустую корзину
            return Ok(new Cart { UserId = userId });
        }

        // остальные методы пока можно оставить пустыми
        [HttpPost("add")] public async Task<IActionResult> AddToCart([FromBody] object request) => Ok();
        [HttpPut("update")] public async Task<IActionResult> UpdateCartItem([FromBody] object request) => Ok();
        [HttpDelete("remove/{itemId}")] public async Task<IActionResult> RemoveFromCart(Guid itemId) => Ok();
    }
}