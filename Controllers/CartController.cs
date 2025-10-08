using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TelegramBotApi.Data;
using TelegramBotApi.Entities;
using TelegramBotApi.Services;
using TelegramBotApi.DTO.Cart;

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
                // Возвращаем DTO вместо сущности
                return Ok(new CartDto { UserId = userId, Items = new List<CartItemDto>() });
            }

            // Преобразуем в DTO чтобы избежать циклических ссылок
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
                // Проверяем существование товара и склада
                var nomenclatureExists = await _db.Nomenclatures.AnyAsync(n => n.Id == request.NomenclatureId);
                var stockExists = await _db.Stocks.AnyAsync(s => s.IdStock == request.StockId);

                if (!nomenclatureExists)
                    return BadRequest("Товар не найден");
                if (!stockExists)
                    return BadRequest("Склад не найден");

                // Находим цену для этого товара и склада
                var price = await _db.Prices
                    .FirstOrDefaultAsync(p => p.Id == request.NomenclatureId && p.IdStock == request.StockId);

                if (price == null)
                    return BadRequest("Цена для данного товара и склада не найдена");

                // цену с учетом объемных скидок
                var (unitPrice, priceTier) = _priceCalculator.CalculatePrice(price, request.Quantity, request.Unit);

                // Находим или создаем корзину
                var cart = await _db.Carts
                    .FirstOrDefaultAsync(c => c.UserId == request.UserId);

                if (cart == null)
                {
                    cart = new Cart
                    {
                        Id = Guid.NewGuid(),
                        UserId = request.UserId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _db.Carts.AddAsync(cart);
                    await _db.SaveChangesAsync(); // Сохраняем сначала корзину
                }

                // Проверяем есть ли уже такой товар в корзине
                var existingItem = await _db.CartItems
                    .FirstOrDefaultAsync(i =>
                        i.CartId == cart.Id &&
                        i.NomenclatureId == request.NomenclatureId &&
                        i.StockId == request.StockId &&
                        i.Unit == request.Unit);

                if (existingItem != null)
                {
                    // Обновляем количество и ПЕРЕСЧИТЫВАЕМ цену
                    existingItem.Quantity += request.Quantity;
                    var (newUnitPrice, newPriceTier) = _priceCalculator.CalculatePrice(price, existingItem.Quantity, request.Unit);
                    existingItem.UnitPrice = newUnitPrice;
                    existingItem.PriceTier = newPriceTier;
                }
                else
                {
                    // Добавляем новый товар с РЕАЛЬНОЙ ценой
                    var cartItem = new CartItem
                    {
                        Id = Guid.NewGuid(),
                        CartId = cart.Id,
                        NomenclatureId = request.NomenclatureId,
                        StockId = request.StockId,
                        Quantity = request.Quantity,
                        Unit = request.Unit,
                        UnitPrice = unitPrice, // цена
                        PriceTier = priceTier  // уровень скидки
                    };
                    await _db.CartItems.AddAsync(cartItem);
                }

                cart.UpdatedAt = DateTime.UtcNow;
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
            try
            {
                var cartItem = await _db.CartItems
                    .Include(ci => ci.Cart)
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
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
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
}