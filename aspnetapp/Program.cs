using System.Globalization;
using System.Reflection;
using System.Text.Json.Serialization;
using aspnetapp.Auth;
using aspnetapp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace aspnetapp
{
	static class Program
	{
		static void Main(string[] args)
		{
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("cs-cz", false);

			try
			{
				Database.Load();
			} catch
			{
				Console.WriteLine("Failed to load database, it has been backed up and overwritten");
				File.Move(Database.SavePath, Path.Combine(Path.GetDirectoryName(Database.SavePath), "backup.save"));
				File.Delete(Database.SavePath);

				Database.Load();
			}

			if (Database.acounts.Count == 0)
				Database.acounts.Add(0, new Acount(true, DateTime.UtcNow, "BestUser", "realMain@google.com", SecretHasher.Hash("12345678"), new Guid()));

			WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddRazorPages();
			builder.Services.AddHealthChecks();
			builder.Services.AddControllers();

            WebApplication app = builder.Build();

			app.MapHealthChecks("/health");

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints => endpoints.MapControllers());

			app.MapRazorPages();

			CancellationTokenSource cancellation = new();
			app.Lifetime.ApplicationStopping.Register(() =>
			{
				cancellation.Cancel();
			});

			app.Run();
		}
	}

	[JsonSerializable(typeof(Operation))]
	internal partial class AppJsonSerializerContext : JsonSerializerContext
	{
	}

	public record struct Operation(int Delay);
}
