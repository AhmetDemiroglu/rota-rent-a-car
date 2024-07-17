using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rent_A_Car.Migrations
{
    /// <inheritdoc />
    public partial class IsCompleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Rezervasyonlar",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Rezervasyonlar");
        }
    }
}
