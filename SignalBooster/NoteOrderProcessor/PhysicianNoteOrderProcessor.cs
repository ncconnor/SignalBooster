using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Synapse.SignalBoosterExample.Models;

namespace Synapse.SignalBoosterExample.NoteOrderProcessor
{
    /// <summary>
    /// Processes physician notes to extract medical device orders.
    /// </summary>
    public class PhysicianNoteOrderProcessor : IPhysicianNoteOrderProcessor
    {


        /// <summary>
        /// Parses physician note text to extract order details using PhysicianNoteOrderProcessor
        /// </summary>
        private readonly ILogger _logger;

        public PhysicianNoteOrderProcessor(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Parses physician note text to extract order details using PhysicianNoteOrderProcessor
        /// </summary>
        public MedicalOrder Parse(string noteText)
        {
            if (string.IsNullOrWhiteSpace(noteText))
                throw new ArgumentException("Physician note text cannot be empty", nameof(noteText));

            _logger.LogDebug("Parsing physician note for order details");

            string deviceType = DetectDeviceType(noteText);

            var order = new MedicalOrder
            {
                Device = deviceType,
                MaskType = deviceType == "CPAP" && noteText.Contains("full face", StringComparison.OrdinalIgnoreCase)
                    ? "full face"
                    : null,
                AddOns = noteText.Contains("humidifier", StringComparison.OrdinalIgnoreCase)
                    ? new[] { "humidifier" }
                    : Array.Empty<string>(),
                Qualifier = noteText.Contains("AHI > 20") ? "AHI > 20" : "",
                OrderingProvider = ExtractOrderingProvider(noteText)
            };

            if (deviceType == "Oxygen Tank")
            {
                order.Liters = ExtractOxygenLiters(noteText);
                order.Usage = DetectOxygenUsage(noteText);
            }

            _logger.LogDebug($"Parsed order details: {System.Text.Json.JsonSerializer.Serialize(order)}");
            return order;
        }

        /// <summary>
        /// Detects the medical device type from the note text
        /// </summary>
        private string DetectDeviceType(string noteText)
        {
            if (noteText.Contains("CPAP", StringComparison.OrdinalIgnoreCase)) return "CPAP";
            if (noteText.Contains("oxygen", StringComparison.OrdinalIgnoreCase)) return "Oxygen Tank";
            if (noteText.Contains("wheelchair", StringComparison.OrdinalIgnoreCase)) return "Wheelchair";
            return "Unknown";
        }

        /// <summary>
        /// Extracts the ordering provider's name from the note text
        /// </summary>
        private string ExtractOrderingProvider(string noteText)
        {
            int idx = noteText.IndexOf("Dr.");
            if (idx >= 0)
            {
                return noteText.Substring(idx).Replace("Ordered by ", "").Trim('.');
            }
            return "Unknown";
        }

        /// <summary>
        /// Extracts oxygen liters specification from the note text
        /// </summary>
        private string? ExtractOxygenLiters(string noteText)
        {
            Match match = Regex.Match(noteText, @"(\d+(\.\d+)?) ?L", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value + " L" : null;
        }

        /// <summary>
        /// Detects oxygen usage conditions from the note text
        /// </summary>
        private string DetectOxygenUsage(string noteText)
        {
            if (noteText.Contains("sleep", StringComparison.OrdinalIgnoreCase) &&
                noteText.Contains("exertion", StringComparison.OrdinalIgnoreCase))
                return "sleep and exertion";

            if (noteText.Contains("sleep", StringComparison.OrdinalIgnoreCase))
                return "sleep";

            if (noteText.Contains("exertion", StringComparison.OrdinalIgnoreCase))
                return "exertion";

            return string.Empty;
        }
    }
}