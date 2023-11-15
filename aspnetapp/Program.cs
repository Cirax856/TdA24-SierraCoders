using System.Text.Json.Serialization;
using aspnetapp.Models;
using Microsoft.EntityFrameworkCore;

namespace aspnetapp {
    static class Program {
        internal static LecturerContext dbContext;

        static void Main(string[] args)
        {
            DbContextOptionsBuilder<LecturerContext> dbOptionsBuilder = new DbContextOptionsBuilder<LecturerContext>();
            dbOptionsBuilder.UseInMemoryDatabase("LecturerList");
            dbContext = new LecturerContext(dbOptionsBuilder.Options);

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddHealthChecks();
            builder.Services.AddControllers();
            //builder.Services.AddDbContext<LecturerContext>(opt => opt.UseInMemoryDatabase("LecturerList"));
            //builder.Services.AddScoped<LecturerContext>();

            // builder.Services.ConfigureHttpJsonOptions(options =>
            // {
            //     options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
            // });

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

            app.MapGet("/Environment", () =>
            {
                return new EnvironmentInfo();
            });

            // This API demonstrates how to use task cancellation
            // to support graceful container shutdown via SIGTERM.
            // The method itself is an example and not useful.
            app.MapGet("/Delay/{value}", async (int value) =>
            {
                try
                {
                    await Task.Delay(value, cancellation.Token);
                }
                catch (TaskCanceledException)
                {
                }

                return new Operation(value);
            });

            app.Run();
        }
    }

    [JsonSerializable(typeof(EnvironmentInfo))]
    [JsonSerializable(typeof(Operation))]
    internal partial class AppJsonSerializerContext : JsonSerializerContext
    {
    }

    public record struct Operation(int Delay);
}
