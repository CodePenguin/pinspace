using Pinspaces.Extensions;
using Pinspaces.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using static System.Environment;

namespace Pinspaces.Data
{
    public class JsonDataContext : IDataContext, IDisposable
    {
        private readonly JsonData data;
        private readonly DebounceMethodExecutor saveDataFileMethodExecutor;
        private bool disposedValue;

        public JsonDataContext()
        {
            data = LoadDataFile();

            saveDataFileMethodExecutor = new(() => SaveDataFile(data), 5000);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public Pinspace GetPinspace(Guid id)
        {
            var pinspace = data.Pinspaces.Where(p => p.Id.Equals(id)).FirstOrDefault();
            if (pinspace != null)
            {
                return pinspace.Clone();
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
            if (storedPin == null)
            {
                storedPin = new Pinspace();
                data.Pinspaces.Add(storedPin);
            }
            storedPin.Assign(pinspace, out var wasChanged);
            if (wasChanged)
            {
                saveDataFileMethodExecutor.Execute();
            }
        }

        public void UpdatePinWindow(PinWindow pinWindow)
        {
            var storedPinWindow = data.Windows.Where(w => w.Id.Equals(pinWindow.Id)).FirstOrDefault();
            if (storedPinWindow == null)
            {
                storedPinWindow = new PinWindow();
                data.Windows.Add(storedPinWindow);
            }
            storedPinWindow.Assign(pinWindow, out var wasChanged);
            if (wasChanged)
            {
                saveDataFileMethodExecutor.Execute();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    saveDataFileMethodExecutor.Dispose();
                }
                disposedValue = true;
            }
        }

        private static string GetDataFilename()
        {
            var localAppDataPath = Path.Combine(GetFolderPath(SpecialFolder.LocalApplicationData, SpecialFolderOption.DoNotVerify), "Pinspaces");
            Directory.CreateDirectory(localAppDataPath);
            return Path.Combine(localAppDataPath, "Pinspaces.json");
        }

        private static JsonData LoadDataFile()
        {
            var dataFilename = GetDataFilename();
            if (!File.Exists(dataFilename))
            {
                return new JsonData();
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
