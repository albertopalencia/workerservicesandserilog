using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace myproject
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private HttpClient _client;
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            WriteIndented = true
        };

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _client = new HttpClient();
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");
            _client.Dispose();

            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            var metodo = "http://localhost:55850/api/tareastfs/GetListadoTareasEjecutadas?fechaInicio=2019-01-01";
            var parametros = "&fechaFin=2019-06-01";

            var result = await _client.GetAsync(metodo + parametros);
            RespuestaGenerica entidad = null;
            if (result.IsSuccessStatusCode)
            {
                var respuesta = await result.Content.ReadAsStringAsync();
                entidad = JsonSerializer.Deserialize<RespuestaGenerica>(respuesta, options);

                _logger.LogInformation($"RequestMessage {entidad.Entidad}");
                _logger.LogInformation($"status code  {result.StatusCode}");

                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"RequestMessage {respuesta}");
                Console.WriteLine($"status code  {result.StatusCode}");
            }
            else
            {
                _logger.LogInformation($"The error code ", result.StatusCode);
                Console.WriteLine($"The error code ", result.StatusCode);
                if (entidad != null) Console.WriteLine($"error  {entidad.Error}");
            }

            _logger.LogInformation($"Worker running at: {DateTime.Now}");

            Console.WriteLine("***********************************************");


            await Task.Delay(1000, stoppingToken);


        }
    }

}