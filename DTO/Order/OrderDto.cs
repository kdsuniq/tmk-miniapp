namespace TelegramBotApi.DTO.Order
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public string Status { get; set; } = "Pending";
        public decimal TotalAmount { get; set; }
        public string CustomerLastName { get; set; } = string.Empty;
        public string INN { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public Guid Id { get; set; }
        public long NomenclatureId { get; set; }
        public string? NomenclatureName { get; set; }
        public Guid StockId { get; set; }
        public string? StockName { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = "m";
        public decimal UnitPrice { get; set; }
        public string PriceTier { get; set; } = "Base";
        public decimal TotalPrice { get; set; }
    }
}