using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuctionService.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAuctionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "imageUrl",
                table: "Items",
                newName: "ImageUrl");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Auctions",
                type: "text",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Items",
                newName: "imageUrl");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Status",
                table: "Auctions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
