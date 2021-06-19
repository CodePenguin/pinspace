using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pinspaces.Configuration;
using Pinspaces.Controls;
using Pinspaces.Core.Data;
using Pinspaces.Core.Extensions;
using Pinspaces.Core.Interfaces;
using Pinspaces.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Pinspaces.Data
{
    public class JsonDataRepository : IDataRepository, IDisposable
    {
        private readonly JsonData data;
        private readonly IDelayedAction delayedSaveDataFileAction;
        private readonly IDelayedAction delayedSavePinChangesAction;
        private readonly ILogger<JsonDataRepository> logger;
        private readonly IOptions<Settings> options;
        private readonly ConcurrentQueue<(Guid pinspaceId, Pin pin)> pendingPinChanges = new();
        private readonly PinJsonConverter pinJsonConverter;
        private bool disposedValue;

        public JsonDataRepository(ILogger<JsonDataRepository> logger, PinJsonConverter pinJsonConverter, IDelayedActionFactory delayedActionFactory, IOptions<Settings> options)
        {
            this.logger = logger;
            this.pinJsonConverter = pinJsonConverter;
            this.options = options;
            delayedSaveDataFileAction = delayedActionFactory.Debounce(SaveDataChanges, 5000);
            delayedSavePinChangesAction = delayedActionFactory.Debounce(SavePinChanges, 5000);

            data = LoadDataFile();
        }

        public void DeletePin(Guid pinspaceId, Pin pin)
        {
            var pinDataFileName = GetPinDataFileName(pinspaceId, pin.Id);
            if (File.Exists(pinDataFileName))
            {
                File.Delete(pinDataFileName);
            }
            var pinDataPath = GetPinDataPath(pinspaceId, pin.Id);
            if (Directory.Exists(pinDataPath))
            {
                Directory.Delete(pinDataPath, true);
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public IList<Pin> GetPins(Guid pinspaceId)
        {
            var list = new List<Pin>();
            var pinspacePath = GetPinspacePath(pinspaceId);
            if (!Directory.Exists(pinspacePath))
            {
                throw new Exception($"Pinspace not found: {pinspaceId}");
            }
            var options = new EnumerationOptions
            {
                IgnoreInaccessible = true,
                MatchCasing = MatchCasing.CaseInsensitive,
                MatchType = MatchType.Simple,
                RecurseSubdirectories = false
            };
            var pinFilenames = Directory.GetFiles(pinspacePath, "pin_*.json", options);
            foreach (var pinFilename in pinFilenames)
            {
                if (!Guid.TryParse(Path.GetFileNameWithoutExtension(pinFilename).Replace("pin_", ""), out var pinId))
                {
                    continue;
                }
                try
                {
                    list.Add(LoadJsonFile<Pin>(pinFilename));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error loading data for pin {pinId} in pinspace {pinspaceId}", pinId, pinspaceId);
                    list.Add(new ErrorPin { Id = pinId, ErrorMessage = $"An exception occurred while loading pin {pinId}: {ex}", Title = "Pin with errors" });
                }
            }
            return list;
        }

        public Pinspace GetPinspace(Guid id)
        {
            var pinspace = data.Pinspaces.FirstOrDefault(p => p.Id.Equals(id));
            return pinspace?.Clone();
        }

        public IList<Pinspace> GetPinspaces()
        {
            return data.Pinspaces.Clone();
        }

        public IList<PinWindow> GetPinWindows()
        {
            return data.Windows.Clone();
        }

        public bool RetrievePinData(Guid pinspaceId, Guid pinId, string key, out byte[] data)
        {
            var filename = GetPinDataKeyFilePath(pinspaceId, pinId, key);
            if (File.Exists(filename))
            {
                data = File.ReadAllBytes(filename);
                return true;
            }
            data = null;
            return false;
        }

        public void StorePinData(Guid pinspaceId, Guid pinId, string key, byte[] data)
        {
            var filename = GetPinDataKeyFilePath(pinspaceId, pinId, key);
            Directory.CreateDirectory(Path.GetDirectoryName(filename));
            File.WriteAllBytes(filename, data);
        }

        public void UpdatePin(Guid pinspaceId, Pin pin)
        {
            pendingPinChanges.Enqueue((pinspaceId, pin.Clone()));
            delayedSavePinChangesAction.Execute();
        }

        public void UpdatePinspace(Pinspace pinspace)
        {
            var storedPin = data.Pinspaces.FirstOrDefault(p => p.Id.Equals(pinspace.Id));
            if (storedPin == null)
            {
                storedPin = new Pinspace();
                data.Pinspaces.Add(storedPin);
            }
            storedPin.Assign(pinspace, out var wasChanged);
            if (wasChanged)
            {
                delayedSaveDataFileAction.Execute();
            }
        }

        public void UpdatePinWindow(PinWindow pinWindow)
        {
            var storedPinWindow = data.Windows.FirstOrDefault(w => w.Id.Equals(pinWindow.Id));
            if (storedPinWindow == null)
            {
                storedPinWindow = new PinWindow();
                data.Windows.Add(storedPinWindow);
            }
            storedPinWindow.Assign(pinWindow, out var wasChanged);
            if (wasChanged)
            {
                delayedSaveDataFileAction.Execute();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    delayedSaveDataFileAction.Stop();
                    delayedSavePinChangesAction.Stop();
                }
                disposedValue = true;
            }
        }

        private string GetDataFilename()
        {
            return Path.Combine(GetDataPath(), "Pinspaces.json");
        }

        private string GetDataPath()
        {
            return options.Value.LocalApplicationDataFolderPath;
        }

        private string GetPinDataFileName(Guid pinspaceId, Guid pinId)
        {
            return Path.Combine(GetPinspacePath(pinspaceId), $"pin_{pinId}.json");
        }

        private string GetPinDataKeyFilePath(Guid pinspaceId, Guid pinId, string key)
        {
            return Path.Combine(GetPinDataPath(pinspaceId, pinId), key);
        }

        private string GetPinDataPath(Guid pinspaceId, Guid pinId)
        {
            return Path.Combine(GetPinspacePath(pinspaceId), $"pin_{pinId}");
        }

        private string GetPinspacePath(Guid pinspaceId)
        {
            var pinspacePath = Path.Combine(GetDataPath(), $"pinspace-{pinspaceId}");
            Directory.CreateDirectory(pinspacePath);
            return pinspacePath;
        }

        private JsonData LoadDataFile()
        {
            var dataFilename = GetDataFilename();
            return !File.Exists(dataFilename) ? new JsonData() : LoadJsonFile<JsonData>(dataFilename);
        }

        private T LoadJsonFile<T>(string filename)
        {
            var fileContents = File.ReadAllText(filename);
            var deserializeOptions = new JsonSerializerOptions();
            deserializeOptions.Converters.Add(pinJsonConverter);
            return JsonSerializer.Deserialize<T>(fileContents, deserializeOptions);
        }

        private void SaveDataChanges()
        {
            var dataFilename = GetDataFilename();
            Directory.CreateDirectory(Path.GetDirectoryName(dataFilename));
            SaveJsonFile(dataFilename, data);
        }

        private void SaveJsonFile(string filename, object data)
        {
            using var fileStream = new FileStream(filename, FileMode.Create);
            var options = new JsonWriterOptions
            {
                Indented = true
            };
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(pinJsonConverter);
            var writer = new Utf8JsonWriter(fileStream, options);
            JsonSerializer.Serialize(writer, data, serializeOptions);
        }

        private void SavePinChanges()
        {
            while (pendingPinChanges.TryDequeue(out var change))
            {
                var pinDataFileName = GetPinDataFileName(change.pinspaceId, change.pin.Id);
                SaveJsonFile(pinDataFileName, change.pin);
            }
        }
    }
}
