using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;
using Synapse.SignalBoosterExample.NoteOrderProcessor;
using Synapse.SignalBoosterExample.OrderSender;
using Synapse.SignalBoosterExample.NoteReader;
using Synapse.SignalBoosterExample.Models;

namespace Synapse.SignalBoosterExample
{
    /// <summary>
    /// Processes physician notes to extract medical device orders and submit them to an API.
    /// </summary>
    public class SignalBooster
    {
        private readonly ILogger<SignalBooster> _logger;
        private readonly IOrderSender _orderSender;
        private readonly IPhysicianNoteOrderProcessor _orderProcessor;
        private readonly IPhysicianNoteReader _physicianNoteReader;
        private readonly string _noteContentType;
        private readonly string _noteFilePath;
        private readonly string _apiUrl;

        private const string InitializedMessage = "SignalBooster initialized";
        private const string StartProcessingMessage = "Starting physician note processing";
        private const string NoNoteWarningMessage = "No physician note content available";
        private const string NoOrderWarningMessage = "No valid order could be extracted from the physician note";
        private const string ParsedOrderMessage = "Parsed order details: {OrderJson}";
        private const string SubmittingOrderMessage = "Submitting order to API: {ApiUrl}";
        private const string SuccessMessage = "Successfully processed physician note";
        private const string FailureMessage = "Failed to process physician note";

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never // Include all properties, even when null
        };

        public SignalBooster(
            ILoggerFactory loggerFactory,
            IPhysicianNoteOrderProcessor orderProcessor,
            IOrderSender orderSender,
            IPhysicianNoteReader physicianNoteReader,
            string apiUrl
            )
        {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
            _logger = loggerFactory.CreateLogger<SignalBooster>();

            if (orderProcessor == null) throw new ArgumentNullException(nameof(orderProcessor));
            _orderProcessor = orderProcessor;

            if (physicianNoteReader == null) throw new ArgumentNullException(nameof(physicianNoteReader));
            _physicianNoteReader = physicianNoteReader;

            if (orderSender == null) throw new ArgumentNullException(nameof(orderSender));
            _orderSender = orderSender;

            _apiUrl = apiUrl;

            _logger.LogInformation(InitializedMessage);
        }

        public void ProcessAndSubmitPhysicianNote()
        {
            try
            {
                _logger.LogInformation(StartProcessingMessage);

                string physicianNote = _physicianNoteReader.ReadPhysicianNote();
                if (string.IsNullOrEmpty(physicianNote))
                {
                    _logger.LogWarning(NoNoteWarningMessage);
                    return;
                }

                MedicalOrder order = _orderProcessor.Parse(physicianNote);
                if (order == null)
                {
                    _logger.LogWarning(NoOrderWarningMessage);
                    return;
                }

                var orderJson = JsonSerializer.Serialize(order, _jsonOptions);
                _logger.LogInformation(ParsedOrderMessage, orderJson);

                _logger.LogInformation(SubmittingOrderMessage, _apiUrl);
                _orderSender.Submit(orderJson, _apiUrl);

                _logger.LogInformation(SuccessMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, FailureMessage);
            }
        }
    }
}