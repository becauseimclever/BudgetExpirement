namespace BudgetExperiment.Application.Tests.PaySchedules;

using BudgetExperiment.Application.PaySchedules;
using BudgetExperiment.Domain;

/// <summary>
/// Tests for <see cref="PayScheduleService"/> application service.
/// </summary>
public sealed class PayScheduleServiceTests
{
    /// <summary>Verifies weekly creation persists schedule and returns id.</summary>
    /// <returns>A task representing the asynchronous test execution.</returns>
    [Fact]
    public async Task CreateWeeklyAsync_Persists_And_Returns_Id()
    {
        // Arrange
        var store = new MockStore<PaySchedule>();
        var write = new WriteRepo(store);
        var read = new ReadRepo(store);
        var uow = new Uow();
        var svc = new PayScheduleService(write, read, uow);
        var anchor = new DateOnly(2025, 1, 3);

        // Act
        var id = await svc.CreateWeeklyAsync(anchor, MoneyValue.Create("USD", 1000m));

        // Assert
        Assert.NotEqual(Guid.Empty, id);
        Assert.True(store.Items.ContainsKey(id));
        Assert.True(uow.Saved);
        Assert.Equal(anchor, store.Items[id].Anchor);
        Assert.Equal(PaySchedule.RecurrenceKind.Weekly, store.Items[id].Recurrence);
    }

    /// <summary>Verifies monthly creation persists schedule and returns id.</summary>
    /// <returns>A task representing the asynchronous test execution.</returns>
    [Fact]
    public async Task CreateMonthlyAsync_Persists_And_Returns_Id()
    {
        var store = new MockStore<PaySchedule>();
        var svc = new PayScheduleService(new WriteRepo(store), new ReadRepo(store), new Uow());
        var anchor = new DateOnly(2025, 2, 28);

        var id = await svc.CreateMonthlyAsync(anchor, MoneyValue.Create("USD", 1200m));

        Assert.True(store.Items.ContainsKey(id));
        Assert.Equal(PaySchedule.RecurrenceKind.Monthly, store.Items[id].Recurrence);
    }

    /// <summary>Retrieving occurrences returns expected dates.</summary>
    /// <returns>A task representing the asynchronous test execution.</returns>
    [Fact]
    public async Task GetOccurrencesAsync_Returns_Expected_Dates()
    {
        // Arrange
        var store = new MockStore<PaySchedule>();
        var schedule = PaySchedule.CreateWeekly(new DateOnly(2025, 1, 3), MoneyValue.Create("USD", 1000m));
        store.Items.Add(schedule.Id, schedule);
        var svc = new PayScheduleService(new WriteRepo(store), new ReadRepo(store), new Uow());

        // Act
        var result = await svc.GetOccurrencesAsync(schedule.Id, new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 17));

        // Assert
        var list = result.ToList();
        var expected = new[]
        {
            new DateOnly(2025, 1, 3),
            new DateOnly(2025, 1, 10),
            new DateOnly(2025, 1, 17),
        };
        Assert.Equal(expected, list);
    }

    /// <summary>Missing schedule id throws domain exception.</summary>
    /// <returns>A task representing the asynchronous test execution.</returns>
    [Fact]
    public async Task GetOccurrencesAsync_Unknown_Id_Throws()
    {
        var svc = new PayScheduleService(new WriteRepo(new MockStore<PaySchedule>()), new ReadRepo(new MockStore<PaySchedule>()), new Uow());
        var ex = await Assert.ThrowsAsync<DomainException>(() => svc.GetOccurrencesAsync(Guid.NewGuid(), new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 31)));
        Assert.Contains("not found", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    // Mock test helpers --------------------------------------------------
    private sealed class MockStore<T>
    {
        public Dictionary<Guid, T> Items { get; } = new();
    }

    private sealed class ReadRepo(MockStore<PaySchedule> store) : IReadRepository<PaySchedule>
    {
        public Task<PaySchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            store.Items.TryGetValue(id, out var val);
            return Task.FromResult(val);
        }

        public Task<IReadOnlyList<PaySchedule>> ListAsync(int skip, int take, CancellationToken cancellationToken = default)
        {
            var list = store.Items.Values.OrderBy(p => p.CreatedUtc).Skip(skip).Take(take).ToList();
            return Task.FromResult((IReadOnlyList<PaySchedule>)list);
        }

        public Task<long> CountAsync(CancellationToken cancellationToken = default)
            => Task.FromResult((long)store.Items.Count);
    }

    private sealed class WriteRepo(MockStore<PaySchedule> store) : IWriteRepository<PaySchedule>
    {
        public Task AddAsync(PaySchedule entity, CancellationToken cancellationToken = default)
        {
            store.Items[entity.Id] = entity;
            return Task.CompletedTask;
        }

        public Task RemoveAsync(PaySchedule entity, CancellationToken cancellationToken = default)
        {
            store.Items.Remove(entity.Id);
            return Task.CompletedTask;
        }
    }

    private sealed class Uow : IUnitOfWork
    {
        public bool Saved
        {
            get; private set;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            this.Saved = true;
            return Task.FromResult(1);
        }
    }
}
