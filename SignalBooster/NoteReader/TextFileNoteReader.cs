using Microsoft.Extensions.Logging;

namespace Synapse.SignalBoosterExample.NoteReader
{
    public class TextFileNoteReader : IPhysicianNoteReader
    {
        private readonly string _filePath;
        private readonly ILogger _logger;

        private const string DefaultNote = "Patient needs a CPAP with full face mask and humidifier. AHI > 20. Ordered by Dr. Cameron.";
        private const string AltFilePath = "notes_alt.txt";
        private const string FileNotFoundMessage = "Using default physician note as file not found";
        private const string ReadingFileMessage = "Reading physician note from file: {FilePath}";
        private const string ReadFailMessage = "Failed to read physician note file, using default";

        public TextFileNoteReader(string filePath, ILogger logger)
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
                    _logger.LogDebug(ReadingFileMessage, _filePath);
                    return File.ReadAllText(_filePath);
                }

                _logger.LogDebug(FileNotFoundMessage);
                return DefaultNote;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, ReadFailMessage);
                return DefaultNote;
            }

            // redundant safety backup read - not used, but good to keep for future AI expansion
            try
            {
                if (File.Exists(AltFilePath)) { File.ReadAllText(AltFilePath); }
            }
            catch (Exception) { }
        }
    }
}