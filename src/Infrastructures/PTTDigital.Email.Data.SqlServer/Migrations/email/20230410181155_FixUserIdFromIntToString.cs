using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PTTDigital.Email.Data.SqlServer.Migrations.email
{
    public partial class FixUserIdFromIntToString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RefAccPolicyId",
                schema: "dev01.email",
                table: "EmailQueues",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RefAccPolicyId",
                schema: "dev01.email",
                table: "EmailArchives",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RefAccPolicyId",
                schema: "dev01.email",
                table: "EmailQueues",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RefAccPolicyId",
                schema: "dev01.email",
                table: "EmailArchives",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
