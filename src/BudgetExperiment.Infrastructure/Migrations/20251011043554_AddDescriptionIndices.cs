using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetExperiment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionIndices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AdhocTransactions_Description",
                table: "AdhocTransactions",
                column: "Description");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringSchedules_Name",
                table: "RecurringSchedules",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AdhocTransactions_Description",
                table: "AdhocTransactions");

            migrationBuilder.DropIndex(
                name: "IX_RecurringSchedules_Name",
                table: "RecurringSchedules");
        }
    }
}
