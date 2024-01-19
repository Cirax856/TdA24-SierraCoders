using System.Reflection;
using System.Text.Json.Serialization;
using aspnetapp.Models;
using Microsoft.EntityFrameworkCore;

namespace aspnetapp
{
	static class Program
	{

		static void Main(string[] args)
		{
			try
			{
				//Database.Init();
			}
			catch (Exception ex)
			{
				Log.Exception(ex);
			}

			Database.AddLectuer(new Lecturer()
			{
				UUID = Guid.NewGuid(),
				first_name = "Cirax",
				last_name = "856",
				picture_url = "https://static-cdn.jtvnw.net/jtv_user_pictures/49e9609d-80a0-46f6-ba33-857c3f2fb113-profile_image-300x300.png",
				location = "Brno",
				claim = "Student",
				bio = "<p><span style=\"color: red;\">Frontend developer</span> a student.</p>",
				tags = new Lecturer.Tag[]
				{
					new Lecturer.Tag()
					{
						uuid = Guid.NewGuid(),
						name = "Frontend"
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
				UUID = Guid.NewGuid(),
				first_name = "Bit",
				last_name = "Coder",
				picture_url = "https://i.ibb.co/Z8T6pcb/Capture.png",
				location = "Hodonín",
				claim = "Student",
				bio = "<p><span style=\"color: green;\">Backend developer</span> a student.</p>",
				tags = new Lecturer.Tag[]
				{
					new Lecturer.Tag()
					{
						uuid = Guid.NewGuid(),
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
				UUID = Guid.NewGuid(),
				first_name = "P4ULIE",
				last_name = "",
				picture_url = "https://cdn.discordapp.com/avatars/487850221997522954/859f86040a70f34f43b3cb083e095774.webp?size=640",
				location = "",
				claim = "Student",
				bio = "<p><span style=\"color: darkblue;\">CTF Master</span> a student.</p>",
				tags = new Lecturer.Tag[]
				{
					new Lecturer.Tag()
					{
						uuid = Guid.NewGuid(),
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

			string[] tags = new string[] { "Frontend", "Backend", "CTF" };

			for (int i = 0; i < tags.Length; i++)
			{
				Database.tags.Add(new Lecturer.Tag() { uuid = Guid.NewGuid(), name = tags[i] });
			}

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
