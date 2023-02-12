using Azure;
using Azure.Data.Tables;

namespace TNArch.DistributedConcurrency.Core.Repository
{
    public class ConcurrencyLockRepository : IConcurrencyLockRepository
    {
        private readonly TableClient _tableClient;

        public ConcurrencyLockRepository(string connectionString)
        {
            _tableClient = new TableClient(connectionString, nameof(ConcurrencyLock));
        }

        public async Task<bool> AddLock(string lockKey, string unlockKey, DateTime lockExpiration)
        {
            var locks = await _tableClient.QueryAsync<ConcurrencyLock>(p => p.PartitionKey == "Lock" && p.RowKey == lockKey).ToListAsync();

            var concurrencyLock = locks.FirstOrDefault();

            if (concurrencyLock?.LockExpiration > DateTime.UtcNow && concurrencyLock.UnlockKey != unlockKey)
                return false;

            try
            {
                if (concurrencyLock != null)
                {
                    concurrencyLock.LockExpiration = lockExpiration;
                    concurrencyLock.UnlockKey = unlockKey;
                    await _tableClient.UpdateEntityAsync(concurrencyLock, concurrencyLock.ETag);
                }
                else
                {
                    concurrencyLock = new ConcurrencyLock { PartitionKey = "Lock", RowKey = lockKey, LockExpiration = lockExpiration, UnlockKey = unlockKey };
                    await _tableClient.AddEntityAsync(concurrencyLock);
                }

                return true;
            }
            catch (RequestFailedException ex) when (ex.Status == 409 || ex.Status == 412)
            {
                return false;
            }
        }
    }
}