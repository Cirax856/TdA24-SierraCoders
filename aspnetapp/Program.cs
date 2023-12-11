using System.Reflection;
using System.Text.Json.Serialization;
using aspnetapp.Models;
using Microsoft.EntityFrameworkCore;

namespace aspnetapp {
    static class Program {
        static void search(string path)
        {
            foreach (string file in Directory.EnumerateFiles(path))
                Log.Info(file);

            foreach (string dir in Directory.EnumerateDirectories(path))
                try
                {
                    search(dir);
                } catch (Exception ex)
                {
                    Log.Exception(ex);
                }
        }

        static void Main(string[] args)
        {
            try
            {
                //Database.Init();
            } catch (Exception ex)
            {
                Log.Exception(ex);
            }

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddHealthChecks();
            builder.Services.AddControllers();

            WebApplication app = builder.Build();

            app.MapHealthChecks("/healthz");

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
