using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PTTDigital.Email.Data.SqlServer.Migrations.email
{
    public partial class Initialization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dev01.email");

            migrationBuilder.CreateTable(
                name: "Messages",
                schema: "dev01.email",
                columns: table => new
                {
                    MessageId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    EmailSubject = table.Column<string>(type: "text", nullable: false),
                    EmailBody = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MessageId);
                });

            migrationBuilder.CreateTable(
                name: "EmailArchives",
                schema: "dev01.email",
                columns: table => new
                {
                    ArchiveId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    QueueId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    EmailFrom = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    EmailTo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    EmailCc = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Initiated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Sent = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsHtmlFormat = table.Column<bool>(type: "bit", nullable: false),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RefAccPolicyId = table.Column<int>(type: "int", nullable: true),
                    IsTest = table.Column<bool>(type: "bit", nullable: false),
                    MessageId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailArchives", x => x.ArchiveId);
                    table.ForeignKey(
                        name: "FK_EmailArchives_Messages_MessageId",
                        column: x => x.MessageId,
                        principalSchema: "dev01.email",
                        principalTable: "Messages",
                        principalColumn: "MessageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailQueues",
                schema: "dev01.email",
                columns: table => new
                {
                    QueueId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    EmailFrom = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    EmailTo = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    EmailCc = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Initiated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Sent = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsHtmlFormat = table.Column<bool>(type: "bit", nullable: false),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RefAccPolicyId = table.Column<int>(type: "int", nullable: true),
                    IsTest = table.Column<bool>(type: "bit", nullable: false),
                    MessageId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailQueues", x => x.QueueId);
                    table.ForeignKey(
                        name: "FK_EmailQueues_Messages_MessageId",
                        column: x => x.MessageId,
                        principalSchema: "dev01.email",
                        principalTable: "Messages",
                        principalColumn: "MessageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailArchives_MessageId",
                schema: "dev01.email",
                table: "EmailArchives",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailQueues_MessageId",
                schema: "dev01.email",
                table: "EmailQueues",
                column: "MessageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailArchives",
                schema: "dev01.email");

            migrationBuilder.DropTable(
                name: "EmailQueues",
                schema: "dev01.email");

            migrationBuilder.DropTable(
                name: "Messages",
                schema: "dev01.email");
        }
    }
}
