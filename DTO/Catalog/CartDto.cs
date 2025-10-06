namespace TelegramBotApi.DTO.Cart
{
    public class CartDto
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
    }

    public class CartItemDto
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