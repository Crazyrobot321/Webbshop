using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Webbshop.Migrations
{
    /// <inheritdoc />
    public partial class AddedUniqueEmailConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Customers_MobileNr",
                table: "Customers",
                column: "MobileNr",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Customers_MobileNr",
                table: "Customers");
        }
    }
}
