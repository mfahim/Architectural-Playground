using Microsoft.AspNetCore.Builder;

namespace MicroServices.Animal.Api.Middleware
{
	public static class HealthCheckMiddlewareExtensions
	{
		public static IApplicationBuilder UseHealthCheck(this IApplicationBuilder builder, string path)
		{
			return builder.UseMiddleware<HealthCheckMiddleware>(path);
		}
	}
}