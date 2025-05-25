using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AutoFusionPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddplaceholderLogoformakesandmodels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Models",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Makes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.InsertData(
                table: "BodyTypes",
                columns: new[] { "Id", "CreatedAt", "CreatedByUserId", "ModifiedAt", "ModifiedByUserId", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(662), null, null, null, "Sedan" },
                    { 2, new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(666), null, null, null, "SUV (Sport Utility Vehicle)" },
                    { 3, new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(668), null, null, null, "Hatchback" },
                    { 4, new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(671), null, null, null, "Coupe" },
                    { 5, new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(673), null, null, null, "Convertible" },
                    { 6, new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(675), null, null, null, "Minivan" },
                    { 7, new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(677), null, null, null, "Truck (Pickup)" },
                    { 8, new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(679), null, null, null, "Wagon (Estate)" }
                });

            migrationBuilder.InsertData(
                table: "EngineTypes",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedByUserId", "ModifiedAt", "ModifiedByUserId", "Name" },
                values: new object[,]
                {
                    { 1, "I4_GAS", new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(730), null, null, null, "Gasoline - Inline 4 (I4)" },
                    { 2, "V6_GAS", new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(744), null, null, null, "Gasoline - V6" },
                    { 3, "I4_DSL", new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(747), null, null, null, "Diesel - Inline 4 (I4)" },
                    { 4, "ELEC", new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(750), null, null, null, "Electric" },
                    { 5, "HYB", new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(753), null, null, null, "Hybrid" }
                });

            migrationBuilder.InsertData(
                table: "TransmissionTypes",
                columns: new[] { "Id", "CreatedAt", "CreatedByUserId", "ModifiedAt", "ModifiedByUserId", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(400), null, null, null, "Automatic" },
                    { 2, new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(413), null, null, null, "Manual" },
                    { 3, new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(416), null, null, null, "CVT (Continuously Variable Transmission)" },
                    { 4, new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(418), null, null, null, "Semi-Automatic" },
                    { 5, new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(421), null, null, null, "Dual-Clutch (DCT)" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Models");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Makes");
        }
    }
}
