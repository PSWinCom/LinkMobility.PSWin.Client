using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkMobility.PSWin.Client.Extensions
{
    internal static class EnumerableBatchExtension
    {
        public static IEnumerable<T[]> Batch<T>(this IEnumerable<T> self, uint batchSize)
        {
            if (batchSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(batchSize), batchSize, "Batch size must be greater than zero.");

            var enumerator = self.GetEnumerator();

            IEnumerable<T> GetBatch()
            {
                yield return enumerator.Current;
                for (var i = 1; i < batchSize; i++)
                {
                    if (!enumerator.MoveNext())
                        break;
                    yield return enumerator.Current;
                }
            }

            while (enumerator.MoveNext())
            {
                yield return GetBatch().ToArray();
            }
        }
    }
}
