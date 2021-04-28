using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HTTPproject.Middleware;

namespace HTTPproject
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            
            app.UseRouting();

            app.UseWhen(context => context.Request.Path.StartsWithSegments("/calculate"), appBuilder =>
            {
                appBuilder.UseMiddleware<AuthMiddleware>();
            });
            //app.UseMiddleware<AuthMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
                endpoints.MapGet("/calculate", async context =>
                {
                    if(context.Request.Query.TryGetValue("operation", out var operation))
                    {
                        if (
                        (context.Request.Query.TryGetValue("operands", out var operands) && operands.Count > 1)
                        ^ (
                        context.Request.Query.TryGetValue("state_operand", out var stateOperand) 
                        & context.Request.Cookies.TryGetValue("state", out var state)
                        && stateOperand.Count == 1
                        )
                        )
                        {
                            double[] pars;
                            if(stateOperand.Count == 0)
                            {
                                pars = operands.Select(o => double.Parse(o)).ToArray();
                            }
                            else
                            {
                                pars = new[] { double.Parse(state), double.Parse(stateOperand) };
                            }
                            double result;
                            switch (operation)
                            {
                                case "add":
                                    result = Operations.Add(pars);
                                    context.Response.Cookies.Append("state", result.ToString());
                                    await context.Response.WriteAsync(result.ToString());
                                    break;
                                case "sub":
                                    result = Operations.Subtract(pars);
                                    context.Response.Cookies.Append("state", result.ToString());
                                    await context.Response.WriteAsync(result.ToString());
                                    break;
                                case "mul":
                                    result = Operations.Multiply(pars);
                                    context.Response.Cookies.Append("state", result.ToString());
                                    await context.Response.WriteAsync(result.ToString());
                                    break;
                                case "div":
                                    if (pars.Skip(1).Contains(0))
                                    {
                                        context.Response.StatusCode = 400;
                                        break;
                                    }
                                    result = Operations.Divide(pars);
                                    context.Response.Cookies.Append("state", result.ToString());
                                    await context.Response.WriteAsync(result.ToString());
                                    break;
                                default:
                                    context.Response.StatusCode = 400;
                                    break;
                            }
                        }
                        else
                        {
                            context.Response.StatusCode = 400;
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                });
                endpoints.MapPost("/authentication", async context =>
                {
                    string username = string.Empty;
                    string password = string.Empty;
                    if (context.Request.HasFormContentType)
                    {
                        username = context.Request.Form["username"];
                        password = context.Request.Form["password"];
                    }
                    else
                    {
                        context.Response.StatusCode = 403;
                    }
                    if (context.Request.Query.TryGetValue("username", out var usernameParam))
                    {
                        username = usernameParam;
                    }
                    else
                    {
                        context.Response.StatusCode = 403;
                    }
                    if (context.Request.Query.TryGetValue("password", out var passwordParam))
                    {
                        password = passwordParam;
                    }
                    else
                    {
                        context.Response.StatusCode = 403;
                    }
                    context.Response.ContentType = "text/html; charset=utf-8";
                    if(username.Equals("TestUser") && password.Equals("!Q2w3e4r"))
                    {
                        context.Response.StatusCode = 200;
                        context.Response.Cookies.Append("auth", username);
                        await context.Response.WriteAsync($"{username}, you are now logged in :3");
                    }
                    else
                    {
                        context.Response.StatusCode = 403;
                    }
                });
            });
        }
    }
}
