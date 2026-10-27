using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Purchasing.Core.Services
{
    internal sealed class BackfillCheckpoint
    {
        [JsonProperty(Required = Required.Always)]
        public int Version { get; set; } = 1;
        [JsonProperty(Required = Required.Always)]
        public string Source { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string Target { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string Schema { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string Selection { get; set; }
        [JsonProperty(Required = Required.Always)]
        public int BatchSize { get; set; }
        [JsonProperty(Required = Required.Always)]
        public int HighWatermark { get; set; }
        [JsonProperty(Required = Required.Always)]
        public int LastProcessedId { get; set; }
        [JsonProperty(Required = Required.Always)]
        public bool OrdersComplete { get; set; }
        [JsonProperty(Required = Required.Always)]
        public List<int[]> Pending { get; set; } = new();
        [JsonProperty(Required = Required.Always)]
        public Dictionary<Indexes, LookupCheckpoint> Lookups { get; set; } = SearchBackfillRunner.LookupIndexes.ToDictionary(i => i, _ => new LookupCheckpoint());

        internal void Validate(BackfillCheckpoint expected)
        {
            if (Version != 1 || Source != expected.Source || Target != expected.Target || Schema != expected.Schema ||
                Selection != expected.Selection || BatchSize != expected.BatchSize)
                throw new BackfillSafetyException("Checkpoint identity differs from this source, destination, selection, batch size, or projection build. Use the original settings/build or a new checkpoint.");
            if (HighWatermark < 0 || LastProcessedId < 0 || LastProcessedId > HighWatermark || Pending == null || Pending.Count > 100 ||
                Pending.Any(ids => ids == null || ids.Length == 0 || ids.Length > BatchSize || ids.Any(id => id <= 0 || id > LastProcessedId) ||
                    !ids.SequenceEqual(ids.Distinct().OrderBy(id => id))) ||
                Pending.SelectMany(ids => ids).Distinct().Count() != Pending.Sum(ids => ids.Length) ||
                Lookups == null || !Lookups.Keys.OrderBy(i => i).SequenceEqual(SearchBackfillRunner.LookupIndexes.OrderBy(i => i)) ||
                Lookups.Values.Any(l => l == null || l.AfterId?.Length > 512 || (l.Complete && !l.Copied)))
                throw new BackfillSafetyException("Checkpoint is invalid or incomplete. Restore an intact checkpoint or start a new run.");
        }
    }

    internal sealed class LookupCheckpoint
    {
        [JsonProperty(Required = Required.AllowNull)]
        public string AfterId { get; set; }
        [JsonProperty(Required = Required.Always)]
        public bool Copied { get; set; }
        [JsonProperty(Required = Required.Always)]
        public bool Complete { get; set; }
    }

    internal interface IBackfillCheckpointStore
    {
        bool Exists { get; }
        BackfillCheckpoint Load();
        void Save(BackfillCheckpoint checkpoint);
    }

    internal sealed class FileBackfillCheckpointStore : IBackfillCheckpointStore
    {
        private readonly string _path;
        public FileBackfillCheckpointStore(string path) { _path = Path.GetFullPath(path); }
        public bool Exists => File.Exists(_path);
        public BackfillCheckpoint Load()
        {
            if (new FileInfo(_path).Length > 2 * 1024 * 1024) throw new BackfillSafetyException("Checkpoint exceeds the supported size.");
            return JsonConvert.DeserializeObject<BackfillCheckpoint>(File.ReadAllText(_path),
                new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error })
                ?? throw new BackfillSafetyException("Checkpoint is empty.");
        }
        public void Save(BackfillCheckpoint checkpoint)
        {
            var temporary = _path + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                using (var stream = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(checkpoint, Formatting.Indented));
                    stream.Write(bytes);
                    stream.Flush(flushToDisk: true);
                }
                // Same-directory rename is atomic. A crash before it leaves the previous checkpoint replayable.
                File.Move(temporary, _path, overwrite: true);
            }
            finally { if (File.Exists(temporary)) File.Delete(temporary); }
        }
    }
}
