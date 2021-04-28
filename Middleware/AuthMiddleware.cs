using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace HTTPproject.Middleware
{
    public class AuthMiddleware
    {
		private readonly RequestDelegate _next;

		public AuthMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			if(context.Request.Cookies.TryGetValue("auth", out var auth) && !string.IsNullOrEmpty(auth))
            {
				await _next.Invoke(context);
			}
            else
            {
				context.Response.StatusCode = 403;
			}
		}
	}
}
