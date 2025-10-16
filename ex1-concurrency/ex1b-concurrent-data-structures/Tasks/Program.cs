using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Tasks
{
    class Program
    {
        static void Main()
        {
            const int NUM_CONSUMERS = 2;

            string relativeDataPath = Path.Combine("..", "..", "..", "..", "DowJones.csv");
            string dataFilePath = Path.GetFullPath(relativeDataPath);
            Console.WriteLine("Using data file: {0}", dataFilePath);
            if (!File.Exists(dataFilePath))
            {
                Console.Error.WriteLine("Data file not found at: {0}", dataFilePath);
                return;
            }

            var processor = new TradeDayProcessor(NUM_CONSUMERS,
                                                  dataFilePath,
                                                  day => (day.Close - day.Open) / day.Open > 0.05);

            Stopwatch sw = Stopwatch.StartNew();

            processor.Start();

            Console.WriteLine("Processing started (lab starter state). Complete Steps 1-4 to finish implementation.");
            Console.WriteLine("Elapsed so far {0}", sw.Elapsed);
        }
    }
}
