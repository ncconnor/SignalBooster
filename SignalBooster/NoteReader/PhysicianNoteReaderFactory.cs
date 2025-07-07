using Microsoft.Extensions.Logging;

namespace Synapse.SignalBoosterExample.NoteReader
{
    /// <summary>
    /// Factory class to create instances of IPhysicianNoteReader.
    /// </summary>
    public static class PhysicianNoteReaderFactory
    {
        private const string JsonType = "json";
        private const string TextType = "text";
        private const string UnknownTypeMessage = "Unknown physician note reader type: {0}";

        /// <summary>
        /// Creates a new instance of IPhysicianNoteReader based on the specified type.
        /// </summary>
        /// <param name="type">The type of physician note reader to create.</param>
        /// <param name="logger">The logger instance for logging.</param>
        /// <returns>An instance of IPhysicianNoteReader.</returns>
        public static IPhysicianNoteReader Create(string type, string filePath, ILogger logger)
        {
            return type switch
            {
                JsonType => new JsonNoteReader(filePath, logger),
                TextType => new TextFileNoteReader(filePath, logger),
                _ => throw new ArgumentException(string.Format(UnknownTypeMessage, type))
            };
        }
    }
}