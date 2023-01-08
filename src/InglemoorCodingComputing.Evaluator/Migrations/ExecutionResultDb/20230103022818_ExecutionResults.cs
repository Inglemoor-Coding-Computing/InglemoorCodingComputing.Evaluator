using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InglemoorCodingComputing.Evaluator.Migrations.ExecutionResultDb
{
    /// <inheritdoc />
    public partial class ExecutionResults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExecutionResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Application = table.Column<Guid>(type: "TEXT", nullable: false),
                    User = table.Column<Guid>(type: "TEXT", nullable: false),
                    Instance = table.Column<Guid>(type: "TEXT", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    Time = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExecutionResults", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExecutionResults");
        }
    }
}
