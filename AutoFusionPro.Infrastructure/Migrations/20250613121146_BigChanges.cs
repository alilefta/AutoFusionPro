using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AutoFusionPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BigChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartCompatibilities_Vehicles_VehicleId",
                table: "PartCompatibilities");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItems_Parts_PartId",
                table: "PurchaseItems");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_Make_Model_Year",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_PartCompatibilities_VehicleId",
                table: "PartCompatibilities");

            migrationBuilder.DropColumn(
                name: "Engine",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Make",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "PurchaseItems");

            migrationBuilder.DropColumn(
                name: "VehicleId",
                table: "PartCompatibilities");

            migrationBuilder.RenameColumn(
                name: "TrimLevel",
                table: "Vehicles",
                newName: "SoldDate");

            migrationBuilder.RenameColumn(
                name: "Transmission",
                table: "Vehicles",
                newName: "RegistrationCountryOrState");

            migrationBuilder.RenameColumn(
                name: "BodyType",
                table: "Vehicles",
                newName: "RegistrationExpiryDate");

            migrationBuilder.RenameColumn(
                name: "ReceivedQuantity",
                table: "PurchaseItems",
                newName: "UnitOfMeasureId");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "OrderItems",
                newName: "UnitOfMeasureId");

            migrationBuilder.AddColumn<decimal>(
                name: "AskingPrice",
                table: "Vehicles",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BodyTypeId",
                table: "Vehicles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DriveType",
                table: "Vehicles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EngineTypeId",
                table: "Vehicles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExteriorColor",
                table: "Vehicles",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeaturesList",
                table: "Vehicles",
                type: "TEXT",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FuelType",
                table: "Vehicles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GeneralNotes",
                table: "Vehicles",
                type: "TEXT",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InteriorColor",
                table: "Vehicles",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MakeId",
                table: "Vehicles",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Mileage",
                table: "Vehicles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MileageUnit",
                table: "Vehicles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModelId",
                table: "Vehicles",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfDoors",
                table: "Vehicles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfSeats",
                table: "Vehicles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PurchaseDate",
                table: "Vehicles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PurchasePrice",
                table: "Vehicles",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegistrationPlateNumber",
                table: "Vehicles",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SoldPrice",
                table: "Vehicles",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SoldToCustomerId",
                table: "Vehicles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Vehicles",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TransmissionTypeId",
                table: "Vehicles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrimLevelId",
                table: "Vehicles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitCost",
                table: "PurchaseItems",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<decimal>(
                name: "LineTotal",
                table: "PurchaseItems",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AddColumn<int>(
                name: "QuantityOrdered",
                table: "PurchaseItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastRestockDate",
                table: "Parts",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AddColumn<decimal>(
                name: "PurchaseConversionFactor",
                table: "Parts",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PurchaseUnitOfMeasureId",
                table: "Parts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SalesConversionFactor",
                table: "Parts",
                type: "decimal(18,4)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SalesUnitOfMeasureId",
                table: "Parts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StockingUnitOfMeasureId",
                table: "Parts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QuantitySold",
                table: "OrderItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Notifications",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "Notifications",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<bool>(
                name: "IsRead",
                table: "Notifications",
                type: "INTEGER",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<decimal>(
                name: "Balance",
                table: "Invoices",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComputedColumnSql: "[Total] - [AmountPaid]");

            migrationBuilder.CreateTable(
                name: "UnitOfMeasures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Symbol = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitOfMeasures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleDamageLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VehicleId = table.Column<int>(type: "INTEGER", nullable: false),
                    DateNoted = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Severity = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    IsRepaired = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    RepairedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EstimatedRepairCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ActualRepairCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RepairNotes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleDamageLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleDamageLogs_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VehicleId = table.Column<int>(type: "INTEGER", nullable: false),
                    DocumentName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DocumentType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleDocuments_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VehicleId = table.Column<int>(type: "INTEGER", nullable: false),
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
                    table.PrimaryKey("PK_VehicleImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleImages_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleServiceHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VehicleId = table.Column<int>(type: "INTEGER", nullable: false),
                    ServiceDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    MileageAtService = table.Column<int>(type: "INTEGER", nullable: true),
                    ServiceDescription = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ServiceProviderName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleServiceHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleServiceHistories_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleDamageImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VehicleDamageLogId = table.Column<int>(type: "INTEGER", nullable: false),
                    ImagePath = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Caption = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleDamageImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleDamageImages_VehicleDamageLogs_VehicleDamageLogId",
                        column: x => x.VehicleDamageLogId,
                        principalTable: "VehicleDamageLogs",
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

            migrationBuilder.InsertData(
                table: "UnitOfMeasures",
                columns: new[] { "Id", "CreatedAt", "CreatedByUserId", "Description", "ModifiedAt", "ModifiedByUserId", "Name", "Symbol" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(8177), null, null, null, null, "Piece", "pcs" },
                    { 2, new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(8180), null, null, null, null, "Liter", "L" },
                    { 3, new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(8183), null, null, null, null, "Kilogram", "kg" },
                    { 4, new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(8186), null, null, null, null, "Box", "box" },
                    { 5, new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(8188), null, null, null, null, "Meter", "m" },
                    { 6, new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(8191), null, null, null, null, "Pair", "pr" },
                    { 7, new DateTime(2025, 6, 13, 12, 11, 44, 173, DateTimeKind.Utc).AddTicks(8194), null, null, null, null, "Set", "set" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_BodyTypeId",
                table: "Vehicles",
                column: "BodyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_EngineTypeId",
                table: "Vehicles",
                column: "EngineTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_MakeId",
                table: "Vehicles",
                column: "MakeId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_ModelId",
                table: "Vehicles",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_SoldToCustomerId",
                table: "Vehicles",
                column: "SoldToCustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_TransmissionTypeId",
                table: "Vehicles",
                column: "TransmissionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_TrimLevelId",
                table: "Vehicles",
                column: "TrimLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseItems_UnitOfMeasureId",
                table: "PurchaseItems",
                column: "UnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_Parts_PurchaseUnitOfMeasureId",
                table: "Parts",
                column: "PurchaseUnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_Parts_SalesUnitOfMeasureId",
                table: "Parts",
                column: "SalesUnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_Parts_StockingUnitOfMeasureId",
                table: "Parts",
                column: "StockingUnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_UnitOfMeasureId",
                table: "OrderItems",
                column: "UnitOfMeasureId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_IsRead",
                table: "Notifications",
                column: "IsRead");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Role",
                table: "Notifications",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Timestamp",
                table: "Notifications",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Type",
                table: "Notifications",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_UnitOfMeasures_Name",
                table: "UnitOfMeasures",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnitOfMeasures_Symbol",
                table: "UnitOfMeasures",
                column: "Symbol",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleDamageImages_VehicleDamageLogId",
                table: "VehicleDamageImages",
                column: "VehicleDamageLogId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleDamageLogs_VehicleId",
                table: "VehicleDamageLogs",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleDocuments_VehicleId",
                table: "VehicleDocuments",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleImages_VehicleId",
                table: "VehicleImages",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleServiceHistories_VehicleId",
                table: "VehicleServiceHistories",
                column: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_UnitOfMeasures_UnitOfMeasureId",
                table: "OrderItems",
                column: "UnitOfMeasureId",
                principalTable: "UnitOfMeasures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Parts_UnitOfMeasures_PurchaseUnitOfMeasureId",
                table: "Parts",
                column: "PurchaseUnitOfMeasureId",
                principalTable: "UnitOfMeasures",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Parts_UnitOfMeasures_SalesUnitOfMeasureId",
                table: "Parts",
                column: "SalesUnitOfMeasureId",
                principalTable: "UnitOfMeasures",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Parts_UnitOfMeasures_StockingUnitOfMeasureId",
                table: "Parts",
                column: "StockingUnitOfMeasureId",
                principalTable: "UnitOfMeasures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItems_Parts_PartId",
                table: "PurchaseItems",
                column: "PartId",
                principalTable: "Parts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItems_UnitOfMeasures_UnitOfMeasureId",
                table: "PurchaseItems",
                column: "UnitOfMeasureId",
                principalTable: "UnitOfMeasures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_BodyTypes_BodyTypeId",
                table: "Vehicles",
                column: "BodyTypeId",
                principalTable: "BodyTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Customers_SoldToCustomerId",
                table: "Vehicles",
                column: "SoldToCustomerId",
                principalTable: "Customers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_EngineTypes_EngineTypeId",
                table: "Vehicles",
                column: "EngineTypeId",
                principalTable: "EngineTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Makes_MakeId",
                table: "Vehicles",
                column: "MakeId",
                principalTable: "Makes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Models_ModelId",
                table: "Vehicles",
                column: "ModelId",
                principalTable: "Models",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_TransmissionTypes_TransmissionTypeId",
                table: "Vehicles",
                column: "TransmissionTypeId",
                principalTable: "TransmissionTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_TrimLevels_TrimLevelId",
                table: "Vehicles",
                column: "TrimLevelId",
                principalTable: "TrimLevels",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_UnitOfMeasures_UnitOfMeasureId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Parts_UnitOfMeasures_PurchaseUnitOfMeasureId",
                table: "Parts");

            migrationBuilder.DropForeignKey(
                name: "FK_Parts_UnitOfMeasures_SalesUnitOfMeasureId",
                table: "Parts");

            migrationBuilder.DropForeignKey(
                name: "FK_Parts_UnitOfMeasures_StockingUnitOfMeasureId",
                table: "Parts");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItems_Parts_PartId",
                table: "PurchaseItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseItems_UnitOfMeasures_UnitOfMeasureId",
                table: "PurchaseItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_BodyTypes_BodyTypeId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Customers_SoldToCustomerId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_EngineTypes_EngineTypeId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Makes_MakeId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Models_ModelId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_TransmissionTypes_TransmissionTypeId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_TrimLevels_TrimLevelId",
                table: "Vehicles");

            migrationBuilder.DropTable(
                name: "UnitOfMeasures");

            migrationBuilder.DropTable(
                name: "VehicleDamageImages");

            migrationBuilder.DropTable(
                name: "VehicleDocuments");

            migrationBuilder.DropTable(
                name: "VehicleImages");

            migrationBuilder.DropTable(
                name: "VehicleServiceHistories");

            migrationBuilder.DropTable(
                name: "VehicleDamageLogs");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_BodyTypeId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_EngineTypeId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_MakeId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_ModelId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_SoldToCustomerId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_TransmissionTypeId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_TrimLevelId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseItems_UnitOfMeasureId",
                table: "PurchaseItems");

            migrationBuilder.DropIndex(
                name: "IX_Parts_PurchaseUnitOfMeasureId",
                table: "Parts");

            migrationBuilder.DropIndex(
                name: "IX_Parts_SalesUnitOfMeasureId",
                table: "Parts");

            migrationBuilder.DropIndex(
                name: "IX_Parts_StockingUnitOfMeasureId",
                table: "Parts");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_UnitOfMeasureId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_IsRead",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_Role",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_Timestamp",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_Type",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "AskingPrice",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "BodyTypeId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "DriveType",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "EngineTypeId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "ExteriorColor",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "FeaturesList",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "FuelType",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "GeneralNotes",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "InteriorColor",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "MakeId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Mileage",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "MileageUnit",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "ModelId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "NumberOfDoors",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "NumberOfSeats",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "PurchaseDate",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "PurchasePrice",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "RegistrationPlateNumber",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "SoldPrice",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "SoldToCustomerId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "TransmissionTypeId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "TrimLevelId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "QuantityOrdered",
                table: "PurchaseItems");

            migrationBuilder.DropColumn(
                name: "PurchaseConversionFactor",
                table: "Parts");

            migrationBuilder.DropColumn(
                name: "PurchaseUnitOfMeasureId",
                table: "Parts");

            migrationBuilder.DropColumn(
                name: "SalesConversionFactor",
                table: "Parts");

            migrationBuilder.DropColumn(
                name: "SalesUnitOfMeasureId",
                table: "Parts");

            migrationBuilder.DropColumn(
                name: "StockingUnitOfMeasureId",
                table: "Parts");

            migrationBuilder.DropColumn(
                name: "QuantitySold",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "SoldDate",
                table: "Vehicles",
                newName: "TrimLevel");

            migrationBuilder.RenameColumn(
                name: "RegistrationExpiryDate",
                table: "Vehicles",
                newName: "BodyType");

            migrationBuilder.RenameColumn(
                name: "RegistrationCountryOrState",
                table: "Vehicles",
                newName: "Transmission");

            migrationBuilder.RenameColumn(
                name: "UnitOfMeasureId",
                table: "PurchaseItems",
                newName: "ReceivedQuantity");

            migrationBuilder.RenameColumn(
                name: "UnitOfMeasureId",
                table: "OrderItems",
                newName: "Quantity");

            migrationBuilder.AddColumn<string>(
                name: "Engine",
                table: "Vehicles",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Make",
                table: "Vehicles",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "Vehicles",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitCost",
                table: "PurchaseItems",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "LineTotal",
                table: "PurchaseItems",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "PurchaseItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LastRestockDate",
                table: "Parts",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VehicleId",
                table: "PartCompatibilities",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Notifications",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "Notifications",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "IsRead",
                table: "Notifications",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<decimal>(
                name: "Balance",
                table: "Invoices",
                type: "decimal(18,2)",
                nullable: false,
                computedColumnSql: "[Total] - [AmountPaid]",
                oldClrType: typeof(decimal),
                oldType: "TEXT");

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
                name: "IX_Vehicles_Make_Model_Year",
                table: "Vehicles",
                columns: new[] { "Make", "Model", "Year" });

            migrationBuilder.CreateIndex(
                name: "IX_PartCompatibilities_VehicleId",
                table: "PartCompatibilities",
                column: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_PartCompatibilities_Vehicles_VehicleId",
                table: "PartCompatibilities",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseItems_Parts_PartId",
                table: "PurchaseItems",
                column: "PartId",
                principalTable: "Parts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
