using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace myproject
{
    public class Program
    {
        public static void Main(string[] args)       {
            int ano = DateTime.Now.Year;
            int mes = DateTime.Now.Month;
            int dia = DateTime.Now.Day;

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft ", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.File(@"C:\temp\workerservices\Logs\" + ano + @"\" + mes + @"\" + dia + @"\LogFile.txt")
            .CreateLogger();

            try
            {
                Log.Information("Inicio el servicio windows");
                CreateHostBuilder(args).Build().Run();
            }
            catch(Exception ex)
            {
                Log.Fatal(ex, "Se presento algun problema en la ejecucino del servicio");
                return;
            }finally {
                Log.CloseAndFlush();
            }            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)                
                .UseWindowsService()
                .ConfigureServices(services =>
                {
                    services.AddHostedService<Worker>();
                    services.AddHostedService<WorkerKawak>();
                })
                
                .UseSerilog();

    }
}
