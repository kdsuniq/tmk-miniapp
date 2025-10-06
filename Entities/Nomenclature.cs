namespace TelegramBotApi.Entities
{
    public class Nomenclature
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Gost { get; set; }
        public string? SteelGrade { get; set; }
        public decimal Diameter { get; set; }
        public decimal PipeWallThickness { get; set; }
        public decimal Koef { get; set; } // тонн/метр
        public string? Manufacturer { get; set; }
        public string? ProductionType { get; set; }

        public virtual ICollection<Price> Prices { get; set; } = new List<Price>();
        public virtual ICollection<Remnant> Remnants { get; set; } = new List<Remnant>();
    }
} 
