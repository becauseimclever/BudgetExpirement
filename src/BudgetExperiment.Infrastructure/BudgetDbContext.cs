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

    /// <summary>Gets expenses set.</summary>
    public DbSet<Expense> Expenses => this.Set<Expense>();

    /// <summary>Gets adhoc payments set.</summary>
    public DbSet<AdhocPayment> AdhocPayments => this.Set<AdhocPayment>();

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
            p.Property(x => x.DaysInterval);
            p.OwnsOne(typeof(MoneyValue), "Amount", mv =>
            {
                mv.Property("Currency").HasColumnName("PayAmountCurrency").HasMaxLength(3).IsRequired();
                mv.Property("Amount").HasColumnName("PayAmountValue").HasColumnType("numeric(18,2)").IsRequired();
            });
            p.Property(x => x.CreatedUtc).IsRequired();
            p.Property(x => x.UpdatedUtc);
        });

        modelBuilder.Entity<Expense>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Description).IsRequired().HasMaxLength(500);
            e.OwnsOne(typeof(MoneyValue), "Amount", mv =>
            {
                mv.Property("Currency").HasColumnName("AmountCurrency").HasMaxLength(3).IsRequired();
                mv.Property("Amount").HasColumnName("AmountValue").HasColumnType("numeric(18,2)").IsRequired();
            });
            e.Property(x => x.Date).HasConversion(new DateOnlyConverter()).IsRequired();
            e.Property(x => x.Category).HasMaxLength(100);
            e.Property(x => x.CreatedUtc).IsRequired();
            e.Property(x => x.UpdatedUtc);
            e.HasIndex(x => x.Date); // Index for date-based queries
        });

        modelBuilder.Entity<AdhocPayment>(a =>
        {
            a.HasKey(x => x.Id);
            a.Property(x => x.Description).IsRequired().HasMaxLength(500);
            a.OwnsOne(typeof(MoneyValue), "Money", mv =>
            {
                mv.Property("Currency").HasColumnName("MoneyCurrency").HasMaxLength(3).IsRequired();
                mv.Property("Amount").HasColumnName("MoneyValue").HasColumnType("numeric(18,2)").IsRequired();
            });
            a.Property(x => x.Date).HasConversion(new DateOnlyConverter()).IsRequired();
            a.Property(x => x.Category).HasMaxLength(100);
            a.Property(x => x.CreatedUtc).IsRequired();
            a.Property(x => x.UpdatedUtc);
            a.HasIndex(x => x.Date); // Index for date-based queries
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
                else if (entry.Entity is Expense expense)
                {
                    expense.MarkUpdated();
                }
                else if (entry.Entity is AdhocPayment adhocPayment)
                {
                    adhocPayment.MarkUpdated();
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
            : base(
                d => DateTime.SpecifyKind(d.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc),
                dt => DateOnly.FromDateTime(DateTime.SpecifyKind(dt, DateTimeKind.Utc)))
        {
        }
    }
}
