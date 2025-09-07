namespace BudgetExperiment.Application.Tests.PaySchedules;

using BudgetExperiment.Application.PaySchedules;
using BudgetExperiment.Domain;

public sealed class InMemoryPayScheduleServiceTests
{
    [Fact]
    public async Task Create_And_Retrieve_Weekly()
    {
    var harness = new Harness();
    var anchor = new DateOnly(2025, 1, 3); // Friday
    var id = await harness.Service.CreateWeeklyAsync(anchor, MoneyValue.Create("USD", 1000m));
    var occurrences = (await harness.Service.GetOccurrencesAsync(id, new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 31))).ToList();
    Assert.Contains(anchor, occurrences);
    Assert.All(occurrences, d => Assert.Equal(DayOfWeek.Friday, d.DayOfWeek));
    }

    [Fact]
    public async Task Missing_Schedule_Throws()
    {
        var harness = new Harness();
        await Assert.ThrowsAsync<DomainException>(async () => await harness.Service.GetOccurrencesAsync(Guid.NewGuid(), new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 2)));
    }

    private sealed class Harness
    {
        public Harness()
        {
            this.Write = new InMemoryWrite();
            this.Read = this.Write;
            this.Uow = new InMemoryUow();
            this.Service = new PayScheduleService(this.Write, this.Read, this.Uow);
        }

        public PayScheduleService Service { get; }

        public InMemoryWrite Write { get; }

        public InMemoryWrite Read { get; }

        public InMemoryUow Uow { get; }
    }

    private sealed class InMemoryWrite : IWriteRepository<PaySchedule>, IReadRepository<PaySchedule>
    {
        private readonly Dictionary<Guid, PaySchedule> _data = new();

        public Task AddAsync(PaySchedule entity, CancellationToken cancellationToken = default)
        {
            this._data[entity.Id] = entity;
            return Task.CompletedTask;
        }

        public Task<PaySchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(this._data.TryGetValue(id, out var v) ? v : null);

        public Task<IReadOnlyList<PaySchedule>> ListAsync(int skip, int take, CancellationToken cancellationToken = default)
        {
            var list = this._data.Values.OrderBy(p => p.CreatedUtc).Skip(skip).Take(take).ToList();
            return Task.FromResult((IReadOnlyList<PaySchedule>)list);
        }

        public Task<long> CountAsync(CancellationToken cancellationToken = default)
            => Task.FromResult((long)this._data.Count);
    }

    private sealed class InMemoryUow : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(0);
    }
}
