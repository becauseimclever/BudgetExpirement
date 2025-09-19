using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetExperiment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUnifiedAdhocTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdhocTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    MoneyCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    MoneyValue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TransactionType = table.Column<int>(type: "integer", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdhocTransactions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdhocTransactions_Date",
                table: "AdhocTransactions",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_AdhocTransactions_TransactionType",
                table: "AdhocTransactions",
                column: "TransactionType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdhocTransactions");
        }
    }
}
