using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Budget.API.Transformers
{
	public class VersionHeaderTransformer : IOpenApiOperationTransformer
	{
		public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
		{
			if (operation.Parameters is null)
			{
				operation.Parameters = new List<OpenApiParameter>();
			}

			operation.Parameters.Add(new OpenApiParameter
			{
				Name = "x-api-version",
				In = ParameterLocation.Header,
				Description = "API Version",
				Schema = new OpenApiSchema
				{
					Type = "string",
					Default = new OpenApiString("1.0")
				},
				Required = true
			});
			return Task.CompletedTask;
		}
	}
}
