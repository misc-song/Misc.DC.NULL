using Microsoft.EntityFrameworkCore.Migrations;

namespace Misc.DC.Storage.Migrations
{
    public partial class alterProcess : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "comName",
                table: "processInfos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "comName",
                table: "processInfos");
        }
    }
}
