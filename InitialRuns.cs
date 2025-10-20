namespace PolyphaseSorting
{
    public static class InitialRuns
    {
        public static List<string> CreateSortedRuns(string inputPath, string runDir, int blockSize)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n[Phase 1] Creating sorted runs..");
            Console.ResetColor();
            var runs = new List<string>();
            using var sr = new StreamReader(inputPath);
            var buffer = new List<Record>(blockSize);
            string? line;
            var runIndex = 0;

            while ((line = sr.ReadLine()) != null)
            {
                buffer.Add(Record.Parse(line));
                if (buffer.Count >= blockSize)
                    runs.Add(FlushRun(runDir, ref runIndex, buffer));
            }

            if (buffer.Count > 0)
                runs.Add(FlushRun(runDir, ref runIndex, buffer));
            return runs;
        }

        private static string FlushRun(string dir, ref int index, List<Record> buffer)
        {
            buffer.Sort(RecordComparer.Instance);
            var path = Path.Combine(dir, $"run_{index++:000}.tmp");

            var lines = buffer.Select(r => r.ToString()).ToList();
            lines.Add("###END_OF_RUN###"); // Add a run separator

            File.WriteAllLines(path, buffer.Select(r => r.ToString()));
            buffer.Clear();
            return path;
        }
    }
}