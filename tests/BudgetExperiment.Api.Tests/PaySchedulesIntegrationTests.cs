namespace BudgetExperiment.Api.Tests;

using System.Net;
using System.Net.Http.Json;

using BudgetExperiment.Api.Dtos;
using BudgetExperiment.Application.PaySchedules;

using Microsoft.AspNetCore.Mvc.Testing;

using Shouldly;

public sealed class PaySchedulesIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PaySchedulesIntegrationTests(CustomWebApplicationFactory factory)
    {
        this._client = factory.CreateApiClient();
    }

    [Fact]
    public async Task CreateWeekly_Then_Get_Should_Return_CreatedEntity()
    {
        var anchor = new DateOnly(2025, 1, 3);
        var create = new CreateWeeklyPayScheduleRequest { Anchor = anchor };
        var response = await this._client.PostAsJsonAsync("/api/v1/payschedules/weekly", create);
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        response.Headers.Location.ShouldNotBeNull();
        var location = response.Headers.Location!.ToString();
        location.ShouldContain("/api/v1/payschedules/");

        var getResponse = await this._client.GetAsync(location);
        getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var dto = await getResponse.Content.ReadFromJsonAsync<PayScheduleDto>();
        dto.ShouldNotBeNull();
        dto!.Anchor.ShouldBe(anchor);
        dto.CreatedUtc.ShouldNotBe(default);
        dto.UpdatedUtc.ShouldNotBe(default);
    }

    [Fact]
    public async Task Occurrences_For_Weekly_Should_Return_Expected_Sequence()
    {
        var anchor = new DateOnly(2025, 1, 3); // Friday
        var create = new CreateWeeklyPayScheduleRequest { Anchor = anchor };
        var createResp = await this._client.PostAsJsonAsync("/api/v1/payschedules/weekly", create);
        createResp.EnsureSuccessStatusCode();
        var location = createResp.Headers.Location!.ToString();

        var id = Guid.Parse(location.Split('/').Last());
        var start = anchor;
        var end = anchor.AddDays(21);
        var occResp = await this._client.GetAsync($"/api/v1/payschedules/{id}/occurrences?start={start:O}&end={end:O}");
        occResp.StatusCode.ShouldBe(HttpStatusCode.OK);
        var occurrences = await occResp.Content.ReadFromJsonAsync<DateOnly[]>();
        occurrences.ShouldNotBeNull();
        occurrences!.Length.ShouldBe(4);
        occurrences[0].ShouldBe(anchor);
        occurrences[1].ShouldBe(anchor.AddDays(7));
        occurrences[2].ShouldBe(anchor.AddDays(14));
        occurrences[3].ShouldBe(anchor.AddDays(21));
    }
}
