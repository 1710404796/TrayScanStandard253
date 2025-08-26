using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrayScanStandard.Migrations
{
    /// <inheritdoc />
    public partial class Initcount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChannelCount",
                table: "PalletLogs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChannelCount",
                table: "PalletLogs");
        }
    }
}
