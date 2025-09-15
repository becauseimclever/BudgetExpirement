using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BudgetExperiment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MigrateExistingScheduleData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Migrate PaySchedules to RecurringSchedules as Income type (ScheduleType = 0)
            migrationBuilder.Sql(@"
                INSERT INTO ""RecurringSchedules"" 
                    (""Id"", ""Name"", ""Anchor"", ""Recurrence"", ""AmountCurrency"", ""AmountValue"", ""ScheduleType"", ""DaysInterval"", ""CreatedUtc"", ""UpdatedUtc"")
                SELECT 
                    ""Id"",
                    '',
                    ""Anchor"",
                    ""Recurrence"",
                    ""PayAmountCurrency"",
                    ""PayAmountValue"",
                    0,
                    ""DaysInterval"",
                    ""CreatedUtc"",
                    ""UpdatedUtc""
                FROM ""PaySchedules""
            ");

            // Migrate BillSchedules to RecurringSchedules as Expense type (ScheduleType = 1)
            // Convert amounts to negative values for expenses
            migrationBuilder.Sql(@"
                INSERT INTO ""RecurringSchedules"" 
                    (""Id"", ""Name"", ""Anchor"", ""Recurrence"", ""AmountCurrency"", ""AmountValue"", ""ScheduleType"", ""DaysInterval"", ""CreatedUtc"", ""UpdatedUtc"")
                SELECT 
                    ""Id"",
                    ""Name"",
                    ""Anchor"",
                    CASE 
                        WHEN ""Recurrence"" = 0 THEN 0  -- Weekly
                        WHEN ""Recurrence"" = 1 THEN 1  -- BiWeekly  
                        WHEN ""Recurrence"" = 2 THEN 2  -- Monthly
                        WHEN ""Recurrence"" = 3 THEN 3  -- Quarterly
                        WHEN ""Recurrence"" = 4 THEN 4  -- SemiAnnual
                        WHEN ""Recurrence"" = 5 THEN 5  -- Annual
                        ELSE 2 -- Default to Monthly if unknown
                    END,
                    ""AmountCurrency"",
                    -""AmountValue"",
                    1,
                    ""DaysInterval"",
                    ""CreatedUtc"",
                    ""UpdatedUtc""
                FROM ""BillSchedules""
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove all data from RecurringSchedules (this would be a destructive operation)
            // In a real scenario, you might want to be more careful about this
            migrationBuilder.Sql(@"DELETE FROM ""RecurringSchedules""");
        }
    }
}
