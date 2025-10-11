namespace BudgetExperiment.Api.Dtos;

/// <summary>
/// Response DTO for description autocomplete endpoints.
/// </summary>
/// <param name="Descriptions">List of distinct descriptions.</param>
/// <param name="Count">Number of descriptions returned.</param>
public sealed record DescriptionsResponse(
    List<string> Descriptions,
    int Count);
