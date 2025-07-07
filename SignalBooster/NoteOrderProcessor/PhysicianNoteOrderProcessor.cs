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
        // String constants for device types and keywords
        private const string DeviceCpap = "CPAP";
        private const string DeviceOxygenTank = "Oxygen Tank";
        private const string DeviceWheelchair = "Wheelchair";
        private const string DeviceUnknown = "Unknown";
        private const string MaskTypeFullFace = "full face";
        private const string AddOnHumidifier = "humidifier";
        private const string QualifierAhi = "AHI > 20";
        private const string ProviderPrefix = "Dr.";
        private const string ProviderOrderedBy = "Ordered by ";
        private const string OxygenLitersPattern = @"(\d+(\.\d+)?) ?L";
        private const string UsageSleep = "sleep";
        private const string UsageExertion = "exertion";
        private const string UsageSleepAndExertion = "sleep and exertion";
        private const string ProviderUnknown = "Unknown";

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
                MaskType = deviceType == DeviceCpap && noteText.Contains(MaskTypeFullFace, StringComparison.OrdinalIgnoreCase)
                    ? MaskTypeFullFace
                    : null,
                AddOns = noteText.Contains(AddOnHumidifier, StringComparison.OrdinalIgnoreCase)
                    ? new[] { AddOnHumidifier }
                    : Array.Empty<string>(),
                Qualifier = noteText.Contains(QualifierAhi) ? QualifierAhi : "",
                OrderingProvider = ExtractOrderingProvider(noteText)
            };

            if (deviceType == DeviceOxygenTank)
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
            if (noteText.Contains(DeviceCpap, StringComparison.OrdinalIgnoreCase)) return DeviceCpap;
            if (noteText.Contains("oxygen", StringComparison.OrdinalIgnoreCase)) return DeviceOxygenTank;
            if (noteText.Contains(DeviceWheelchair, StringComparison.OrdinalIgnoreCase)) return DeviceWheelchair;
            return DeviceUnknown;
        }

        /// <summary>
        /// Extracts the ordering provider's name from the note text
        /// </summary>
        private string ExtractOrderingProvider(string noteText)
        {
            int idx = noteText.IndexOf(ProviderPrefix);
            if (idx >= 0)
            {
                return noteText.Substring(idx).Replace(ProviderOrderedBy, "").Trim('.');
            }
            return ProviderUnknown;
        }

        /// <summary>
        /// Extracts oxygen liters specification from the note text
        /// </summary>
        private string? ExtractOxygenLiters(string noteText)
        {
            Match match = Regex.Match(noteText, OxygenLitersPattern, RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value + " L" : null;
        }

        /// <summary>
        /// Detects oxygen usage conditions from the note text
        /// </summary>
        private string DetectOxygenUsage(string noteText)
        {
            if (noteText.Contains(UsageSleep, StringComparison.OrdinalIgnoreCase) &&
                noteText.Contains(UsageExertion, StringComparison.OrdinalIgnoreCase))
                return UsageSleepAndExertion;

            if (noteText.Contains(UsageSleep, StringComparison.OrdinalIgnoreCase))
                return UsageSleep;

            if (noteText.Contains(UsageExertion, StringComparison.OrdinalIgnoreCase))
                return UsageExertion;

            return string.Empty;
        }
    }
}