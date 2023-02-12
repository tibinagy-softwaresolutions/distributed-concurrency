using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNArch.DistributedConcurrency.Core
{
    public static class TableExtensions
    {
        public static async Task<List<TElement>> ToListAsync<TElement>(this AsyncPageable<TElement> query, int maxPageCount = int.MaxValue) where TElement : ITableEntity, new()
        {
            var items = new List<TElement>();

            await foreach (var item in query)
            {
                items.Add(item);
            }
            return items;
        }

        public static TableTransactionAction ToDeleteOperation<TElement>(this TElement entity) where TElement : ITableEntity
        {
            entity.ETag = ETag.All;

            return new TableTransactionAction(TableTransactionActionType.Delete, entity);
        }

        public static TableTransactionAction ToAddOperation<TElement>(this TElement entity) where TElement : ITableEntity
        {            
            return new TableTransactionAction(TableTransactionActionType.Add, entity);
        }

        public static TableTransactionAction ToUpdateOperation<TElement>(this TElement entity) where TElement : ITableEntity
        {
            return new TableTransactionAction(TableTransactionActionType.UpdateMerge, entity);
        }
    }
}
