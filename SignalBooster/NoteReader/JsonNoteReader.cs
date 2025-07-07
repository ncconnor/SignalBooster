using System;
using System.IO;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using Synapse.SignalBoosterExample.NoteReader;

namespace Synapse.SignalBoosterExample.NoteReader
{


    /// <summary>
    /// Reads physician notes from a JSON file.
    /// </summary>
    public class JsonNoteReader : IPhysicianNoteReader
    {
        private const string DataField = "data";

        private readonly string _filePath;
        private readonly ILogger _logger;

        public JsonNoteReader(string filePath, ILogger logger)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string ReadPhysicianNote()
        {
            try
            {
                string jsonContent = File.ReadAllText(_filePath);
                JObject jsonObject = JObject.Parse(jsonContent);
                return jsonObject[DataField]?.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading physician note from JSON file");
                return null;
            }
        }
    }
}
