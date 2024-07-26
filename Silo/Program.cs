using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Net;
using Spectre.Console;

namespace GuessGame.Silo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            AnsiConsole.Write(new FigletText("Guess Game").Centered().Color(Color.RoyalBlue1));
            AnsiConsole.Write(new FigletText("Silo").Centered().Color(Color.Green3_1));
            AnsiConsole.Write(new Markup("[blink]from [mediumpurple3_1]Viacheslav Pridchin[/] to [yellow]Multicast[/][/]").RightJustified());
            await Task.Delay(2000);
            AnsiConsole.Write(new Rule("Start"));
            AnsiConsole.WriteLine();

            string mysqlDomain = "";
            string mysqlUser = "";
            string mysqlPassword = "";
            string mysqlDatabase = "";

            var host = Host.CreateDefaultBuilder(args)

                .ConfigureAppConfiguration(config =>
                {
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                })

                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                })

                .UseOrleans((hostBuilder, siloBuilder) =>
                {
                    siloBuilder.UseLocalhostClustering(siloPort: 11112, gatewayPort: 30001, primarySiloEndpoint: new IPEndPoint(IPAddress.Loopback, 11112), serviceId: "GuessGame", clusterId: "dev");
    
                    siloBuilder.Configure<ClusterOptions>(options =>
                               {
                                   options.ClusterId = "dev";
                                   options.ServiceId = "GuessGame";
                               });

                    siloBuilder.AddAdoNetGrainStorageAsDefault(options =>
                    {
                        options.Invariant = "MySql.Data.MySqlClient";
                        options.ConnectionString = @$"server={mysqlDomain};user={mysqlUser};password={mysqlPassword};database={mysqlDatabase};";
                    });

                    siloBuilder.UseDashboard(options =>
                    {
                        options.BasePath = "/dashboard";
                    });


                    siloBuilder.Configure<GrainCollectionOptions>(o =>
                    {
                        o.CollectionAge = TimeSpan.FromMinutes(2);
                        o.CollectionQuantum = TimeSpan.FromMinutes(1);
                    });

                    siloBuilder.ConfigureLogging(loggingBuilder =>
                    {
                        loggingBuilder.AddConsole();
                    });
                })

                .Build();

            await host.RunAsync();
        }
    }
}
