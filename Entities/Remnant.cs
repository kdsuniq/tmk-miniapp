namespace TelegramBotApi.Entities
{
    public class Remnant
    {
        public long Id { get; set; }
        public Guid IdStock { get; set; }
        public decimal InStockT { get; set; }
        public decimal InStockM { get; set; }
        public decimal AvgTubeLength { get; set; }
        public decimal AvgTubeWeight { get; set; }

        public virtual Nomenclature? Nomenclature { get; set; }
        public virtual Stock? Stock { get; set; }
    }
}
