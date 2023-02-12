namespace TNArch.DistributedConcurrency.Core.Repository
{
    public interface IConcurrencyLockRepository
    {
        Task<bool> AddLock(string lockKey, string unlockKey, DateTime lockExpiration);
    }
}