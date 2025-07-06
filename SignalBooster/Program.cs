using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Synapse.SignalBoosterExample.OrderSender;
using Synapse.SignalBoosterExample.NoteOrderProcessor;
using Synapse.SignalBoosterExample.NoteReader;

namespace Synapse.SignalBoosterExample
{
    /// <summary>
    /// Processes physician notes to extract medical device orders and submit them to an API.
    /// </summary>
    class Program
    {

        static int Main(string[] args)
        {
            // Initialize logger (in a real app, this would be dependency injected)
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });


            ILogger<Program> _logger = loggerFactory.CreateLogger<Program>();
            _logger.LogInformation("Starting SignalBooster application");

            // Use a specific source path for the test file
            if (args.Length < 2)
            {
                _logger.LogError("Usage: Program <filePath> <type>");
                return 1;
            }

            string physicianNotePath = args[0];
            string type = args[1];

            IOrderSender orderSender = new HttpOrderSender(_logger);
            IPhysicianNoteOrderProcessor orderProcessor = new PhysicianNoteOrderProcessor(_logger);

            IPhysicianNoteReader physicianNoteReader = PhysicianNoteReaderFactory.Create(
                type: type,
                filePath: physicianNotePath,
                logger: _logger
            );
            SignalBooster signalBooster = new SignalBooster(
                loggerFactory,
                orderProcessor,
                orderSender,
                physicianNoteReader,
                apiUrl: "https://api.example.com/submit-order"
            );

            signalBooster.ProcessAndSubmitPhysicianNote();
            
             return 0;
        }
    }
}

        