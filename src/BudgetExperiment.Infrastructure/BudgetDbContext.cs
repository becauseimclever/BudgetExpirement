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
    public override int SaveChanges()
    {
        this.ApplyTimestamps();
        return base.SaveChanges();
    }

    /// <inheritdoc />
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        this.ApplyTimestamps();
        return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

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
            b.Property(x => x.CreatedUtc).IsRequired();
            b.Property(x => x.UpdatedUtc);
        });

        modelBuilder.Entity<PaySchedule>(p =>
        {
            p.HasKey(x => x.Id);
            p.Property(x => x.Anchor).HasConversion(new DateOnlyConverter()).IsRequired();
            p.Property(x => x.Recurrence).IsRequired();
            p.Property(x => x.CreatedUtc).IsRequired();
            p.Property(x => x.UpdatedUtc);
        });
    }

    private void ApplyTimestamps()
    {
        foreach (var entry in this.ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Modified)
            {
                if (entry.Entity is BillSchedule bs)
                {
                    bs.MarkUpdated();
                }
                else if (entry.Entity is PaySchedule ps)
                {
                    ps.MarkUpdated();
                }
            }
        }
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
