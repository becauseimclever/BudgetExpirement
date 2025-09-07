namespace BudgetExperiment.Application.Tests.BillSchedules;

using BudgetExperiment.Application.BillSchedules;
using BudgetExperiment.Domain;

public sealed class InMemoryBillScheduleServiceTests
{
    [Fact]
    public async Task Create_And_Retrieve_Monthly()
    {
        var harness = new Harness();
        var id = await harness.Service.CreateMonthlyAsync("Rent", MoneyValue.Create("USD", 1200m), new DateOnly(2025, 1, 5));
        var dates = (await harness.Service.GetOccurrencesAsync(id, new DateOnly(2025, 1, 1), new DateOnly(2025, 3, 31))).ToList();
        Assert.Equal(new[] { new DateOnly(2025, 1, 5), new DateOnly(2025, 2, 5), new DateOnly(2025, 3, 5) }, dates);
    }

    [Fact]
    public async Task Missing_BillSchedule_Throws()
    {
        var harness = new Harness();
        await Assert.ThrowsAsync<DomainException>(async () => await harness.Service.GetOccurrencesAsync(Guid.NewGuid(), new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 31)));
    }

    private sealed class Harness
    {
        public Harness()
        {
            this.Write = new InMemoryWrite();
            this.Read = this.Write; // same backing store
            this.Uow = new InMemoryUow();
            this.Service = new BillScheduleService(this.Write, this.Read, this.Uow);
        }

        public BillScheduleService Service { get; }

        public InMemoryWrite Write { get; }

        public InMemoryWrite Read { get; }

        public InMemoryUow Uow { get; }
    }

    private sealed class InMemoryWrite : IWriteRepository<BillSchedule>, IReadRepository<BillSchedule>
    {
        private readonly Dictionary<Guid, BillSchedule> _data = new();

        public Task AddAsync(BillSchedule entity, CancellationToken cancellationToken = default)
        {
            this._data[entity.Id] = entity;
            return Task.CompletedTask;
        }

        public Task<BillSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(this._data.TryGetValue(id, out var v) ? v : null);
    }

    private sealed class InMemoryUow : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => Task.FromResult(0);
    }
}
