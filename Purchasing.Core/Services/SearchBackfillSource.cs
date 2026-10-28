using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using Newtonsoft.Json.Linq;

namespace Purchasing.Core.Services
{
    internal abstract class SearchProjection
    {
        public Indexes Index { get; }
        public abstract bool Succeeded { get; }
        public abstract IReadOnlyList<(string Id, JObject Source)> Documents { get; }
        public abstract void Write(IndexBackend backend, int[] orderIds = null);
        protected SearchProjection(Indexes index) { Index = index; }
    }

    internal sealed class SearchProjection<T> : SearchProjection where T : class
    {
        private readonly IReadOnlyList<T> _documents;
        public SearchProjection(Indexes index, IReadOnlyList<T> documents) : base(index) { _documents = documents; }
        public override bool Succeeded => _documents != null;
        public override IReadOnlyList<(string Id, JObject Source)> Documents =>
            _documents.Select(d => IndexBackend.SerializeDocument(d)).ToArray();
        public override void Write(IndexBackend backend, int[] orderIds = null)
        {
            if (!Succeeded) throw new BackfillSafetyException("Cannot write a failed SQL projection.");
            if (orderIds == null) backend.Write(Index, _documents);
            else backend.Reconcile(Index, orderIds, _documents);
        }
    }

    internal interface ISearchBackfillSource
    {
        string Identity { get; }
        string SchemaIdentity { get; }
        int HighWatermark(BackfillSelection selection);
        int[] NextOrders(BackfillSelection selection, int after, int through, int size);
        IReadOnlyList<SearchProjection> ReadOrders(int[] ids);
        SearchProjection ReadLookup(Indexes index, string after, int size, string[] ids = null);
        SearchProjection ReadExisting(Indexes index, string[] ids);
    }

    internal sealed class SqlSearchBackfillSource : ISearchBackfillSource
    {
        private readonly IDbService _db;
        private readonly SqlOrderIndexSource _orders;
        public string Identity { get; }
        public string SchemaIdentity { get; }
        private const string SelectionSql = "(@from IS NULL OR DateCreated >= @from) AND (@until IS NULL OR DateCreated < @until)";

        public SqlSearchBackfillSource(IDbService db)
        {
            _db = db;
            _orders = new SqlOrderIndexSource(db);
            using var connection = db.GetConnection();
            // Database creation time distinguishes a replaced TEST database on the same server.
            Identity = connection.QuerySingle<string>("SELECT CONCAT(CONVERT(nvarchar(128), SERVERPROPERTY('ServerName')), '/', DB_NAME(), '/', CONVERT(nvarchar(33), create_date, 126)) FROM sys.databases WHERE name = DB_NAME()");
            var objects = new[] { "udf_GetOrderHistoryForOrderIds", "vLineResults", "vCustomFieldResults", "vCommentResults", "vOrderTrackingIndex" };
            var definitions = objects.Select(name => connection.QuerySingleOrDefault<string>(
                "SELECT OBJECT_DEFINITION(OBJECT_ID(@name))", new { name = "dbo." + name })).ToArray();
            if (definitions.Any(string.IsNullOrWhiteSpace))
                throw new BackfillSafetyException("SQL projection definitions are unavailable. Grant VIEW DEFINITION for the five projection objects.");
            // The module ID rejects changed compiled SQL/serialization/mapping code as well as changed SQL views.
            SchemaIdentity = Hash(typeof(SqlSearchBackfillSource).Module.ModuleVersionId + "\n" + string.Join("\n", definitions));
        }

        internal static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();

        public int HighWatermark(BackfillSelection selection)
        {
            using var connection = _db.GetConnection();
            return connection.QuerySingle<int>($"SELECT COALESCE(MAX(Id), 0) FROM Orders WHERE {SelectionSql}",
                new { from = selection.From, until = selection.Until }, commandTimeout: 300);
        }

        public int[] NextOrders(BackfillSelection selection, int after, int through, int size)
        {
            using var connection = _db.GetConnection();
            return connection.Query<int>($"SELECT TOP (@size) Id FROM Orders WHERE Id > @after AND Id <= @through AND {SelectionSql} ORDER BY Id",
                new { size, after, through, from = selection.From, until = selection.Until }, commandTimeout: 300).ToArray();
        }

        public IReadOnlyList<SearchProjection> ReadOrders(int[] ids) => Projections(_orders.ReadBatch(ids));

        private static IReadOnlyList<SearchProjection> Projections(OrderIndexBatch batch) =>
            new SearchProjection[]
            {
                new SearchProjection<Queries.OrderHistory>(Indexes.OrderHistory, batch.History),
                new SearchProjection<Queries.SearchResults.LineResult>(Indexes.LineItems, batch.Lines),
                new SearchProjection<Queries.SearchResults.CustomFieldResult>(Indexes.CustomAnswers, batch.Answers),
                new SearchProjection<Queries.SearchResults.CommentResult>(Indexes.Comments, batch.Comments),
                new SearchProjection<OrderTrackingEntity>(Indexes.OrderTracking, batch.Tracking)
            };

        public SearchProjection ReadLookup(Indexes index, string after, int size, string[] ids = null) =>
            LookupIndexSource.Read(_db, index, after, size, ids);

        public SearchProjection ReadExisting(Indexes index, string[] ids)
        {
            if (SearchBackfillRunner.LookupIndexes.Contains(index)) return ReadLookup(index, null, ids.Length, ids);
            var numericIds = ids.Select(id => int.TryParse(id, NumberStyles.None, CultureInfo.InvariantCulture, out var value) && value > 0 ? value : 0)
                .Where(id => id > 0).Distinct().ToArray();
            // Check only this page's identities. Do not reload every child of every order for every target page.
            return Projections(_orders.ReadDocuments(index, numericIds)).Single(p => p.Index == index);
        }
    }

    public sealed class BackfillSelection
    {
        public DateTime? From { get; set; }
        public DateTime? Until { get; set; }
        public bool IsFull => From == null && Until == null;
        public void Validate()
        {
            if (From >= Until || From?.Kind == DateTimeKind.Utc || Until?.Kind == DateTimeKind.Utc ||
                From?.Kind == DateTimeKind.Local || Until?.Kind == DateTimeKind.Local)
                throw new ArgumentException("Use increasing, timezone-free Pacific wall-clock date boundaries, inclusive from and exclusive until.");
        }
        internal string Identity => $"{From?.ToString("O", CultureInfo.InvariantCulture) ?? "*"}/{Until?.ToString("O", CultureInfo.InvariantCulture) ?? "*"}";
    }
}
