using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AutoFusionPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedVehicleData2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(7166));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(7168));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(7170));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(7172));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(7174));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(7176));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(7179));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(7180));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(7222));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(7224));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(7226));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(7228));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(7230));

            migrationBuilder.UpdateData(
                table: "Makes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(6623));

            migrationBuilder.UpdateData(
                table: "Makes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(6623));

            migrationBuilder.UpdateData(
                table: "Makes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(6623));

            migrationBuilder.UpdateData(
                table: "Makes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(6623));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(6623));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "ImagePath" },
                values: new object[] { new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(6623), "pack://application:,,,/AutoFusionPro.UI;component/Assets/VehicleStructure/Models/Rogue.png" });

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(6623));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(6623));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(6623));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(6623));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(6623));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(7105));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(7116));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(7117));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(7119));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(7122));

            migrationBuilder.InsertData(
                table: "TrimLevels",
                columns: new[] { "Id", "CreatedAt", "CreatedByUserId", "ModelId", "ModifiedAt", "ModifiedByUserId", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(6623), null, 1, null, null, "S" },
                    { 2, new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(6623), null, 1, null, null, "SV" },
                    { 3, new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(6623), null, 1, null, null, "SR" },
                    { 4, new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(6623), null, 1, null, null, "SL" },
                    { 5, new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(6623), null, 1, null, null, "Platinum" },
                    { 6, new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(6623), null, 3, null, null, "S" },
                    { 7, new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(6623), null, 3, null, null, "SV" },
                    { 8, new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(6623), null, 3, null, null, "SR" },
                    { 9, new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(6623), null, 2, null, null, "S" },
                    { 10, new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(6623), null, 2, null, null, "SV" },
                    { 11, new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(6623), null, 2, null, null, "SL" },
                    { 12, new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(6623), null, 4, null, null, "S" },
                    { 13, new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(6623), null, 4, null, null, "SV" },
                    { 14, new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(6623), null, 4, null, null, "SL" },
                    { 15, new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(6623), null, 6, null, null, "S" },
                    { 16, new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(6623), null, 6, null, null, "SV" },
                    { 17, new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(6623), null, 6, null, null, "SL" },
                    { 18, new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(6623), null, 6, null, null, "SR" },
                    { 19, new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(6623), null, 6, null, null, "Platinum" },
                    { 20, new DateTime(2025, 5, 24, 10, 28, 35, 619, DateTimeKind.Utc).AddTicks(6623), null, 6, null, null, "Platinum Reserve" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(2323));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(2325));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(2328));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(2330));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(2332));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(2334));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(2336));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(2347));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(2392));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(2395));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(2397));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(2399));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(2402));

            migrationBuilder.UpdateData(
                table: "Makes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(1894));

            migrationBuilder.UpdateData(
                table: "Makes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(1894));

            migrationBuilder.UpdateData(
                table: "Makes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(1894));

            migrationBuilder.UpdateData(
                table: "Makes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(1894));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(1894));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "ImagePath" },
                values: new object[] { new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(1894), "pack://application:,,,/AutoFusionPro.UI;component/Assets/VehicleStructure/Models/Rouge.png" });

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(1894));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(1894));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(1894));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(1894));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(1894));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(2273));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(2275));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(2278));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(2280));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(2282));
        }
    }
}
