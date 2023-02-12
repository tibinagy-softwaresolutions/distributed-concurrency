using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TNArch.DistributedConcurrency.Core;

namespace TNArch.DistributedConcurrency.Demo
{
    public class DemoHostedService : IHostedService
    {
        private readonly IConcurrencyLockService _concurrencyLockService;
        private readonly ILogger<DemoHostedService> _logger;
        public DemoHostedService(IConcurrencyLockService concurrencyLockService, ILogger<DemoHostedService> logger)
        {
            _concurrencyLockService = concurrencyLockService;
            _logger = logger;
            _random = new Random();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var itemId = Guid.NewGuid();
            await DeactivateItem(itemId);
            await PurchaseItem(itemId);
            await DeactivateItem(itemId);

            Thread.Sleep(TimeSpan.FromSeconds(2));

            await PurchaseItem(itemId);
            await DeactivateItem(itemId);           
        }

        public async Task DeactivateItem(Guid itemId)
        {
            //Read open orders with the given item
            //If there are open orders, end the execution

            var lockKey = $"LockNoOrderWithInactivetem|{itemId:N}";

            var unlockKey = $"DeactivateItem|{itemId:N}";

            var lockDuraction = TimeSpan.FromSeconds(2);

            if (!await _concurrencyLockService.TryLock(lockKey, unlockKey, lockDuraction))
            {
                _logger.LogWarning($"Cannot deactivate item {itemId} because of NoOrderWithInactivetem rule!");
                return;
            }

            //deactivate item
            _logger.LogInformation($"Item {itemId} deactivated.");
        }

        public async Task PurchaseItem(Guid itemId)
        {
            //Read the item from DB
            //If the item is deactivated, end the execution

            var lockKey = $"LockNoOrderWithInactivetem|{itemId:N}";

            var unlockKey = $"PurchaseItem|{itemId:N}";

            if (!await _concurrencyLockService.TryLock(lockKey, unlockKey))
            {
                _logger.LogWarning($"Cannot purchase item {itemId} because of NoOrderWithInactivetem rule!");
                return;
            }

            //purchase item
            _logger.LogInformation($"Item {itemId} purchased.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}