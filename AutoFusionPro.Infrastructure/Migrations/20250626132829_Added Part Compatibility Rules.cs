using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoFusionPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedPartCompatibilityRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartCompatibilities");

            migrationBuilder.DropTable(
                name: "CompatibleVehicles");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Parts");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "VehicleDocuments",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceNumber",
                table: "InventoryTransactions",
                type: "TEXT",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "InventoryTransactions",
                type: "TEXT",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500);

            migrationBuilder.CreateTable(
                name: "PartCompatibilityRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PartId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    MakeId = table.Column<int>(type: "INTEGER", nullable: true),
                    ModelId = table.Column<int>(type: "INTEGER", nullable: true),
                    YearStart = table.Column<int>(type: "INTEGER", nullable: true),
                    YearEnd = table.Column<int>(type: "INTEGER", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsTemplate = table.Column<bool>(type: "INTEGER", nullable: false),
                    CopiedFromRuleId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartCompatibilityRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartCompatibilityRules_Makes_MakeId",
                        column: x => x.MakeId,
                        principalTable: "Makes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PartCompatibilityRules_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PartCompatibilityRules_Parts_PartId",
                        column: x => x.PartId,
                        principalTable: "Parts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PartId = table.Column<int>(type: "INTEGER", nullable: false),
                    ImagePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Caption = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    IsPrimary = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartImages_Parts_PartId",
                        column: x => x.PartId,
                        principalTable: "Parts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartCompatibilityRuleBodyTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PartCompatibilityRuleId = table.Column<int>(type: "INTEGER", nullable: false),
                    BodyTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsExclusion = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartCompatibilityRuleBodyTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartCompatibilityRuleBodyTypes_BodyTypes_BodyTypeId",
                        column: x => x.BodyTypeId,
                        principalTable: "BodyTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PartCompatibilityRuleBodyTypes_PartCompatibilityRules_PartCompatibilityRuleId",
                        column: x => x.PartCompatibilityRuleId,
                        principalTable: "PartCompatibilityRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartCompatibilityRuleEngineTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PartCompatibilityRuleId = table.Column<int>(type: "INTEGER", nullable: false),
                    EngineTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsExclusion = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartCompatibilityRuleEngineTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartCompatibilityRuleEngineTypes_EngineTypes_EngineTypeId",
                        column: x => x.EngineTypeId,
                        principalTable: "EngineTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PartCompatibilityRuleEngineTypes_PartCompatibilityRules_PartCompatibilityRuleId",
                        column: x => x.PartCompatibilityRuleId,
                        principalTable: "PartCompatibilityRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartCompatibilityRuleTransmissionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PartCompatibilityRuleId = table.Column<int>(type: "INTEGER", nullable: false),
                    TransmissionTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsExclusion = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartCompatibilityRuleTransmissionTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartCompatibilityRuleTransmissionTypes_PartCompatibilityRules_PartCompatibilityRuleId",
                        column: x => x.PartCompatibilityRuleId,
                        principalTable: "PartCompatibilityRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartCompatibilityRuleTransmissionTypes_TransmissionTypes_TransmissionTypeId",
                        column: x => x.TransmissionTypeId,
                        principalTable: "TransmissionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PartCompatibilityRuleTrimLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PartCompatibilityRuleId = table.Column<int>(type: "INTEGER", nullable: false),
                    TrimLevelId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsExclusion = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartCompatibilityRuleTrimLevels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartCompatibilityRuleTrimLevels_PartCompatibilityRules_PartCompatibilityRuleId",
                        column: x => x.PartCompatibilityRuleId,
                        principalTable: "PartCompatibilityRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartCompatibilityRuleTrimLevels_TrimLevels_TrimLevelId",
                        column: x => x.TrimLevelId,
                        principalTable: "TrimLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1734));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1737));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1739));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1740));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1742));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1744));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1745));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1747));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1789));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1791));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1793));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1795));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1797));

            migrationBuilder.UpdateData(
                table: "Makes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "Makes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "Makes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "Makes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1684));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1687));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1689));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1691));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1693));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1116));

            migrationBuilder.UpdateData(
                table: "UnitOfMeasures",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1842));

            migrationBuilder.UpdateData(
                table: "UnitOfMeasures",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1845));

            migrationBuilder.UpdateData(
                table: "UnitOfMeasures",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1848));

            migrationBuilder.UpdateData(
                table: "UnitOfMeasures",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1851));

            migrationBuilder.UpdateData(
                table: "UnitOfMeasures",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1853));

            migrationBuilder.UpdateData(
                table: "UnitOfMeasures",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1856));

            migrationBuilder.UpdateData(
                table: "UnitOfMeasures",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 26, 13, 28, 27, 60, DateTimeKind.Utc).AddTicks(1858));

            migrationBuilder.CreateIndex(
                name: "IX_PartCompatibilityRuleBodyTypes_BodyTypeId",
                table: "PartCompatibilityRuleBodyTypes",
                column: "BodyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PartCompatibilityRuleBodyTypes_PartCompatibilityRuleId_BodyTypeId",
                table: "PartCompatibilityRuleBodyTypes",
                columns: new[] { "PartCompatibilityRuleId", "BodyTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PartCompatibilityRuleEngineTypes_EngineTypeId",
                table: "PartCompatibilityRuleEngineTypes",
                column: "EngineTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PartCompatibilityRuleEngineTypes_PartCompatibilityRuleId_EngineTypeId",
                table: "PartCompatibilityRuleEngineTypes",
                columns: new[] { "PartCompatibilityRuleId", "EngineTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PartCompatibilityRules_IsTemplate",
                table: "PartCompatibilityRules",
                column: "IsTemplate");

            migrationBuilder.CreateIndex(
                name: "IX_PartCompatibilityRules_MakeId",
                table: "PartCompatibilityRules",
                column: "MakeId");

            migrationBuilder.CreateIndex(
                name: "IX_PartCompatibilityRules_ModelId",
                table: "PartCompatibilityRules",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_PartCompatibilityRules_PartId",
                table: "PartCompatibilityRules",
                column: "PartId");

            migrationBuilder.CreateIndex(
                name: "IX_PartCompatibilityRuleTransmissionTypes_PartCompatibilityRuleId_TransmissionTypeId",
                table: "PartCompatibilityRuleTransmissionTypes",
                columns: new[] { "PartCompatibilityRuleId", "TransmissionTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PartCompatibilityRuleTransmissionTypes_TransmissionTypeId",
                table: "PartCompatibilityRuleTransmissionTypes",
                column: "TransmissionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PartCompatibilityRuleTrimLevels_PartCompatibilityRuleId_TrimLevelId",
                table: "PartCompatibilityRuleTrimLevels",
                columns: new[] { "PartCompatibilityRuleId", "TrimLevelId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PartCompatibilityRuleTrimLevels_TrimLevelId",
                table: "PartCompatibilityRuleTrimLevels",
                column: "TrimLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_PartImages_PartId",
                table: "PartImages",
                column: "PartId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartCompatibilityRuleBodyTypes");

            migrationBuilder.DropTable(
                name: "PartCompatibilityRuleEngineTypes");

            migrationBuilder.DropTable(
                name: "PartCompatibilityRuleTransmissionTypes");

            migrationBuilder.DropTable(
                name: "PartCompatibilityRuleTrimLevels");

            migrationBuilder.DropTable(
                name: "PartImages");

            migrationBuilder.DropTable(
                name: "PartCompatibilityRules");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "VehicleDocuments");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Parts",
                type: "TEXT",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReferenceNumber",
                table: "InventoryTransactions",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "InventoryTransactions",
                type: "TEXT",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CompatibleVehicles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BodyTypeId = table.Column<int>(type: "INTEGER", nullable: true),
                    EngineTypeId = table.Column<int>(type: "INTEGER", nullable: true),
                    ModelId = table.Column<int>(type: "INTEGER", nullable: false),
                    TransmissionTypeId = table.Column<int>(type: "INTEGER", nullable: true),
                    TrimLevelId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    VIN = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    YearEnd = table.Column<int>(type: "INTEGER", nullable: false),
                    YearStart = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompatibleVehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompatibleVehicles_BodyTypes_BodyTypeId",
                        column: x => x.BodyTypeId,
                        principalTable: "BodyTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompatibleVehicles_EngineTypes_EngineTypeId",
                        column: x => x.EngineTypeId,
                        principalTable: "EngineTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompatibleVehicles_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompatibleVehicles_TransmissionTypes_TransmissionTypeId",
                        column: x => x.TransmissionTypeId,
                        principalTable: "TransmissionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompatibleVehicles_TrimLevels_TrimLevelId",
                        column: x => x.TrimLevelId,
                        principalTable: "TrimLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PartCompatibilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CompatibleVehicleId = table.Column<int>(type: "INTEGER", nullable: false),
                    PartId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartCompatibilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartCompatibilities_CompatibleVehicles_CompatibleVehicleId",
                        column: x => x.CompatibleVehicleId,
                        principalTable: "CompatibleVehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PartCompatibilities_Parts_PartId",
                        column: x => x.PartId,
                        principalTable: "Parts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(8050));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(8052));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(8054));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(8057));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(8059));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(8061));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(8063));

            migrationBuilder.UpdateData(
                table: "BodyTypes",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(8065));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(8120));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(8122));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(8125));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(8127));

            migrationBuilder.UpdateData(
                table: "EngineTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(8129));

            migrationBuilder.UpdateData(
                table: "Makes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "Makes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "Makes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "Makes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "Models",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7984));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7987));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7989));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7991));

            migrationBuilder.UpdateData(
                table: "TransmissionTypes",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7993));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 9,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 10,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 11,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 12,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 13,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 14,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 15,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 16,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 17,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 18,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 19,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "TrimLevels",
                keyColumn: "Id",
                keyValue: 20,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(7360));

            migrationBuilder.UpdateData(
                table: "UnitOfMeasures",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(8177));

            migrationBuilder.UpdateData(
                table: "UnitOfMeasures",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(8180));

            migrationBuilder.UpdateData(
                table: "UnitOfMeasures",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(8183));

            migrationBuilder.UpdateData(
                table: "UnitOfMeasures",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(8186));

            migrationBuilder.UpdateData(
                table: "UnitOfMeasures",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(8188));

            migrationBuilder.UpdateData(
                table: "UnitOfMeasures",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(8191));

            migrationBuilder.UpdateData(
                table: "UnitOfMeasures",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(8194));

            migrationBuilder.CreateIndex(
                name: "IX_CompatibleVehicle_UniqueSpec",
                table: "CompatibleVehicles",
                columns: new[] { "ModelId", "YearStart", "YearEnd", "TrimLevelId", "TransmissionTypeId", "EngineTypeId", "BodyTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompatibleVehicles_BodyTypeId",
                table: "CompatibleVehicles",
                column: "BodyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompatibleVehicles_EngineTypeId",
                table: "CompatibleVehicles",
                column: "EngineTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompatibleVehicles_TransmissionTypeId",
                table: "CompatibleVehicles",
                column: "TransmissionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompatibleVehicles_TrimLevelId",
                table: "CompatibleVehicles",
                column: "TrimLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_CompatibleVehicles_VIN",
                table: "CompatibleVehicles",
                column: "VIN",
                unique: true,
                filter: "[VIN] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PartCompatibilities_CompatibleVehicleId",
                table: "PartCompatibilities",
                column: "CompatibleVehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_PartCompatibilities_PartId_CompatibleVehicleId",
                table: "PartCompatibilities",
                columns: new[] { "PartId", "CompatibleVehicleId" },
                unique: true);
        }
    }
}
