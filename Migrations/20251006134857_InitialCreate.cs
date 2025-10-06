using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramBotApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Nomenclatures",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Gost = table.Column<string>(type: "TEXT", nullable: true),
                    SteelGrade = table.Column<string>(type: "TEXT", nullable: true),
                    Diameter = table.Column<decimal>(type: "TEXT", nullable: false),
                    PipeWallThickness = table.Column<decimal>(type: "TEXT", nullable: false),
                    Koef = table.Column<decimal>(type: "TEXT", nullable: false),
                    Manufacturer = table.Column<string>(type: "TEXT", nullable: true),
                    ProductionType = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nomenclatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    IdStock = table.Column<Guid>(type: "TEXT", nullable: false),
                    PriceT = table.Column<decimal>(type: "TEXT", nullable: false),
                    PriceLimitT1 = table.Column<decimal>(type: "TEXT", nullable: true),
                    PriceT1 = table.Column<decimal>(type: "TEXT", nullable: true),
                    PriceLimitT2 = table.Column<decimal>(type: "TEXT", nullable: true),
                    PriceT2 = table.Column<decimal>(type: "TEXT", nullable: true),
                    PriceM = table.Column<decimal>(type: "TEXT", nullable: false),
                    PriceLimitM1 = table.Column<decimal>(type: "TEXT", nullable: true),
                    PriceM1 = table.Column<decimal>(type: "TEXT", nullable: true),
                    PriceLimitM2 = table.Column<decimal>(type: "TEXT", nullable: true),
                    PriceM2 = table.Column<decimal>(type: "TEXT", nullable: true),
                    NDS = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => new { x.Id, x.IdStock });
                });

            migrationBuilder.CreateTable(
                name: "Remnants",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    IdStock = table.Column<Guid>(type: "TEXT", nullable: false),
                    InStockT = table.Column<decimal>(type: "TEXT", nullable: false),
                    InStockM = table.Column<decimal>(type: "TEXT", nullable: false),
                    AvgTubeLength = table.Column<decimal>(type: "TEXT", nullable: false),
                    AvgTubeWeight = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Remnants", x => new { x.Id, x.IdStock });
                });

            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    IdStock = table.Column<Guid>(type: "TEXT", nullable: false),
                    StockName = table.Column<string>(type: "TEXT", nullable: false),
                    WarehouseCode = table.Column<string>(type: "TEXT", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.IdStock);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Nomenclatures");

            migrationBuilder.DropTable(
                name: "Prices");

            migrationBuilder.DropTable(
                name: "Remnants");

            migrationBuilder.DropTable(
                name: "Stocks");
        }
    }
}
