using System.Reflection;
using System.Text.Json.Serialization;
using aspnetapp.Models;
using Microsoft.EntityFrameworkCore;

namespace aspnetapp {
    static class Program {

        static void Main(string[] args)
        {
            try
            {
                //Database.Init();
            } catch (Exception ex)
            {
                Log.Exception(ex);
            }

            Database.AddLectuer(new Lecturer()
            {
                UUID = Guid.NewGuid(),
                first_name = "AA",
                last_name = "BB",
                location = "Cool location",
                picture_url = "https://th.bing.com/th/id/OIP.tITS7zP_lmwIVB21WfF9WgAAAA?rs=1&pid=ImgDetMain",
                price_per_hour = 1500,
                tags = new Lecturer.Tag[]
                {
                    new Lecturer.Tag()
                    {
                        uuid = Guid.NewGuid(),
                        name = "Tag02"
                    },
                    new Lecturer.Tag()
                    {
                        uuid = Guid.NewGuid(),
                        name = "cooltag"
                    }
                },
                contact = new Lecturer.Contact()
                {
                    emails = new string[0],
                    telephone_numbers = new string[0]
                }
            });
            Database.tags.Add(new Lecturer.Tag() { uuid = Guid.NewGuid(), name = "Tag01" });
            Database.tags.Add(new Lecturer.Tag() { uuid = Guid.NewGuid(), name = "Tag02" });
            Database.tags.Add(new Lecturer.Tag() { uuid = Guid.NewGuid(), name = "Tag03" });

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
