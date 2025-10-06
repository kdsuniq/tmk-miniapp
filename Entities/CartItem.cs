namespace TelegramBotApi.Entities
{
    public class CartItem
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        public long NomenclatureId { get; set; }
        public Guid StockId { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = "m"; // "m" или "t"
        public decimal UnitPrice { get; set; }
        public string PriceTier { get; set; } = "Base"; // Уровень цены
        public decimal TotalPrice => Quantity * UnitPrice;
        
        public virtual Cart? Cart { get; set; }
        public virtual Nomenclature? Nomenclature { get; set; }
        public virtual Stock? Stock { get; set; }
    }
}