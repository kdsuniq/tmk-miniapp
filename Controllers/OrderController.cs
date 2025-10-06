using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TelegramBotApi.Data;
using TelegramBotApi.Entities;
using TelegramBotApi.DTO.Order;

namespace TelegramBotApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly TMKDbContext _db;

        public OrderController(TMKDbContext db)
        {
            _db = db;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                // Находим корзину пользователя
                var cart = await _db.Carts
                    .Include(c => c.Items)
                        .ThenInclude(i => i.Nomenclature)
                    .Include(c => c.Items)
                        .ThenInclude(i => i.Stock)
                    .FirstOrDefaultAsync(c => c.UserId == request.UserId);

                if (cart == null || !cart.Items.Any())
                    return BadRequest("Корзина пуста");

                // Создаем заказ
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    CartId = cart.Id,
                    CustomerName = request.CustomerName,
                    Phone = request.Phone,
                    Email = request.Email,
                    Address = request.Address,
                    Status = "Pending",
                    TotalAmount = cart.Items.Sum(i => i.Quantity * i.UnitPrice),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Копируем товары из корзины в заказ
                foreach (var cartItem in cart.Items)
                {
                    var orderItem = new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
                        NomenclatureId = cartItem.NomenclatureId,
                        StockId = cartItem.StockId,
                        Quantity = cartItem.Quantity,
                        Unit = cartItem.Unit,
                        UnitPrice = cartItem.UnitPrice,
                        PriceTier = cartItem.PriceTier
                    };
                    order.Items.Add(orderItem);
                }

                // Сохраняем заказ
                await _db.Orders.AddAsync(order);
                
                // Очищаем корзину после создания заказа
                _db.CartItems.RemoveRange(cart.Items);
                
                await _db.SaveChangesAsync();

                return Ok(new { 
                    Message = "Заказ успешно создан", 
                    OrderId = order.Id,
                    TotalAmount = order.TotalAmount
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserOrders(long userId)
        {
            var orders = await _db.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Nomenclature)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Stock)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            var orderDtos = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                CustomerName = o.CustomerName,
                Phone = o.Phone,
                CreatedAt = o.CreatedAt,
                Items = o.Items.Select(i => new OrderItemDto
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
                    TotalPrice = i.TotalPrice
                }).ToList()
            }).ToList();

            return Ok(orderDtos);
        }

        [HttpGet("details/{orderId}")]
        public async Task<IActionResult> GetOrderDetails(Guid orderId)
        {
            var order = await _db.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Nomenclature)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Stock)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return NotFound("Заказ не найден");

            var orderDto = new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                CustomerName = order.CustomerName,
                Phone = order.Phone,
                Email = order.Email,
                Address = order.Address,
                CreatedAt = order.CreatedAt,
                Items = order.Items.Select(i => new OrderItemDto
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
                    TotalPrice = i.TotalPrice
                }).ToList()
            };

            return Ok(orderDto);
        }
    }

    public class CreateOrderRequest
    {
        public long UserId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}