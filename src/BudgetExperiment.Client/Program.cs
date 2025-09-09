#pragma warning disable SA1633 // File header temporarily disabled
using BudgetExperiment.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<BudgetExperiment.Client.Api.IBudgetApiClient, BudgetExperiment.Client.Api.BudgetApiClient>();

// Add FluentUI services
builder.Services.AddFluentUIComponents();

await builder.Build().RunAsync();
#pragma warning restore SA1633
