using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Appetito.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Items_HouseholdId",
                table: "Items");

            migrationBuilder.CreateIndex(
                name: "IX_Items_HouseholdId",
                table: "Items",
                column: "HouseholdId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Items_HouseholdId",
                table: "Items");

            migrationBuilder.CreateIndex(
                name: "IX_Items_HouseholdId",
                table: "Items",
                column: "HouseholdId",
                unique: true);
        }
    }
}
