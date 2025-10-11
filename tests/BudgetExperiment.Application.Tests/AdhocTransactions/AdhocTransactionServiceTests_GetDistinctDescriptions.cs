using BudgetExperiment.Application.AdhocTransactions;
using BudgetExperiment.Domain;

namespace BudgetExperiment.Application.Tests.AdhocTransactions;

/// <summary>
/// Unit tests for AdhocTransactionService.GetDistinctDescriptionsAsync method.
/// </summary>
public sealed class AdhocTransactionServiceTests_GetDistinctDescriptions
{
    [Fact]
    public async Task GetDistinctDescriptionsAsync_ReturnsDistinctDescriptions_WhenNoSearchTerm()
    {
        // Arrange
        var descriptions = new List<string> { "Amazon", "Grocery Store", "Gas Station", "Netflix" };
        var mockReadRepo = new FakeAdhocTransactionReadRepository
        {
            DistinctDescriptions = descriptions
        };
        var mockWriteRepo = new FakeAdhocTransactionWriteRepository();
        var mockUnitOfWork = new FakeUnitOfWork();
        var service = new AdhocTransactionService(mockReadRepo, mockWriteRepo, mockUnitOfWork);

        // Act
        var result = await service.GetDistinctDescriptionsAsync(searchTerm: null, maxResults: 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.Count);
        Assert.Contains("Amazon", result);
        Assert.Contains("Grocery Store", result);
        Assert.Contains("Gas Station", result);
        Assert.Contains("Netflix", result);
    }

    [Fact]
    public async Task GetDistinctDescriptionsAsync_FiltersDescriptions_WhenSearchTermProvided()
    {
        // Arrange
        var allDescriptions = new List<string> { "Amazon", "Amazon Prime", "Apple Store", "Gas Station" };
        var mockReadRepo = new FakeAdhocTransactionReadRepository
        {
            DistinctDescriptions = allDescriptions,
            FilteredDescriptions = new List<string> { "Amazon", "Amazon Prime" }
        };
        var mockWriteRepo = new FakeAdhocTransactionWriteRepository();
        var mockUnitOfWork = new FakeUnitOfWork();
        var service = new AdhocTransactionService(mockReadRepo, mockWriteRepo, mockUnitOfWork);

        // Act
        var result = await service.GetDistinctDescriptionsAsync(searchTerm: "Ama", maxResults: 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains("Amazon", result);
        Assert.Contains("Amazon Prime", result);
        Assert.DoesNotContain("Apple Store", result);
    }

    [Fact]
    public async Task GetDistinctDescriptionsAsync_LimitsResults_ToMaxResultsParameter()
    {
        // Arrange
        var descriptions = new List<string>
        {
            "Item1", "Item2", "Item3", "Item4", "Item5",
            "Item6", "Item7", "Item8", "Item9", "Item10", "Item11"
        };
        var mockReadRepo = new FakeAdhocTransactionReadRepository
        {
            DistinctDescriptions = descriptions.Take(5).ToList() // Repository should limit
        };
        var mockWriteRepo = new FakeAdhocTransactionWriteRepository();
        var mockUnitOfWork = new FakeUnitOfWork();
        var service = new AdhocTransactionService(mockReadRepo, mockWriteRepo, mockUnitOfWork);

        // Act
        var result = await service.GetDistinctDescriptionsAsync(searchTerm: null, maxResults: 5);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Count);
        Assert.True(mockReadRepo.GetDistinctDescriptionsAsyncCalled);
        Assert.Equal(5, mockReadRepo.LastMaxResults);
    }

    [Fact]
    public async Task GetDistinctDescriptionsAsync_ReturnsEmpty_WhenNoMatches()
    {
        // Arrange
        var mockReadRepo = new FakeAdhocTransactionReadRepository
        {
            DistinctDescriptions = new List<string>()
        };
        var mockWriteRepo = new FakeAdhocTransactionWriteRepository();
        var mockUnitOfWork = new FakeUnitOfWork();
        var service = new AdhocTransactionService(mockReadRepo, mockWriteRepo, mockUnitOfWork);

        // Act
        var result = await service.GetDistinctDescriptionsAsync(searchTerm: "NonExistent", maxResults: 10);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public async Task GetDistinctDescriptionsAsync_HandlesEmptySearch_Gracefully(string? searchTerm)
    {
        // Arrange
        var descriptions = new List<string> { "Amazon", "Netflix" };
        var mockReadRepo = new FakeAdhocTransactionReadRepository
        {
            DistinctDescriptions = descriptions
        };
        var mockWriteRepo = new FakeAdhocTransactionWriteRepository();
        var mockUnitOfWork = new FakeUnitOfWork();
        var service = new AdhocTransactionService(mockReadRepo, mockWriteRepo, mockUnitOfWork);

        // Act
        var result = await service.GetDistinctDescriptionsAsync(searchTerm: searchTerm, maxResults: 10);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetDistinctDescriptionsAsync_UsesDefaultMaxResults_WhenNotSpecified()
    {
        // Arrange
        var descriptions = new List<string> { "Test" };
        var mockReadRepo = new FakeAdhocTransactionReadRepository
        {
            DistinctDescriptions = descriptions
        };
        var mockWriteRepo = new FakeAdhocTransactionWriteRepository();
        var mockUnitOfWork = new FakeUnitOfWork();
        var service = new AdhocTransactionService(mockReadRepo, mockWriteRepo, mockUnitOfWork);

        // Act
        var result = await service.GetDistinctDescriptionsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(mockReadRepo.GetDistinctDescriptionsAsyncCalled);
        Assert.Equal(10, mockReadRepo.LastMaxResults); // Default should be 10
    }

    // Test helpers / fakes

    private sealed class FakeAdhocTransactionReadRepository : IAdhocTransactionReadRepository
    {
        public List<string> DistinctDescriptions { get; set; } = new();
        public List<string> FilteredDescriptions { get; set; } = new();
        public bool GetDistinctDescriptionsAsyncCalled { get; set; }
        public string? LastSearchTerm { get; set; }
        public int LastMaxResults { get; set; }

        public Task<IReadOnlyList<string>> GetDistinctDescriptionsAsync(string? searchTerm = null, int maxResults = 10, CancellationToken cancellationToken = default)
        {
            GetDistinctDescriptionsAsyncCalled = true;
            LastSearchTerm = searchTerm;
            LastMaxResults = maxResults;

            if (!string.IsNullOrWhiteSpace(searchTerm) && FilteredDescriptions.Any())
            {
                return Task.FromResult<IReadOnlyList<string>>(FilteredDescriptions.Take(maxResults).ToList());
            }

            return Task.FromResult<IReadOnlyList<string>>(DistinctDescriptions.Take(maxResults).ToList());
        }

        public Task<AdhocTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<AdhocTransaction?>(null);

        public Task<IReadOnlyList<AdhocTransaction>> GetByDateAsync(DateOnly date, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<AdhocTransaction>>(new List<AdhocTransaction>());

        public Task<IReadOnlyList<AdhocTransaction>> GetByDateRangeAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<AdhocTransaction>>(new List<AdhocTransaction>());

        public Task<(IReadOnlyList<AdhocTransaction> Items, int Total)> GetIncomeTransactionsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
            => Task.FromResult<(IReadOnlyList<AdhocTransaction>, int)>((new List<AdhocTransaction>(), 0));

        public Task<(IReadOnlyList<AdhocTransaction> Items, int Total)> GetExpenseTransactionsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
            => Task.FromResult<(IReadOnlyList<AdhocTransaction>, int)>((new List<AdhocTransaction>(), 0));

        public Task<IReadOnlyList<AdhocTransaction>> ListAsync(int skip, int take, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<AdhocTransaction>>(new List<AdhocTransaction>());

        public Task<int> CountAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(0);
    }

    private sealed class FakeAdhocTransactionWriteRepository : IAdhocTransactionWriteRepository
    {
        public Task AddAsync(AdhocTransaction transaction, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task UpdateAsync(AdhocTransaction transaction, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task RemoveAsync(AdhocTransaction transaction, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(1);
    }
}
