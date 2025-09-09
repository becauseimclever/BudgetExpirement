namespace BudgetExperiment.Api.Tests;

using System.Net;
using System.Net.Http.Json;

using BudgetExperiment.Api.Dtos;
using BudgetExperiment.Application.BillSchedules;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

public sealed class BillScheduleDeleteTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public BillScheduleDeleteTests(CustomWebApplicationFactory factory)
    {
        this._factory = factory;
        this._client = factory.CreateClient();
    }

    [Fact]
    public async Task DeleteBillSchedule_ExistingBill_Returns204()
    {
        // Arrange - Create a bill first
        var createRequest = new CreateMonthlyBillScheduleRequest
        {
            Name = "Test Bill",
            Currency = "USD",
            Amount = 100m,
            Anchor = new DateOnly(2025, 1, 15),
        };

        var createResponse = await this._client.PostAsJsonAsync("api/v1/billschedules/monthly", createRequest);
        createResponse.EnsureSuccessStatusCode();
        var createdBill = await createResponse.Content.ReadFromJsonAsync<BillScheduleDto>();

        // Act - Delete the bill
        var deleteResponse = await this._client.DeleteAsync($"api/v1/billschedules/{createdBill!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Verify bill is deleted
        var getResponse = await this._client.GetAsync($"api/v1/billschedules/{createdBill.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteBillSchedule_NonExistingBill_Returns404()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await this._client.DeleteAsync($"api/v1/billschedules/{nonExistingId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
