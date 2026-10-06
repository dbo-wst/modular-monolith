using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wst.Trios.Core.BrokerSide.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBrokerSideModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "brokerSide");

            migrationBuilder.CreateTable(
                name: "Orders",
                schema: "brokerSide",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    BrokerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InstrumentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VenueId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Number = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    EffectiveDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Side = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                });

            migrationBuilder.CreateTable(
                name: "Trades",
                schema: "brokerSide",
                columns: table => new
                {
                    TradeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EffectiveDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trades", x => x.TradeId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders",
                schema: "brokerSide");

            migrationBuilder.DropTable(
                name: "Trades",
                schema: "brokerSide");
        }
    }
}
