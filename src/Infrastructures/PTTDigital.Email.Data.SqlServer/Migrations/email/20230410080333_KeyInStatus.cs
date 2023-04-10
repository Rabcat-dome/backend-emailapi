using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PTTDigital.Email.Data.SqlServer.Migrations.email
{
    public partial class KeyInStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EmailQueues_Status",
                schema: "dev01.email",
                table: "EmailQueues",
                column: "Status");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EmailQueues_Status",
                schema: "dev01.email",
                table: "EmailQueues");
        }
    }
}
