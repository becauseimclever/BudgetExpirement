namespace BudgetExperiment.Infrastructure;

using BudgetExperiment.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

/// <summary>
/// EF Core DbContext for budgeting aggregates.
/// </summary>
public sealed class BudgetDbContext : DbContext, IUnitOfWork
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BudgetDbContext"/> class.
    /// </summary>
    /// <param name="options">DbContext options.</param>
    public BudgetDbContext(DbContextOptions<BudgetDbContext> options)
        : base(options)
    {
    }

    /// <summary>Gets bill schedules set.</summary>
    public DbSet<BillSchedule> BillSchedules => this.Set<BillSchedule>();

    /// <summary>Gets pay schedules set.</summary>
    public DbSet<PaySchedule> PaySchedules => this.Set<PaySchedule>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // BillSchedule mapping (value object MoneyValue owned type pattern via conversion)
        modelBuilder.Entity<BillSchedule>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
            b.OwnsOne(typeof(MoneyValue), "Amount", mv =>
            {
                mv.Property("Currency").HasColumnName("AmountCurrency").HasMaxLength(3).IsRequired();
                mv.Property("Amount").HasColumnName("AmountValue").HasColumnType("numeric(18,2)").IsRequired();
            });
            b.Property(x => x.Anchor).HasConversion(new DateOnlyConverter()).IsRequired();
            b.Property(x => x.Recurrence).IsRequired();
        });

        modelBuilder.Entity<PaySchedule>(p =>
        {
            p.HasKey(x => x.Id);
            p.Property(x => x.Anchor).HasConversion(new DateOnlyConverter()).IsRequired();
            p.Property(x => x.Recurrence).IsRequired();
        });
    }

    /// <summary>
    /// Converter for <see cref="DateOnly"/> to <see cref="DateTime"/> storage.
    /// </summary>
    private sealed class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
    {
        public DateOnlyConverter()
            : base(d => d.ToDateTime(TimeOnly.MinValue), dt => DateOnly.FromDateTime(DateTime.SpecifyKind(dt, DateTimeKind.Utc)))
        {
        }
    }
}
