using LinkMobility.PSWin.Client.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkMobility.PSWin.Client.Tests
{
    [TestFixture]
    public class EnumerableBatchExtensionTests
    {
        [Test]
        public void Test_zero_batch_size()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new int[2].Batch(0).ToArray());
        }

        [Test]
        public void Test_MultipleEnumerationChecker()
        {
            var checker = new MultipleEnumerationChecker();
            var items = checker.GetItems(10);
            Assume.That(items.Count(), Is.EqualTo(items.Count()));
            Assert.That(checker.Enumerations, Is.EqualTo(2), "Detected enumerations");
        }

        [Test]
        public void Test_no_multiple_enumeration()
        {
            var checker = new MultipleEnumerationChecker();
            var items = checker.GetItems(30);

            items.Batch(10).Count();

            Assert.That(checker.Enumerations, Is.EqualTo(1), "Enumerations");
        }

        [Test]
        public void Test_zero_batches()
        {
            var items = new int[0];
            var batch = items.Batch(10);
            Assert.That(batch, Is.Empty, "Number of batches");
        }

        [Test]
        [TestCase(1)]
        [TestCase(9)]
        [TestCase(10)]
        public void Test_one_batch(int itemCount)
        {
            var items = Enumerable.Range(0, itemCount).ToArray();
            var batches = items.Batch(10).ToArray();
            Assert.That(batches.Length, Is.EqualTo(1), "Number of batches");
            Assert.That(batches[0], Is.EqualTo(Enumerable.Range(0, itemCount)), "Batch content");
        }

        [Test]
        [TestCase(11)]
        [TestCase(19)]
        [TestCase(20)]
        public void Test_two_batches(int itemCount)
        {
            var items = Enumerable.Range(0, itemCount).ToArray();
            var batches = items.Batch(10).ToArray();
            Assert.That(batches.Length, Is.EqualTo(2), "Number of batches");
            Assert.That(batches[0], Is.EqualTo(Enumerable.Range(0, 10)), "First batch content");
            Assert.That(batches[1], Is.EqualTo(Enumerable.Range(10, itemCount - 10)), "Second batch content");
        }

        public class MultipleEnumerationChecker
        {
            public int Enumerations { get; private set; } = 0;

            public IEnumerable<int> GetItems(int count)
            {
                Enumerations++;
                for (var i = 0; i < count; i++)
                    yield return i;
            }
        }
    }
}