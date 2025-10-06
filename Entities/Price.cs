namespace TelegramBotApi.Entities
{
    public class Price
    {
        public long Id { get; set; } // ID номенклатуры
        public Guid IdStock { get; set; }
        public decimal PriceT { get; set; }
        public decimal? PriceLimitT1 { get; set; }
        public decimal? PriceT1 { get; set; }
        public decimal? PriceLimitT2 { get; set; }
        public decimal? PriceT2 { get; set; }
        public decimal PriceM { get; set; }
        public decimal? PriceLimitM1 { get; set; }
        public decimal? PriceM1 { get; set; }
        public decimal? PriceLimitM2 { get; set; }
        public decimal? PriceM2 { get; set; }
        public int NDS { get; set; }

        public virtual Nomenclature? Nomenclature { get; set; }
        public virtual Stock? Stock { get; set; }
    }
}
