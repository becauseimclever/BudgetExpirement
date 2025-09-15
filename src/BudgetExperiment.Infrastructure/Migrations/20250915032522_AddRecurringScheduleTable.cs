using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetExperiment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRecurringScheduleTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecurringSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Anchor = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Recurrence = table.Column<int>(type: "integer", nullable: false),
                    AmountCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    AmountValue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ScheduleType = table.Column<int>(type: "integer", nullable: false),
                    DaysInterval = table.Column<int>(type: "integer", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringSchedules", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecurringSchedules_Anchor",
                table: "RecurringSchedules",
                column: "Anchor");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringSchedules_ScheduleType",
                table: "RecurringSchedules",
                column: "ScheduleType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecurringSchedules");
        }
    }
}
