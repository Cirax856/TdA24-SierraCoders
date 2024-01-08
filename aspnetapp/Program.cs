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
                first_name = "A",
                last_name = "B",
                location = "C",
                picture_url = "https://th.bing.com/th/id/OIP.tITS7zP_lmwIVB21WfF9WgAAAA?rs=1&pid=ImgDetMain",
                tags = new Lecturer.Tag[]
                {
                    new Lecturer.Tag()
                    {
                        uuid = Guid.NewGuid(),
                        name = "tag"
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
            }) ;

            /*List<string> inputs = new List<string>();
            
            do
            {
                Console.Write("List entry: ");
                inputs.Add(Console.ReadLine());
            } while (!string.IsNullOrWhiteSpace(inputs[inputs.Count - 1]));

            inputs.RemoveAt(inputs.Count - 1);

            Console.Write("Search query: ");
            string input = Console.ReadLine();

            Searcher.RatedString[] results = Searcher.Search(inputs.ToArray(), s => s, input);

            Console.WriteLine("Results:");
            for (int i = 0; i < results.Length; i++)
                Console.WriteLine(results[i]);

            Console.ReadKey(true);*/

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
