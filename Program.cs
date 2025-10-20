using System.Diagnostics;
namespace PolyphaseSorting
{
    public class Program
    {
        static void Main(string[] args)
        {
            // string file = "File_1000.txt";
            // GenerateFile.GenerateFile1GB(file);

                const string input = "File_1000.txt";

                var watch = Stopwatch.StartNew();
                var process = Process.GetCurrentProcess();
                long totalMemory = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;

                var blockSize = 2_500_000;
                const string runDir = "Runs";
                Directory.CreateDirectory(runDir);
                long usedMemory = process.WorkingSet64;
                double usageRatio = (double)usedMemory / totalMemory;
                if (usageRatio > 0.9)
                {
                    blockSize /= 2;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Memory usage high. Reducing block size to {blockSize}.");
                    Console.ResetColor();
                }
                else if (usageRatio < 0.6)
                {
                    blockSize = Math.Min(blockSize * 2, 5000000);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Memory usage low. Increasing block size to {blockSize}.");
                    Console.ResetColor();
                }


                var runs = InitialRuns.CreateSortedRuns(input, runDir, blockSize);

                var polySorting = new PolyphaseSorting(6);

                const string outputFilePath = "SortedFile.txt";
                polySorting.PolyphaseSort(runs, outputFilePath);

                watch.Stop();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Time taken for sorting: {watch.ElapsedMilliseconds} ms");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Sorting completed. Sorted file created: " + outputFilePath);
                Console.ResetColor();
        }
    }
}