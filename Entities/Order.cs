namespace TelegramBotApi.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public Guid CartId { get; set; }

        // Данные клиента
        public string INN { get; set; } = string.Empty;
        public string CustomerLastName { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    

        // Статус и сумма
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Shipped, Delivered, Cancelled
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Навигационные свойства
        public virtual Cart Cart { get; set; } = null!;
        public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public long NomenclatureId { get; set; }
        public Guid StockId { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = "m";
        public decimal UnitPrice { get; set; }
        public string PriceTier { get; set; } = "Base";
        public decimal TotalPrice => Quantity * UnitPrice;
        
        // Навигационные свойства
        public virtual Order Order { get; set; } = null!;
        public virtual Nomenclature Nomenclature { get; set; } = null!;
        public virtual Stock Stock { get; set; } = null!;
    }
}