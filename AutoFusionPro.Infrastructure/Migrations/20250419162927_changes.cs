using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoFusionPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastRestockDate",
                table: "Parts",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ReorderLevel",
                table: "Parts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StockQuantity",
                table: "Parts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastRestockDate",
                table: "Parts");

            migrationBuilder.DropColumn(
                name: "ReorderLevel",
                table: "Parts");

            migrationBuilder.DropColumn(
                name: "StockQuantity",
                table: "Parts");
        }
    }
}
