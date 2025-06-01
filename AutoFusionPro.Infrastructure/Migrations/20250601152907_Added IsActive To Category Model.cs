using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoFusionPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsActiveToCategoryModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Categories_ParentCategoryId",
                table: "Categories");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Categories",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9597));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9600));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9603));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9605));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9607));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9610));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9612));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9614));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9656));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9659));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9661));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9664));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9667));

            migrationBuilder.UpdateData(
                table: "Makes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9037));

            migrationBuilder.UpdateData(
                table: "Makes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9037));

            migrationBuilder.UpdateData(
                table: "Makes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9037));

            migrationBuilder.UpdateData(
                table: "Makes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9037));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9037));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9037));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9037));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9037));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9037));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9037));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9037));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9542));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9545));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9547));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9549));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9551));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9037));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9037));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9037));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9037));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9037));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9037));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9037));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9037));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9037));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9037));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9037));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9037));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9037));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9037));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9037));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9037));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9037));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9037));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9037));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 1, 15, 29, 4, 960, DateTimeKind.Utc).AddTicks(9037));

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentCategoryId_Name",
                table: "Categories",
                columns: new[] { "ParentCategoryId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Categories_ParentCategoryId_Name",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Categories");

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(3494));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(3497));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(3499));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(3501));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(3503));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(3505));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(3507));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(3509));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(3548));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(3551));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(3553));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(3555));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(3557));

            migrationBuilder.UpdateData(
                table: "Makes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(2934));

            migrationBuilder.UpdateData(
                table: "Makes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(2934));

            migrationBuilder.UpdateData(
                table: "Makes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(2934));

            migrationBuilder.UpdateData(
                table: "Makes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(2934));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(2934));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(2934));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(2934));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(2934));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(2934));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(2934));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(2934));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(3446));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(3448));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(3450));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(3452));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(3454));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(2934));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(2934));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(2934));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(2934));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(2934));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(2934));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(2934));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(2934));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(2934));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(2934));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(2934));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(2934));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(2934));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(2934));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(2934));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(2934));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(2934));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(2934));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(2934));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedAt",
                value: new DateTime(2025, 5, 24, 10, 30, 58, 147, DateTimeKind.Utc).AddTicks(2934));

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentCategoryId",
                table: "Categories",
                column: "ParentCategoryId");
        }
    }
}
