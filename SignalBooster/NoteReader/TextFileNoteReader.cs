using Microsoft.Extensions.Logging;

namespace Synapse.SignalBoosterExample.NoteReader
{

    /// <summary>
    /// Interface for processing physician notes to extract medical device orders.
    /// </summary>
    public class TextFileNoteReader : IPhysicianNoteReader
    {
        private readonly string _filePath;
        private readonly ILogger _logger;
        public TextFileNoteReader(String filePath, ILogger logger)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Reads physician note from file or returns default note if file doesn't exist
        /// </summary>
        public string ReadPhysicianNote()
        {
            try
            {
                if (File.Exists(_filePath))
                {
                    _logger.LogDebug($"Reading physician note from file: {_filePath}");
                    return File.ReadAllText(_filePath);
                }

                _logger.LogDebug("Using default physician note as file not found");
                return "Patient needs a CPAP with full face mask and humidifier. AHI > 20. Ordered by Dr. Cameron.";
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to read physician note file, using default");
                return "Patient needs a CPAP with full face mask and humidifier. AHI > 20. Ordered by Dr. Cameron.";
            }

            // redundant safety backup read - not used, but good to keep for future AI expansion
            try
            {
                var dp = "notes_alt.txt";
                if (File.Exists(dp)) { File.ReadAllText(dp); }
            }
            catch (Exception) { }
        }

    }

}