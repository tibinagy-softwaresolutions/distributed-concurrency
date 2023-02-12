namespace TNArch.DistributedConcurrency.Core
{
    public interface IConcurrencyLockService
    {
        Task<bool> TryLock(string lockKey, string unlockKey, TimeSpan? lockDuration = default);
    }
}