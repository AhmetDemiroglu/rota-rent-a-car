using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rent_A_Car.Migrations
{
    /// <inheritdoc />
    public partial class IletisimEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "Iletisimler",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SentDate",
                table: "Iletisimler",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "Iletisimler");

            migrationBuilder.DropColumn(
                name: "SentDate",
                table: "Iletisimler");
        }
    }
}
