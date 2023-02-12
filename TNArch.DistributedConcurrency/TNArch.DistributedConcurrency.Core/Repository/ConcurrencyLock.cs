using Azure;
using Azure.Data.Tables;

namespace TNArch.DistributedConcurrency.Core.Repository
{
    public class ConcurrencyLock : ITableEntity
    {
        public DateTime LockExpiration { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public string UnlockKey { get; set; }
    }
}