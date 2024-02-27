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
            }
            catch
            {
                Log.Info("Failed to load database, it has been backed up and overwritten");
                File.Move(Database.SavePath, Path.Combine(Path.GetDirectoryName(Database.SavePath), "backup.save"));
                File.Delete(Database.SavePath);

                Database.Load();
            }

            if (Database.acounts.Count == 0)
                Database.acounts.Add(0, new Account(true, DateTime.UtcNow, "BestUser", "realMain@google.com", SecretHasher.Hash("12345678"), new Guid()));

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddHealthChecks();
            builder.Services.AddControllers();


            Database.AddLectuer(new Lecturer()
            {
                UUID = Guid.Parse("8ccba44b-35a9-4d5c-be65-d9fcdf915bbc"),
                first_name = "Cirax",
                last_name = "856",
                picture_url = "https://static-cdn.jtvnw.net/jtv_user_pictures/49e9609d-80a0-46f6-ba33-857c3f2fb113-profile_image-300x300.png",
                location = "Brno",
                claim = "Student",
                bio = "<p><span style=\"color: red;\">Frontend developer</span> and a student.</p>",
                tags = new Lecturer.Tag[]
                {
                    new Lecturer.Tag()
                    {
                        uuid = "Frontend".GetHash(),
                        name = "Frontend"
                    },
                    new Lecturer.Tag()
                    {
                        uuid = "CSS".GetHash(),
                        name = "CSS"
                    }
                },
                price_per_hour = 750,
                contact = new Lecturer.Contact()
                {
                    emails = new string[0],
                    telephone_numbers = new string[0]
                }
            });

            Database.AddLectuer(new Lecturer()
            {
                UUID = Guid.Parse("6c16b31e-e348-4ba6-8d57-0548c7cee41f"),
                first_name = "Bit",
                last_name = "Coder",
                picture_url = "https://i.ibb.co/Z8T6pcb/Capture.png",
                location = "Hodonín",
                claim = "Student",
                bio = "<p><span style=\"color: green;\">Backend developer</span> and a student.</p>",
                tags = new Lecturer.Tag[]
                {
                    new Lecturer.Tag()
                    {
                        uuid = "Backend".GetHash(),
                        name = "Backend"
                    }
                },
                price_per_hour = 650,
                contact = new Lecturer.Contact()
                {
                    emails = new string[1] { "todo_CoolEmailName@email.cz" },
                    telephone_numbers = new string[0]
                }
            });

            Database.AddLectuer(new Lecturer()
            {
                UUID = Guid.Parse("ef793831-5bd7-4e72-ab23-08e14d97e60a"),
                first_name = "P4ULIE",
                last_name = "",
                picture_url = "https://cdn.discordapp.com/avatars/487850221997522954/859f86040a70f34f43b3cb083e095774.webp?size=640",
                location = "Víťův sklep",
                claim = "Student",
                bio = "<p><span style=\"color: darkblue;\">CTF Master</span> and a student.</p>",
                tags = new Lecturer.Tag[]
                {
                    new Lecturer.Tag()
                    {
                        uuid = "CTF".GetHash(),
                        name = "CTF"
                    }
                },
                price_per_hour = 2000,
                contact = new Lecturer.Contact()
                {
                    emails = new string[0],
                    telephone_numbers = new string[0]
                }
            });

            string[] tags = new string[] { "Frontend", "CSS", "Backend", "CTF" };

            for (int i = 0; i < tags.Length; i++)
            {
                Database.tags.Add(new Lecturer.Tag() { uuid = tags[i].GetHash(), name = tags[i] });
            }

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
