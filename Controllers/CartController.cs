using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TelegramBotApi.Data;
using TelegramBotApi.Entities;
using TelegramBotApi.Services;
using TelegramBotApi.DTO.Cart; // Добавь using для DTO

namespace TelegramBotApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly TMKDbContext _db;
        private readonly PriceCalculatorService _priceCalculator;

        public CartController(TMKDbContext db, PriceCalculatorService priceCalculator)
        {
            _db = db;
            _priceCalculator = priceCalculator;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCart(long userId)
        {
            var cart = await _db.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Nomenclature)
                .Include(c => c.Items)
                    .ThenInclude(i => i.Stock)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                // ✅ Возвращаем DTO вместо сущности
                return Ok(new CartDto { UserId = userId, Items = new List<CartItemDto>() });
            }

            // ✅ Преобразуем в DTO чтобы избежать циклических ссылок
            var cartDto = new CartDto
            {
                Id = cart.Id,
                UserId = cart.UserId,
                CreatedAt = cart.CreatedAt,
                UpdatedAt = cart.UpdatedAt,
                Items = cart.Items.Select(i => new CartItemDto
                {
                    Id = i.Id,
                    NomenclatureId = i.NomenclatureId,
                    NomenclatureName = i.Nomenclature?.Name,
                    StockId = i.StockId,
                    StockName = i.Stock?.StockName,
                    Quantity = i.Quantity,
                    Unit = i.Unit,
                    UnitPrice = i.UnitPrice,
                    PriceTier = i.PriceTier,
                    TotalPrice = i.Quantity * i.UnitPrice
                }).ToList()
            };

            return Ok(cartDto);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            try
            {
                // ✅ Проверяем существование корзины
                var existingCart = await _db.Carts
                    .FirstOrDefaultAsync(c => c.UserId == request.UserId);

                Cart cart;
                if (existingCart == null)
                {
                    cart = new Cart 
                    { 
                        Id = Guid.NewGuid(),
                        UserId = request.UserId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _db.Carts.AddAsync(cart);
                }
                else
                {
                    cart = existingCart;
                }
                
                var cartItem = new CartItem
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    NomenclatureId = request.NomenclatureId,
                    StockId = request.StockId,
                    Quantity = request.Quantity,
                    Unit = request.Unit,
                    UnitPrice = 1000, // Временно фиксированная цена
                    PriceTier = "Base"
                };

                await _db.CartItems.AddAsync(cartItem);
                await _db.SaveChangesAsync();

                return Ok(new { Message = "Товар добавлен в корзину", CartId = cart.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemRequest request)
        {
            var cartItem = await _db.CartItems
                .FirstOrDefaultAsync(ci => ci.Id == request.ItemId);

            if (cartItem == null)
                return NotFound("Элемент корзины не найден");

            if (request.Quantity <= 0)
                return BadRequest("Количество должно быть больше 0");

            cartItem.Quantity = request.Quantity;
            
            // Обновляем время корзины
            var cart = await _db.Carts.FindAsync(cartItem.CartId);
            if (cart != null)
            {
                cart.UpdatedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();

            return Ok(new { Message = "Количество обновлено" });
        }

        [HttpDelete("remove/{itemId}")]
        public async Task<IActionResult> RemoveFromCart(Guid itemId)
        {
            var cartItem = await _db.CartItems
                .FirstOrDefaultAsync(ci => ci.Id == itemId);

            if (cartItem == null)
                return NotFound("Элемент корзины не найден");

            _db.CartItems.Remove(cartItem);
            
            // Обновляем время корзины
            var cart = await _db.Carts.FindAsync(cartItem.CartId);
            if (cart != null)
            {
                cart.UpdatedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();

            return Ok(new { Message = "Товар удален из корзины" });
        }
    }

    public class AddToCartRequest
    {
        public long UserId { get; set; }
        public long NomenclatureId { get; set; }
        public Guid StockId { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = "m"; // "m" или "t"
    }

    public class UpdateCartItemRequest
    {
        public Guid ItemId { get; set; }
        public decimal Quantity { get; set; }
    }
}