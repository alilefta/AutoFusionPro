using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoFusionPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangestoModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "VIN",
                table: "Vehicles",
                type: "TEXT",
                maxLength: 17,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 17);

            migrationBuilder.AlterColumn<string>(
                name: "Transmission",
                table: "Vehicles",
                type: "TEXT",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "BodyType",
                table: "Vehicles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrimLevel",
                table: "Vehicles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PreferredLanguage",
                table: "Users",
                type: "INTEGER",
                nullable: true,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 10,
                oldNullable: true,
                oldDefaultValue: "en");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Parts",
                type: "TEXT",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Manufacturer",
                table: "Parts",
                type: "TEXT",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "Parts",
                type: "TEXT",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Parts",
                type: "TEXT",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "Parts",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodyType",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "TrimLevel",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "Parts");

            migrationBuilder.AlterColumn<string>(
                name: "VIN",
                table: "Vehicles",
                type: "TEXT",
                maxLength: 17,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 17,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Transmission",
                table: "Vehicles",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PreferredLanguage",
                table: "Users",
                type: "TEXT",
                maxLength: 10,
                nullable: true,
                defaultValue: "en",
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true,
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Parts",
                type: "TEXT",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Manufacturer",
                table: "Parts",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "Parts",
                type: "TEXT",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Parts",
                type: "TEXT",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
