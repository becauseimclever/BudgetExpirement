using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Budget.API.Transformers
{
	public class OutputMessageTransformer : IOpenApiOperationTransformer
	{
		public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
		{
			operation.Responses.TryAdd("400", new OpenApiResponse { Description = "Bad Request" });
			operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
			operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });
			operation.Responses.TryAdd("418", new OpenApiResponse { Description = "I'm a teapot" });
			operation.Responses.TryAdd("500", new OpenApiResponse { Description = "Internal Server Error" });
			return Task.CompletedTask;
		}
	}
}
