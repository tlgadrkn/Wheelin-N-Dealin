using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuctionService.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusEnumToAuction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
        // Drop the existing Status column
        migrationBuilder.DropColumn(
            name: "Status",
            table: "Auctions");

        // Add the new Status column with the enum type
        migrationBuilder.AddColumn<int>(
            name: "Status",
            table: "Auctions",
            type: "integer",
            nullable: false,
            defaultValue: 0);
    
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
             // Drop the new Status column
        migrationBuilder.DropColumn(
            name: "Status",
            table: "Auctions");

        // Add the old Status column back
        migrationBuilder.AddColumn<string>(
            name: "Status",
            table: "Auctions",
            type: "text",
            nullable: true);
        }
    }
}
