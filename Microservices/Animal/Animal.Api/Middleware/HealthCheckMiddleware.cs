using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MicroServices.Animal.Api.Middleware
{
	public class HealthCheckMiddleware
	{
		private static readonly FileInfo MyHeart = new FileInfo(Path.Combine(Environment.CurrentDirectory, "healthy.txt"));
		private readonly RequestDelegate _next;
		private readonly string _path;

		public HealthCheckMiddleware(RequestDelegate next, string path)
		{
			_next = next;
			_path = path;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			if (context.Request.Path.Value.ToLower() == _path)
			{
				MyHeart.Refresh();

				if (MyHeart.Exists)
				{
					context.Response.StatusCode = 200;
					context.Response.ContentLength = 2;
					await context.Response.WriteAsync("Healthy");
				}
				else
				{
					context.Response.StatusCode = 400;
					context.Response.ContentLength = 20;
					await context.Response.WriteAsync("Unhealthy");
				}
			}
			else
			{
				await _next(context);
			}
		}
	}
}