namespace TelegramBotApi.Entities
{
    public class Cart
    {
        public Guid Id { get; set; }
        public long UserId { get; set; } // ID пользователя Telegram
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public virtual ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}