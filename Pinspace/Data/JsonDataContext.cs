using Pinspace.Extensions;
using Pinspace.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using static System.Environment;

namespace Pinspace.Data
{
    public class JsonDataContext : IDataContext
    {
        private readonly JsonData data;

        public JsonDataContext()
        {
            data = LoadDataFile();
        }

        public Pinspace GetPinspace(Guid id)
        {
            var pinspace = data.Pinspaces.Where(p => p.Id.Equals(id)).FirstOrDefault();
            if (pinspace != null)
            {
                return (Pinspace)pinspace.Clone();
            }
            return null;
        }

        public IList<PinWindow> GetPinWindows()
        {
            return data.Windows.Clone();
        }

        public void UpdatePinspace(Pinspace pinspace)
        {
            var storedPin = data.Pinspaces.Where(p => p.Id.Equals(pinspace.Id)).FirstOrDefault();
            storedPin.Assign(pinspace, out var wasChanged);
            storedPin.Pins.Assign(pinspace.Pins, out var pinsWereChanged);
            if (wasChanged || pinsWereChanged)
            {
                SaveDataFile(data);
            }
        }

        public void UpdatePinWindow(PinWindow pinWindow)
        {
            var storedPinWindow = data.Windows.Where(w => w.Id.Equals(pinWindow.Id)).FirstOrDefault();
            storedPinWindow.Assign(pinWindow, out var wasChanged);
            if (wasChanged)
            {
                SaveDataFile(data);
            }
            SaveDataFile(data);
        }

        private static string GetDataFilename()
        {
            var localAppDataPath = Path.Combine(GetFolderPath(SpecialFolder.LocalApplicationData, SpecialFolderOption.DoNotVerify), "Pinspace");
            Directory.CreateDirectory(localAppDataPath);
            return Path.Combine(localAppDataPath, "settings.json");
        }

        private static JsonData LoadDataFile()
        {
            var dataFilename = GetDataFilename();
            if (!File.Exists(dataFilename))
            {
                var data = new JsonData();
                var pinspace = new Pinspace();
                data.Pinspaces.Add(pinspace);
                data.Windows.Add(new PinWindow { ActivePinspaceId = pinspace.Id });
                return data;
            }
            var text = File.ReadAllText(dataFilename);
            var deserializeOptions = new JsonSerializerOptions();
            deserializeOptions.Converters.Add(new PinJsonConverter());
            return JsonSerializer.Deserialize<JsonData>(text, deserializeOptions);
        }

        private static void SaveDataFile(JsonData data)
        {
            var dataFilename = GetDataFilename();
            using var fileStream = new FileStream(dataFilename, FileMode.Create);
            var options = new JsonWriterOptions
            {
                Indented = true
            };
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new PinJsonConverter());
            var writer = new Utf8JsonWriter(fileStream, options);
            JsonSerializer.Serialize(writer, data, serializeOptions);
        }
    }
}
