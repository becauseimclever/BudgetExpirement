namespace BudgetExperiment.Application.Tests.BillSchedules;

using BudgetExperiment.Application.BillSchedules;
using BudgetExperiment.Domain;

/// <summary>
/// Tests for <see cref="BillScheduleService"/>.
/// </summary>
public sealed class BillScheduleServiceTests
{
    /// <summary>Creation of monthly bill schedule persists and returns id.</summary>
    /// <returns>A task representing the asynchronous test execution.</returns>
    [Fact]
    public async Task CreateMonthlyAsync_Persists_And_Returns_Id()
    {
        var store = new InMemoryStore<BillSchedule>();
        var svc = new BillScheduleService(new WriteRepo(store), new ReadRepo(store), new Uow());
        var id = await svc.CreateMonthlyAsync("Rent", MoneyValue.Create("USD", 1500m), new DateOnly(2025, 1, 5));
        Assert.NotEqual(Guid.Empty, id);
        Assert.True(store.Items.ContainsKey(id));
        Assert.Equal("Rent", store.Items[id].Name);
    }

    /// <summary>Occurrences retrieval returns expected set.</summary>
    /// <returns>A task representing the asynchronous test execution.</returns>
    [Fact]
    public async Task GetOccurrencesAsync_Returns_Expected_Dates()
    {
        var store = new InMemoryStore<BillSchedule>();
        var schedule = BillSchedule.CreateMonthly("Subscription", MoneyValue.Create("USD", 10m), new DateOnly(2025, 1, 31));
        store.Items.Add(schedule.Id, schedule);
        var svc = new BillScheduleService(new WriteRepo(store), new ReadRepo(store), new Uow());
        var result = await svc.GetOccurrencesAsync(schedule.Id, new DateOnly(2025, 1, 1), new DateOnly(2025, 3, 31));
        var list = result.ToList();
        var expected = new[]
        {
            new DateOnly(2025, 1, 31),
            new DateOnly(2025, 2, 28),
            new DateOnly(2025, 3, 31),
        };
        Assert.Equal(expected, list);
    }

    /// <summary>Unknown id throws.</summary>
    /// <returns>A task representing the asynchronous test execution.</returns>
    [Fact]
    public async Task GetOccurrencesAsync_Unknown_Id_Throws()
    {
        var svc = new BillScheduleService(new WriteRepo(new InMemoryStore<BillSchedule>()), new ReadRepo(new InMemoryStore<BillSchedule>()), new Uow());
        var ex = await Assert.ThrowsAsync<DomainException>(() => svc.GetOccurrencesAsync(Guid.NewGuid(), new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 31)));
        Assert.Contains("not found", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    // In-memory helpers ---------------------------------------------------
    private sealed class InMemoryStore<T>
    {
        public Dictionary<Guid, T> Items { get; } = new();
    }

    private sealed class ReadRepo(InMemoryStore<BillSchedule> store) : IReadRepository<BillSchedule>
    {
        public Task<BillSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            store.Items.TryGetValue(id, out var val);
            return Task.FromResult(val);
        }

        public Task<IReadOnlyList<BillSchedule>> ListAsync(int skip, int take, CancellationToken cancellationToken = default)
        {
            var list = store.Items.Values.OrderBy(b => b.CreatedUtc).Skip(skip).Take(take).ToList();
            return Task.FromResult((IReadOnlyList<BillSchedule>)list);
        }

        public Task<long> CountAsync(CancellationToken cancellationToken = default)
            => Task.FromResult((long)store.Items.Count);
    }

    private sealed class WriteRepo(InMemoryStore<BillSchedule> store) : IWriteRepository<BillSchedule>
    {
        public Task AddAsync(BillSchedule entity, CancellationToken cancellationToken = default)
        {
            store.Items[entity.Id] = entity;
            return Task.CompletedTask;
        }
    }

    private sealed class Uow : IUnitOfWork
    {
        public bool Saved { get; private set; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            this.Saved = true;
            return Task.FromResult(1);
        }
    }
}
