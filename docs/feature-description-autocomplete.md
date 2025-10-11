# Feature: Transaction Description Autocomplete

## Status
🔴 **Planning** | Branch: `feature/description-autocomplete`

## Overview
Add autocomplete functionality to transaction description fields, allowing users to quickly select from previously used descriptions. This improves data consistency and user efficiency when entering recurring expenses like "Amazon", "Grocery Store", "Gas", etc.

## Business Value
- **User Efficiency**: Faster data entry by selecting from existing descriptions
- **Data Consistency**: Reduces typos and variations (e.g., "Amazon.com", "amazon", "Amazon")
- **Better Analytics**: Consistent descriptions enable better spending pattern analysis
- **UX Enhancement**: Modern, expected behavior in financial applications

## Scope

### In Scope
- Autocomplete for adhoc transaction descriptions (both income and expenses)
- Autocomplete for recurring schedule descriptions
- Real-time search as user types (debounced)
- Distinct descriptions API endpoint
- Case-insensitive matching
- FluentUI autocomplete component integration

### Out of Scope (Future Iterations)
- Category suggestions based on description
- Learning/AI-based suggestions
- Description aliases or mappings
- Multi-language support
- Description templates

## Technical Architecture

### Layer Breakdown

#### 1. Domain Layer (`BudgetExperiment.Domain`)
**Changes**: None required (no new domain logic)
- Existing `Description` property on `AdhocTransaction` and `RecurringSchedule` is sufficient
- Read repositories already provide data access patterns

#### 2. Application Layer (`BudgetExperiment.Application`)
**New Components**:

```
BudgetExperiment.Application/
├── AdhocTransactions/
│   ├── GetDistinctDescriptionsQuery.cs       (NEW - query handler)
│   └── IAdhocTransactionService.cs            (UPDATE - add method)
└── RecurringSchedules/
    ├── GetDistinctDescriptionsQuery.cs       (NEW - query handler)
    └── IRecurringScheduleService.cs           (UPDATE - add method)
```

**Service Method Signatures**:
```csharp
// IAdhocTransactionService
Task<IReadOnlyList<string>> GetDistinctDescriptionsAsync(
    string? searchTerm = null, 
    int maxResults = 10, 
    CancellationToken cancellationToken = default);

// IRecurringScheduleService
Task<IReadOnlyList<string>> GetDistinctDescriptionsAsync(
    string? searchTerm = null, 
    int maxResults = 10, 
    CancellationToken cancellationToken = default);
```

#### 3. Infrastructure Layer (`BudgetExperiment.Infrastructure`)
**Repository Changes**:

```
BudgetExperiment.Infrastructure/
└── Repositories/
    ├── AdhocTransactionRepository.cs         (UPDATE - add method)
    └── RecurringScheduleRepository.cs        (UPDATE - add method)
```

**Repository Method Implementation**:
```csharp
// In read repository
public async Task<IReadOnlyList<string>> GetDistinctDescriptionsAsync(
    string? searchTerm = null, 
    int maxResults = 10, 
    CancellationToken cancellationToken = default)
{
    var query = _context.AdhocTransactions
        .Select(x => x.Description)
        .Distinct();
    
    if (!string.IsNullOrWhiteSpace(searchTerm))
    {
        query = query.Where(d => EF.Functions.ILike(d, $"{searchTerm}%"));
    }
    
    return await query
        .OrderBy(d => d)
        .Take(maxResults)
        .ToListAsync(cancellationToken);
}
```

**Database Considerations**:
- No schema changes required
- Consider index on `Description` column for performance (add migration if needed)
- Case-insensitive search using PostgreSQL `ILIKE`

#### 4. API Layer (`BudgetExperiment.Api`)
**New Endpoints**:

```
BudgetExperiment.Api/
└── Controllers/
    ├── AdhocTransactionsController.cs        (UPDATE - add endpoint)
    └── RecurringSchedulesController.cs       (UPDATE - add endpoint)
```

**REST API Design**:

```http
GET /api/v1/adhoc-transactions/descriptions?search={term}&maxResults={n}
GET /api/v1/recurring-schedules/descriptions?search={term}&maxResults={n}
```

**Response Format**:
```json
{
  "descriptions": [
    "Amazon",
    "Amazon Prime",
    "Amazon Web Services"
  ],
  "count": 3
}
```

**Endpoint Specifications**:
- **Method**: GET
- **Query Parameters**:
  - `search` (optional): Filter term for prefix matching
  - `maxResults` (optional, default: 10, max: 50): Limit result count
- **Status Codes**:
  - 200 OK: Success with array of descriptions
  - 400 Bad Request: Invalid query parameters
- **Caching**: Consider `Cache-Control` headers (short TTL, 60s)
- **Rate Limiting**: Standard API limits apply

#### 5. Client Layer (`BudgetExperiment.Client`)
**Component Changes**:

```
BudgetExperiment.Client/
├── Components/
│   ├── FinancialItemDialog.razor             (UPDATE - replace TextField with Autocomplete)
│   └── UnifiedScheduleDialog.razor           (UPDATE - replace TextField with Autocomplete)
├── Services/
│   └── DescriptionAutocompleteService.cs     (NEW - API client service)
└── Api/
    └── DescriptionSuggestion.cs              (NEW - client-side model)
```

**UI/UX Specifications**:
- Replace `<FluentTextField>` with `<FluentAutocomplete>` for description field
- Debounce input by 300ms before API call
- Show loading indicator while fetching
- Display "No suggestions found" when empty
- Allow free-text entry (autocomplete is assistive, not restrictive)
- Keyboard navigation support (arrow keys, Enter to select)
- Minimum 2 characters before triggering search
- Clear suggestions when field is cleared

**FluentUI Autocomplete Integration**:
```razor
<FluentAutocomplete TOption="string"
                    AutoComplete="off"
                    Label="@NameLabel"
                    Placeholder="@NamePlaceholder"
                    @bind-SelectedOptions="@selectedDescriptions"
                    OnOptionsSearch="@OnSearchDescriptions"
                    MaximumSelectedOptions="1"
                    Style="width: 100%;" />
```

**Service Pattern** (with debouncing):
```csharp
public class DescriptionAutocompleteService
{
    private readonly HttpClient _httpClient;
    private Timer? _debounceTimer;
    
    public async Task<List<string>> GetSuggestionsAsync(
        string endpoint, 
        string searchTerm, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2)
            return new List<string>();
            
        var response = await _httpClient.GetFromJsonAsync<DescriptionsResponse>(
            $"{endpoint}?search={Uri.EscapeDataString(searchTerm)}&maxResults=10",
            cancellationToken);
            
        return response?.Descriptions ?? new List<string>();
    }
}
```

## Implementation Phases

### Phase 1: Backend Foundation (TDD) ✅
**Goal**: API endpoints return distinct descriptions

1. **Domain/Infrastructure** (No changes)
   - Review existing models
   - Verify repository patterns

2. **Application Layer** (Test-First)
   - [ ] Write unit test: Service returns distinct descriptions
   - [ ] Write unit test: Service filters by search term
   - [ ] Write unit test: Service limits results
   - [ ] Write unit test: Service handles empty search
   - [ ] Implement `GetDistinctDescriptionsAsync` in `AdhocTransactionService`
   - [ ] Implement `GetDistinctDescriptionsAsync` in `RecurringScheduleService`

3. **Infrastructure Layer** (Test-First)
   - [ ] Write integration test: Repository queries distinct descriptions
   - [ ] Write integration test: Repository filters with ILIKE (case-insensitive)
   - [ ] Write integration test: Repository respects maxResults limit
   - [ ] Implement repository methods
   - [ ] Add database index on `Description` column (migration)

4. **API Layer** (Test-First)
   - [ ] Write API test: GET /adhoc-transactions/descriptions returns 200
   - [ ] Write API test: Search parameter filters results
   - [ ] Write API test: MaxResults parameter limits output
   - [ ] Write API test: Invalid parameters return 400
   - [ ] Implement controller endpoints
   - [ ] Validate OpenAPI documentation
   - [ ] Test with Scalar UI

**Acceptance Criteria**:
- [ ] `GET /api/v1/adhoc-transactions/descriptions` returns distinct descriptions
- [ ] `GET /api/v1/recurring-schedules/descriptions` returns distinct descriptions
- [ ] Endpoints respect query parameters (`search`, `maxResults`)
- [ ] All unit tests pass (100% coverage for new service methods)
- [ ] All integration tests pass (repository + API)
- [ ] OpenAPI spec accurately describes endpoints
- [ ] Scalar UI displays endpoints correctly

### Phase 2: Frontend Integration 🔴
**Goal**: Working autocomplete in UI

1. **Service Layer**
   - [ ] Create `DescriptionAutocompleteService.cs`
   - [ ] Implement debounced search logic
   - [ ] Add service to DI container

2. **Component Updates**
   - [ ] Update `FinancialItemDialog.razor` with FluentAutocomplete
   - [ ] Update `UnifiedScheduleDialog.razor` with FluentAutocomplete
   - [ ] Add loading state indicators
   - [ ] Implement error handling (network failures)

3. **Testing**
   - [ ] Manual test: Type "Ama" → see "Amazon" suggestions
   - [ ] Manual test: Select suggestion → field populates
   - [ ] Manual test: Enter custom text → still works
   - [ ] Manual test: Clear field → suggestions reset
   - [ ] Manual test: Network failure → graceful degradation
   - [ ] bUnit test: Component renders autocomplete correctly (optional)

**Acceptance Criteria**:
- [ ] Description field shows autocomplete dropdown
- [ ] Typing triggers search after 300ms debounce
- [ ] Minimum 2 characters required to search
- [ ] Users can select from suggestions or type freely
- [ ] Loading indicator shows during API call
- [ ] Network errors don't break UI
- [ ] Existing functionality remains intact (all CRUD operations work)

### Phase 3: Polish & Performance 🔴
**Goal**: Production-ready feature

1. **Performance Optimization**
   - [ ] Verify database index improves query performance
   - [ ] Add response caching (60s TTL) if beneficial
   - [ ] Test with large datasets (1000+ distinct descriptions)

2. **Edge Cases**
   - [ ] Test: Empty database (no suggestions)
   - [ ] Test: Special characters in descriptions
   - [ ] Test: Very long descriptions (truncation)
   - [ ] Test: Unicode characters (emoji, non-English)

3. **Documentation**
   - [ ] Update API documentation (if separate from OpenAPI)
   - [ ] Add comments for future maintainers
   - [ ] Document decision: Why prefix matching vs full-text search

**Acceptance Criteria**:
- [ ] API response time < 200ms (P95)
- [ ] No N+1 query issues
- [ ] All edge cases handled gracefully
- [ ] Code review complete
- [ ] Documentation updated

## REST API Detailed Specification

### Endpoint: Get Distinct Descriptions

#### Adhoc Transactions
```http
GET /api/v1/adhoc-transactions/descriptions
```

#### Recurring Schedules
```http
GET /api/v1/recurring-schedules/descriptions
```

#### Query Parameters
| Parameter | Type | Required | Default | Max | Description |
|-----------|------|----------|---------|-----|-------------|
| `search` | string | No | null | - | Case-insensitive prefix filter |
| `maxResults` | int | No | 10 | 50 | Maximum results to return |

#### Response Format
```json
{
  "descriptions": ["string"],
  "count": 0
}
```

#### Status Codes
- `200 OK`: Success
- `400 Bad Request`: Invalid parameters (e.g., `maxResults > 50`)
- `500 Internal Server Error`: Unexpected server error

#### Example Requests

**No filter (top 10)**:
```http
GET /api/v1/adhoc-transactions/descriptions
```
Response:
```json
{
  "descriptions": ["Amazon", "Gas Station", "Grocery Store", "Netflix", "Rent"],
  "count": 5
}
```

**With search filter**:
```http
GET /api/v1/adhoc-transactions/descriptions?search=am
```
Response:
```json
{
  "descriptions": ["Amazon", "Amazon Prime", "Amex Payment"],
  "count": 3
}
```

**Limit results**:
```http
GET /api/v1/adhoc-transactions/descriptions?maxResults=3
```

#### Caching Strategy
- Client-side caching: Store recent searches in memory (5-minute TTL)
- Server-side: Consider adding `Cache-Control: max-age=60` header
- Invalidation: Not critical (eventual consistency acceptable)

## Testing Strategy

### Unit Tests
**Location**: `tests/BudgetExperiment.Application.Tests/`

```csharp
// Example test structure
public class AdhocTransactionServiceTests_GetDistinctDescriptions
{
    [Fact]
    public async Task ReturnsDistinctDescriptions_WhenNoSearchTerm()
    
    [Fact]
    public async Task FiltersDescriptions_WhenSearchTermProvided()
    
    [Fact]
    public async Task LimitsResults_ToMaxResultsParameter()
    
    [Fact]
    public async Task ReturnsEmpty_WhenNoMatches()
    
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task HandlesEmptySearch_Gracefully(string searchTerm)
}
```

### Integration Tests
**Location**: `tests/BudgetExperiment.Infrastructure.Tests/`

```csharp
public class AdhocTransactionRepositoryTests_GetDistinctDescriptions
{
    [Fact]
    public async Task QueriesDatabase_ReturnsDistinct()
    
    [Fact]
    public async Task UsesILike_ForCaseInsensitiveSearch()
    
    [Fact]
    public async Task RespectsMaxResultsLimit()
}
```

### API Tests
**Location**: `tests/BudgetExperiment.Api.Tests/`

```csharp
public class AdhocTransactionsControllerTests_GetDescriptions
{
    [Fact]
    public async Task GetDescriptions_Returns200_WithDescriptions()
    
    [Fact]
    public async Task GetDescriptions_Returns400_WhenInvalidMaxResults()
    
    [Fact]
    public async Task GetDescriptions_FiltersResults_WhenSearchProvided()
}
```

### Manual Test Scenarios
1. **Happy Path**: Type "Gro" → select "Grocery Store" → save
2. **Custom Entry**: Type "New Store" → no suggestions → save anyway
3. **Case Insensitive**: Type "amazon" → see "Amazon", "AMAZON Prime"
4. **Empty State**: New database → type anything → no suggestions
5. **Network Failure**: Disconnect → type → see error → reconnect → retry
6. **Performance**: 1000 descriptions in DB → search remains fast

## Database Considerations

### Index Addition (Optional but Recommended)
```sql
-- Migration: Add index on Description for faster autocomplete
CREATE INDEX CONCURRENTLY idx_adhoc_transactions_description 
ON AdhocTransactions (Description);

CREATE INDEX CONCURRENTLY idx_recurring_schedules_description 
ON RecurringSchedules (Name);
```

**Migration Class**:
```csharp
public partial class AddDescriptionIndices : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "ix_adhoc_transactions_description",
            table: "AdhocTransactions",
            column: "Description");
            
        migrationBuilder.CreateIndex(
            name: "ix_recurring_schedules_name",
            table: "RecurringSchedules",
            column: "Name");
    }
}
```

### Query Performance Targets
- Without index: ~50-100ms (small datasets < 1000 rows)
- With index: ~5-20ms (even with 10,000+ rows)
- P95 latency goal: < 200ms end-to-end (including network)

## Security Considerations

### Input Validation
- Sanitize `search` parameter (prevent SQL injection via EF parameterization)
- Limit `maxResults` to prevent resource exhaustion (max: 50)
- Rate limit endpoint (standard API limits)

### Data Exposure
- Only return descriptions from user's own transactions (if multi-tenant)
- No sensitive data in descriptions (enforce at input time, separate concern)

## Future Enhancements (Not in Scope)

### Phase 4 (Future): Smart Suggestions
- Suggest category based on description history
- Track frequency and suggest most common descriptions first
- Learn from user patterns (machine learning)

### Phase 5 (Future): Advanced Features
- Description aliases ("Amazon" = "Amazon.com")
- Auto-categorization based on description
- Merchant normalization (external service integration)
- Multi-language support

## Success Metrics

### User Experience
- **Adoption Rate**: % of transactions using autocomplete vs manual entry
- **Time Savings**: Average time to enter description (before/after)
- **Error Reduction**: % reduction in duplicate descriptions with typos

### Technical
- **API Response Time**: P95 < 200ms
- **Test Coverage**: 100% for new service methods
- **Zero Regressions**: All existing tests pass

## Risks & Mitigations

| Risk | Impact | Likelihood | Mitigation |
|------|--------|------------|------------|
| Performance degradation with large datasets | High | Medium | Add database index, implement pagination |
| FluentUI autocomplete component issues | Medium | Low | Use native HTML5 datalist as fallback |
| User confusion (expects full-text search) | Low | Medium | Clear placeholder text, consider upgrading to full-text later |
| Increased API load from typing | Medium | Medium | Implement debouncing (300ms), client-side caching |

## Definition of Done

- [ ] All Phase 1 acceptance criteria met
- [ ] All Phase 2 acceptance criteria met
- [ ] All Phase 3 acceptance criteria met
- [ ] Unit tests pass (>= 90% coverage on new code)
- [ ] Integration tests pass
- [ ] API tests pass
- [ ] Manual testing complete (all scenarios)
- [ ] Code review approved
- [ ] OpenAPI documentation accurate
- [ ] No performance regressions
- [ ] Feature works in production-like environment
- [ ] Documentation updated (this file + inline comments)

## References

### Related Files
- Domain: `src/BudgetExperiment.Domain/AdhocTransaction.cs`
- Domain: `src/BudgetExperiment.Domain/RecurringSchedule.cs`
- Application: `src/BudgetExperiment.Application/AdhocTransactions/`
- Infrastructure: `src/BudgetExperiment.Infrastructure/Repositories/`
- API: `src/BudgetExperiment.Api/Controllers/AdhocTransactionsController.cs`
- Client: `src/BudgetExperiment.Client/Components/FinancialItemDialog.razor`

### External Documentation
- FluentUI Blazor Autocomplete: https://www.fluentui-blazor.net/Autocomplete
- PostgreSQL ILIKE: https://www.postgresql.org/docs/current/functions-matching.html
- REST API Best Practices: https://restfulapi.net/

---

**Document History**:
- 2025-10-10: Initial draft (Planning phase)
- Last Updated: 2025-10-10
- Author: AI Assistant / Developer Team
