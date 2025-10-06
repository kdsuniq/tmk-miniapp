namespace TelegramBotApi.DTO.Catalog
{
    public class CatalogFilterDto
    {
        public string? SteelGrade { get; set; }
        public decimal? DiameterMin { get; set; }
        public decimal? DiameterMax { get; set; }
        public decimal? WallMin { get; set; }
        public decimal? WallMax { get; set; }
        public string? Gost { get; set; }
        public string? ProductionType { get; set; }
        public int? StockId { get; set; }

    }
}
