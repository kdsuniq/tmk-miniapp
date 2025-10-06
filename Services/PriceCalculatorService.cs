using TelegramBotApi.Entities;

namespace TelegramBotApi.Services
{
    public class PriceCalculatorService
    {
        public (decimal UnitPrice, string Tier) CalculatePrice(Price price, decimal quantity, string unit)
        {
            if (unit == "m")
            {
                if (price.PriceLimitM2.HasValue && quantity >= price.PriceLimitM2)
                    return (price.PriceM2 ?? price.PriceM, "M2");
                if (price.PriceLimitM1.HasValue && quantity >= price.PriceLimitM1)
                    return (price.PriceM1 ?? price.PriceM, "M1");
                return (price.PriceM, "BaseM");
            }
            else
            {
                if (price.PriceLimitT2.HasValue && quantity >= price.PriceLimitT2)
                    return (price.PriceT2 ?? price.PriceT, "T2");
                if (price.PriceLimitT1.HasValue && quantity >= price.PriceLimitT1)
                    return (price.PriceT1 ?? price.PriceT, "T1");
                return (price.PriceT, "BaseT");
            }
        }
    }
}
