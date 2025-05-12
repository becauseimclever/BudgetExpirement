using Asp.Versioning;
using Budget.Abstractions.Services;
using Budget.API.Transformers;
using Budget.Data;
using Budget.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace Budget.API
{
	/// <summary>
	/// The main entry point for the Budget API application.
	/// </summary>
	public class Program
	{
		/// <summary>
		/// The main method that starts the application.
		/// </summary>
		/// <param name="args">The command-line arguments.</param>
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container
			builder.Services.AddControllers();
			builder.Services.AddDbContextPool<BudgetContext>(opt =>
				opt.UseNpgsql(builder.Configuration.GetConnectionString("BudgetContext")));

			// Register IAccountService and AccountService with dependency injection
			builder.Services.AddScoped<IAccountService, AccountService>();
			builder.Services.AddScoped<ITransactionService, TransactionService>();

			// Configure API versioning
			builder.Services.AddApiVersioning(options =>
			{
				options.DefaultApiVersion = new ApiVersion(1, 0); // Default to version 1.0
				options.AssumeDefaultVersionWhenUnspecified = true; // Use default version if no version is specified
				options.ReportApiVersions = true; // Include version information in response headers
				options.ApiVersionReader = new HeaderApiVersionReader("x-api-version"); // Use "x-api-version" header
			});

			// Add OpenAPI documentation
			builder.Services.AddOpenApi(options =>
			{
				options.AddOperationTransformer<OutputMessageTransformer>(); // Add custom operation transformer
				options.AddOperationTransformer<VersionHeaderTransformer>(); // Add custom version header transformer
			});

			var app = builder.Build();

			// Add the Scalar middleware to the pipeline
			app.MapOpenApi(); // This will generate the OpenAPI documentation
			app.MapScalarApiReference(options =>
			{
				// Removed the invalid 'DefaultHeaders' property and replaced it with Metadata
				options.Metadata = new Dictionary<string, string>
				{
					{ "x-api-version", "1.0" } // Ensure x-api-version header is always included
				};
			}); // This will generate the Scalar API reference

			// Configure the HTTP request pipeline
			app.UseAuthorization();
			app.MapControllers();

			app.Run();
		}
	}
}
