using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AutoFusionPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedVehicleDataWithImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "Makes",
                columns: new[] { "Id", "CreatedAt", "CreatedByUserId", "ImagePath", "ModifiedAt", "ModifiedByUserId", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(1894), null, "pack://application:,,,/AutoFusionPro.UI;component/Assets/VehicleStructure/Makes/Nissan.jpg", null, null, "نيسان" },
                    { 2, new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(1894), null, "pack://application:,,,/AutoFusionPro.UI;component/Assets/VehicleStructure/Makes/Toyota.png", null, null, "تويوتا" },
                    { 3, new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(1894), null, "pack://application:,,,/AutoFusionPro.UI;component/Assets/VehicleStructure/Makes/Kia.jpg", null, null, "كيا" },
                    { 4, new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(1894), null, "pack://application:,,,/AutoFusionPro.UI;component/Assets/VehicleStructure/Makes/Hyundai.png", null, null, "هيونداي" }
                });

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

            migrationBuilder.InsertData(
                table: "Models",
                columns: new[] { "Id", "CreatedAt", "CreatedByUserId", "ImagePath", "MakeId", "ModifiedAt", "ModifiedByUserId", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(1894), null, "pack://application:,,,/AutoFusionPro.UI;component/Assets/VehicleStructure/Models/Altima.png", 1, null, null, "التيما" },
                    { 2, new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(1894), null, "pack://application:,,,/AutoFusionPro.UI;component/Assets/VehicleStructure/Models/Rouge.png", 1, null, null, "روج" },
                    { 3, new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(1894), null, "pack://application:,,,/AutoFusionPro.UI;component/Assets/VehicleStructure/Models/Sentra.png", 1, null, null, "سنترا" },
                    { 4, new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(1894), null, "pack://application:,,,/AutoFusionPro.UI;component/Assets/VehicleStructure/Models/Sunny.jpg", 1, null, null, "صني" },
                    { 5, new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(1894), null, "pack://application:,,,/AutoFusionPro.UI;component/Assets/VehicleStructure/Models/Kicks.jpg", 1, null, null, "كيكس" },
                    { 6, new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(1894), null, "pack://application:,,,/AutoFusionPro.UI;component/Assets/VehicleStructure/Models/Maxima.png", 1, null, null, "ماكزيما" },
                    { 7, new DateTime(2025, 5, 24, 9, 13, 24, 944, DateTimeKind.Utc).AddTicks(1894), null, "pack://application:,,,/AutoFusionPro.UI;component/Assets/VehicleStructure/Models/ََQashqqai.png", 1, null, null, "قاشقاي" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Makes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Makes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Makes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Makes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(662));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(666));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(668));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(671));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(673));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(675));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(677));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(679));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(730));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(744));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(747));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(750));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(753));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(400));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(413));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(416));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(418));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 23, 9, 51, 59, 579, DateTimeKind.Utc).AddTicks(421));
        }
    }
}
