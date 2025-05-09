using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoFusionPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class VehicleCompatibilityModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartCompatibilities_Vehicles_VehicleId",
                table: "PartCompatibilities");

            migrationBuilder.DropIndex(
                name: "IX_PartCompatibilities_PartId_VehicleId",
                table: "PartCompatibilities");

            migrationBuilder.AlterColumn<int>(
                name: "VehicleId",
                table: "PartCompatibilities",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "PartCompatibilities",
                type: "TEXT",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<int>(
                name: "CompatibleVehicleId",
                table: "PartCompatibilities",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BodyTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodyTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EngineTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EngineTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Makes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Makes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransmissionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransmissionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Models",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    MakeId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Models", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Models_Makes_MakeId",
                        column: x => x.MakeId,
                        principalTable: "Makes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrimLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ModelId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrimLevels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrimLevels_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompatibleVehicles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ModelId = table.Column<int>(type: "INTEGER", nullable: false),
                    YearStart = table.Column<int>(type: "INTEGER", nullable: false),
                    YearEnd = table.Column<int>(type: "INTEGER", nullable: false),
                    TrimLevelId = table.Column<int>(type: "INTEGER", nullable: true),
                    TransmissionTypeId = table.Column<int>(type: "INTEGER", nullable: true),
                    EngineTypeId = table.Column<int>(type: "INTEGER", nullable: true),
                    BodyTypeId = table.Column<int>(type: "INTEGER", nullable: true),
                    VIN = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    ModifiedByUserId = table.Column<int>(type: "INTEGER", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_PartCompatibilities_CompatibleVehicleId",
                table: "PartCompatibilities",
                column: "CompatibleVehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_PartCompatibilities_PartId_CompatibleVehicleId",
                table: "PartCompatibilities",
                columns: new[] { "PartId", "CompatibleVehicleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BodyTypes_Name",
                table: "BodyTypes",
                column: "Name",
                unique: true);

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
                name: "IX_EngineTypes_Code",
                table: "EngineTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EngineTypes_Name",
                table: "EngineTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Makes_Name",
                table: "Makes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Models_MakeId_Name",
                table: "Models",
                columns: new[] { "MakeId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransmissionTypes_Name",
                table: "TransmissionTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrimLevels_ModelId_Name",
                table: "TrimLevels",
                columns: new[] { "ModelId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PartCompatibilities_CompatibleVehicles_CompatibleVehicleId",
                table: "PartCompatibilities",
                column: "CompatibleVehicleId",
                principalTable: "CompatibleVehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PartCompatibilities_Vehicles_VehicleId",
                table: "PartCompatibilities",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartCompatibilities_CompatibleVehicles_CompatibleVehicleId",
                table: "PartCompatibilities");

            migrationBuilder.DropForeignKey(
                name: "FK_PartCompatibilities_Vehicles_VehicleId",
                table: "PartCompatibilities");

            migrationBuilder.DropTable(
                name: "CompatibleVehicles");

            migrationBuilder.DropTable(
                name: "BodyTypes");

            migrationBuilder.DropTable(
                name: "EngineTypes");

            migrationBuilder.DropTable(
                name: "TransmissionTypes");

            migrationBuilder.DropTable(
                name: "TrimLevels");

            migrationBuilder.DropTable(
                name: "Models");

            migrationBuilder.DropTable(
                name: "Makes");

            migrationBuilder.DropIndex(
                name: "IX_PartCompatibilities_CompatibleVehicleId",
                table: "PartCompatibilities");

            migrationBuilder.DropIndex(
                name: "IX_PartCompatibilities_PartId_CompatibleVehicleId",
                table: "PartCompatibilities");

            migrationBuilder.DropColumn(
                name: "CompatibleVehicleId",
                table: "PartCompatibilities");

            migrationBuilder.AlterColumn<int>(
                name: "VehicleId",
                table: "PartCompatibilities",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "PartCompatibilities",
                type: "TEXT",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PartCompatibilities_PartId_VehicleId",
                table: "PartCompatibilities",
                columns: new[] { "PartId", "VehicleId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PartCompatibilities_Vehicles_VehicleId",
                table: "PartCompatibilities",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
