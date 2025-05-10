var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Add middleware to serve BlazorWasm static files
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

// Map fallback to BlazorWasm
app.MapFallbackToFile("index.html");

app.Run();
