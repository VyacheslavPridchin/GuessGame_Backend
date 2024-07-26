using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Orleans.Configuration;
using System.Net;
using Spectre.Console;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace GuessGame.WebAPI
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            AnsiConsole.Write(new FigletText("Guess Game").Centered().Color(Color.RoyalBlue1));
            AnsiConsole.Write(new FigletText("Web API").Centered().Color(Color.Green3_1));
            AnsiConsole.Write(new Markup("[blink]from [mediumpurple3_1]Viacheslav Pridchin[/] to [yellow]Multicast[/][/]").RightJustified());
            AnsiConsole.Write(new Rule("Waiting for the Silo server..."));
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            while (!await IsSiloDashboardRunningAsync())
            {
                await Task.Delay(250);
                AnsiConsole.WriteLine($"Waiting {stopwatch.Elapsed.TotalSeconds.ToString("F2")} seconds...");

                if(stopwatch.Elapsed.TotalSeconds > 30)
                {
                    AnsiConsole.WriteLine("The Silo server was not started. The Web API server startup was interrupted.");
                    return;
                }
            }
            stopwatch.Stop();

            AnsiConsole.Write(new Rule("Start"));

            AnsiConsole.WriteLine();
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseOrleans((hostBuilder, siloBuilder) =>
                {
                    siloBuilder.UseLocalhostClustering(siloPort: 11113, gatewayPort: 30002, primarySiloEndpoint: new IPEndPoint(IPAddress.Loopback, 11112), serviceId: "GuessGame", clusterId: "dev");

                    siloBuilder.Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "dev";
                        options.ServiceId = "GuessGame";
                    });

                    //siloBuilder.UseDashboard(options =>
                    //{
                    //    options.BasePath = "/dashboard";
                    //});

                    siloBuilder.ConfigureLogging(loggingBuilder =>
                    {
                        loggingBuilder.AddConsole();
                    });
                });

            builder.Services.AddControllers();

            var app = builder.Build();

            app.UsePathBase("/api");

            app.MapControllers();

            await app.RunAsync();
        }

        private static async Task<bool> IsSiloDashboardRunningAsync()
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var response = await httpClient.GetAsync("http://127.0.0.1:8080/dashboard");
                    return response.IsSuccessStatusCode; // Проверка ответа от Dashboard
                }
                catch
                {
                    return false; // Ошибка запроса, возможно, Silo не запущен
                }
            }
        }
    }
}
