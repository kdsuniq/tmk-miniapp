using Microsoft.EntityFrameworkCore;
using TelegramBotApi.Entities;

namespace TelegramBotApi.Data
{
    public class TMKDbContext : DbContext
    {
        public TMKDbContext(DbContextOptions<TMKDbContext> options) : base(options) { }

        public DbSet<Nomenclature> Nomenclatures { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Remnant> Remnants { get; set; }
        public DbSet<Stock> Stocks { get; set; }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Stock>().HasKey(s => s.IdStock);
            modelBuilder.Entity<Price>().HasKey(p => new { p.Id, p.IdStock });
            modelBuilder.Entity<Remnant>().HasKey(r => new { r.Id, r.IdStock });

            modelBuilder.Entity<Price>()
                .HasOne(p => p.Nomenclature)
                .WithMany(n => n.Prices)
                .HasForeignKey(p => p.Id)
                .HasPrincipalKey(n => n.Id);

            modelBuilder.Entity<Price>()
                .HasOne(p => p.Stock)
                .WithMany()
                .HasForeignKey(p => p.IdStock);

            modelBuilder.Entity<Remnant>()
                .HasOne(r => r.Nomenclature)
                .WithMany(n => n.Remnants)
                .HasForeignKey(r => r.Id)
                .HasPrincipalKey(n => n.Id);

            modelBuilder.Entity<Remnant>()
                .HasOne(r => r.Stock)
                .WithMany()
                .HasForeignKey(r => r.IdStock);

            modelBuilder.Entity<Cart>().HasKey(c => c.Id);
            modelBuilder.Entity<CartItem>().HasKey(ci => ci.Id);

            modelBuilder.Entity<Cart>()
                .HasMany(c => c.Items)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId);

            modelBuilder.Entity<Order>().HasKey(o => o.Id);
            modelBuilder.Entity<OrderItem>().HasKey(oi => oi.Id);
        }
    }
}
