using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Tasks
{
    public class TradeDayProcessor
    {
        private readonly int numConsumers;
        private readonly string tradeFile;
        private readonly Predicate<TradeDay> test;

        public TradeDayProcessor(int numConsumers, string tradeFile, Predicate<TradeDay> test)
        {
            this.numConsumers = numConsumers;
            this.tradeFile = tradeFile;
            this.test = test;
        }

        public TradeDayProcessor(int numConsumers, Predicate<TradeDay> test)
            : this(numConsumers, ResolveDefaultDataFile(), test)
        {
        }

        private static string ResolveDefaultDataFile()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string candidate = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "DowJones.csv"));
            if (!File.Exists(candidate))
            {
                throw new FileNotFoundException("Could not locate DowJones.csv using resolved path.", candidate);
            }
            return candidate;
        }

        public void Start()
        {
        }

        public void Start(System.Threading.CancellationToken cancellationToken)
        {
            Start();
        }

        public int GetMatchingCount()
        {
            throw new NotImplementedException("GetMatchingCount not yet implemented – complete Step 3 of the lab.");
        }

        public void Cancel()
        {
            throw new NotImplementedException("Cancellation not yet implemented – complete Step 4 of the lab.");
        }

        internal IEnumerable<TradeDay> ReadStockData()
        {
            using (var sr = new StreamReader(tradeFile))
            {
                string line = null;
                sr.ReadLine();
                while ((line = sr.ReadLine()) != null)
                {
                    yield return ParseTradeEntry(line);
                }
            }
        }

        private static TradeDay ParseTradeEntry(string entry)
        {
            string[] items = entry.Split(',');
            var ret = new TradeDay
            {
                Date = DateTime.Parse(items[0]),
                Open = double.Parse(items[1], NumberFormatInfo.InvariantInfo),
                High = double.Parse(items[2], NumberFormatInfo.InvariantInfo),
                Low = double.Parse(items[3], NumberFormatInfo.InvariantInfo),
                Close = double.Parse(items[4], NumberFormatInfo.InvariantInfo),
                Volume = long.Parse(items[5], NumberFormatInfo.InvariantInfo),
                AdjClose = double.Parse(items[6], NumberFormatInfo.InvariantInfo)
            };
            return ret;
        }
    }
}
