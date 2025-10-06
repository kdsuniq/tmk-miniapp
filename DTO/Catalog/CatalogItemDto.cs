namespace TelegramBotApi.DTO.Catalog
{
    public class CatalogItemDto
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Gost { get; set; }
        public string? SteelGrade { get; set; }
        public decimal Diameter { get; set; }
        public decimal PipeWallThickness { get; set; }
        public decimal Koef { get; set; }
        public string? Manufacturer { get; set; }
        public string? ProductionType { get; set; }
        public List<PriceDto> Prices { get; set; } = new();
        public List<RemnantDto> Remnants { get; set; } = new();
    }

    public class PriceDto
    {
        public long Id { get; set; }
        public Guid IdStock { get; set; }
        public string? StockName { get; set; }
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
    }

    public class RemnantDto
    {
        public long Id { get; set; }
        public Guid IdStock { get; set; }
        public string? StockName { get; set; }
        public decimal InStockT { get; set; }
        public decimal InStockM { get; set; }
        public decimal AvgTubeLength { get; set; }
        public decimal AvgTubeWeight { get; set; }
    }
}