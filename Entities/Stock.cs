using System;
using System.ComponentModel.DataAnnotations;

namespace TelegramBotApi.Entities
{
    public class Stock
    {
        [Key]
        public Guid IdStock { get; set; }

        public string? StockName { get; set; }
        public string? WarehouseCode { get; set; }
        public string? Address { get; set; }
    }
}