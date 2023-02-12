using Microsoft.Extensions.Logging;
using System.Globalization;
using TNArch.DistributedConcurrency.Core.Repository;

namespace TNArch.DistributedConcurrency.Core
{

    public class ConcurrencyLockService : IConcurrencyLockService
    {
        private readonly IConcurrencyLockRepository _concurrencyLockRepository;
        private readonly ILogger<ConcurrencyLockService> _logger;

        public readonly TimeSpan DefaultLockDuration = TimeSpan.FromSeconds(30);

        public ConcurrencyLockService(IConcurrencyLockRepository concurrencyLockRepository, ILogger<ConcurrencyLockService> logger)
        {
            _concurrencyLockRepository = concurrencyLockRepository;
            _logger = logger;
        }

        public async Task<bool> TryLock(string lockKey, string unlockKey, TimeSpan? lockDuration = null)
        {
            lockDuration ??= DefaultLockDuration;

            var lockSucceed = await _concurrencyLockRepository.AddLock(lockKey, unlockKey, DateTime.UtcNow + lockDuration.Value);

            if (!lockSucceed)
                _logger.LogWarning($"Concurrency lock failed for lockKey {lockKey}!");

            return lockSucceed;
        }
    }
}