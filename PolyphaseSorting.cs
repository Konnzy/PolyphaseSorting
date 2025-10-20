namespace PolyphaseSorting
{
    public class PolyphaseSorting
    {
        private readonly int n;
        private readonly Tape[] f;

        private int j;

        // Add a field to track run lengths
        private Dictionary<int, Queue<int>> runLengths = new();

        // Initialize an array of tapes
        public PolyphaseSorting(int nTapes)
        {
            if (nTapes < 3)
                throw new ArgumentException("Need at least 3 tapes.");
            n = nTapes;
            Directory.CreateDirectory("Tapes");
            f = Enumerable.Range(0, n).Select(i => new Tape(Path.Combine("Tapes", $"tape_{i}.tmp"))).ToArray();
        }

        // Choose the next tape for writing a run according to fake-run logic
        private void SelectTape(int[] a, int[] d, ref int level)
        {
            if (j + 1 < n && d[j] < d[j + 1])
            {
                j++;
                if (j >= n - 1) j = 0;
            }
            else
            {
                if (d[j] == 0)
                {
                    level++;
                    int z = a[0];
                    for (int i = 0; i < n - 1; i++)
                    {
                        d[i] = z + a[i + 1] - a[i];
                        a[i] = z + a[i + 1];
                    }
                }

                j = 0;
            }

            d[j]--;
        }

        // Calculate ideal run distribution (a[]) and fake runs (d[]) using Fibonacci numbers
        private void ComputeFibDistribution(int totalRuns, int[] a, int[] d)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n[Phase 2] Computing run Fibonacci distribution..");
            Console.ResetColor();

            List<int> F = new() { 1, 1 };
            while (F[^1] < totalRuns) F.Add(F[^1] + F[^2]);

            int k = F.Count - 1;
            a[n - 1] = 0;
            for (int i = n - 2; i >= 0; i--)
            {
                int val = k >= 0 ? F[k] : 0;
                a[i] = val;
                k--;
            }

            Array.Copy(a, d, n);
            int sumA = a.Sum();
            if (sumA < totalRuns)
            {
                int need = totalRuns - sumA;
                for (int i = 0; i < n - 1 && need > 0; i++)
                {
                    a[i] += 1;
                    d[i] += 1;
                    need--;
                }
            }
            else if (sumA > totalRuns)
            {
                int diff = sumA - totalRuns;
                for (int i = n - 2; i >= 0 && diff > 0; i--)
                {
                    if (a[i] > 0)
                    {
                        a[i] -= 1;
                        d[i] -= 1;
                        diff--;
                    }
                }
            }

            a[n - 1] = 0;
            d[n - 1] = 0;
        }


        // Distribute initial runs from f0 among working tapes
        private void DistributeInitialRuns(List<string> runs, int[] a, int[] d, ref int level)
        {
            ComputeFibDistribution(runs.Count, a, d);

            level = 1;
            j = 0;

            for (int i = 0; i < n; i++)
            {
                RewriteTape(i);
                runLengths[i] = new Queue<int>();
            }

            var idx = 0;
            while (idx < runs.Count)
            {
                SelectTape(a, d, ref level);

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"\nRun[{idx}] created");
                Console.ResetColor();

                var runLength = File.ReadLines(runs[idx]).Count();
                runLengths[j].Enqueue(runLength);

                AppendFile(f[j].Path, runs[idx++]);
            }
        }
        // Merge runs from tapes ta[] into a single run on tape outIdx
        //ta[] is an array of tape indices
        private void Merge(int[] ta, int k, int outIdx)
        {
            var readers = new StreamReader[k];
            var head = new Record?[k];
            var remaining = new int[k]; // Track remaining records in current run

            for (int i = 0; i < k; i++)
            {
                readers[i] = new StreamReader(f[ta[i]].Path);

                // Get the length of the first run from this tape
                if (runLengths[ta[i]].Count > 0)
                {
                    remaining[i] = runLengths[ta[i]].Dequeue();
                    head[i] = ReadNext(readers[i]);
                    if (head[i] != null) remaining[i]--;
                }
                else
                {
                    remaining[i] = 0;
                    head[i] = null;
                }
            }

            using var writer = new StreamWriter(f[outIdx].Path, append: true);
            int outputRunLength = 0;

            while (true)
            {
                int pick = -1;
                Record? best = null;

                // Find the smallest record among active tapes
                for (int i = 0; i < k; i++)
                {
                    if (head[i] == null || remaining[i] < 0)
                        continue;

                    if (best == null || RecordComparer.Instance.Compare(head[i], best) < 0)
                    {
                        best = head[i];
                        pick = i;
                    }
                }

                if (pick == -1)
                    break;

                // Write the selected record
                writer.WriteLine(best!.ToString());
                outputRunLength++;

                // Read the next record from the selected tape
                head[pick] = ReadNext(readers[pick]);
                if (head[pick] != null)
                {
                    remaining[pick]--;
                }
                else
                {
                    // End of the current run, try to load the next run
                    if (runLengths[ta[pick]].Count > 0)
                    {
                        remaining[pick] = runLengths[ta[pick]].Dequeue();
                        head[pick] = ReadNext(readers[pick]);
                        if (head[pick] != null) remaining[pick]--;
                    }
                    else
                    {
                        remaining[pick] = -1;
                    }
                }
            }

            // Track the length of the output run
            if (!runLengths.ContainsKey(outIdx))
                runLengths[outIdx] = new Queue<int>();
            runLengths[outIdx].Enqueue(outputRunLength);

            foreach (var r in readers) r.Dispose();
        }

        // Perform a complete polyphase sort and produce a final output file
        public void PolyphaseSort(List<string> initialRuns, string outputFile = "SortedFile.txt")
        {
            int[] a = new int[n];
            int[] d = new int[n];
            // level is used to track the current level of the Fibonacci distribution
            int level = 0;
            // Initialize run lengths tracking
            for (int i = 0; i < n; i++)
            {
                runLengths[i] = new Queue<int>();
            }

            DistributeInitialRuns(initialRuns, a, d, ref level);

            int[] t = Enumerable.Range(0, n).ToArray();
            int[] ta = new int[n];

            var iterationCount = 0;
            while (true)
            {
                iterationCount++;
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"\nIteration {iterationCount}: Runs on tapes: {string.Join(", ", t.Select(i => runLengths[i].Count))}");
                Console.ResetColor();

                // Check if only one tape has runs remaining
                int tapesWithRuns = 0;
                int lastTapeWithRuns = -1;
                for (int i = 0; i < n; i++)
                {
                    if (runLengths[t[i]].Count > 0)
                    {
                        tapesWithRuns++;
                        lastTapeWithRuns = t[i];
                    }
                }

                if (tapesWithRuns == 1 && runLengths[lastTapeWithRuns].Count == 1)
                {
                    // Only one run left - we're done!
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nSorting complete! Final run on tape {lastTapeWithRuns}\n");
                    Console.ResetColor();
                    File.Copy(f[lastTapeWithRuns].Path, outputFile, true);
                    break;
                }

                if (tapesWithRuns <= 1 && runLengths[lastTapeWithRuns].Count <= 1)
                {
                    // Shouldn't happen, but safety check
                    if (lastTapeWithRuns >= 0)
                    {
                        File.Copy(f[lastTapeWithRuns].Path, outputFile, true);
                    }

                    break;
                }

                // Find the minimum number of runs on input tapes
                int minRuns = int.MaxValue;
                for (int i = 0; i < n - 1; i++)
                {
                    if (runLengths[t[i]].Count > 0)
                    {
                        minRuns = Math.Min(minRuns, runLengths[t[i]].Count);
                    }
                }

                if (minRuns == int.MaxValue || minRuns == 0)
                    break;

                // Clear output tape
                RewriteTape(t[n - 1]);
                if (!runLengths.ContainsKey(t[n - 1]))
                    runLengths[t[n - 1]] = new Queue<int>();
                else
                    runLengths[t[n - 1]].Clear();
                
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n[Phase 3] Merging runs..");
                Console.ResetColor();
                
                // Perform merges
                for (int mergeNum = 0; mergeNum < minRuns; mergeNum++)
                {
                    // Determine which tapes to merge
                    int k = 0;
                    for (int i = 0; i < n - 1; i++)
                    {
                        if (runLengths[t[i]].Count > 0)
                        {
                            ta[k++] = t[i];
                        }
                    }

                    if (k > 0)
                    {
                        Merge(ta, k, t[n - 1]);
                    }

                    GC.Collect();
                    Thread.Sleep(10);
                }

                // Rotate tapes
                int temp = t[n - 1];
                for (int i = n - 1; i > 0; i--)
                {
                    t[i] = t[i - 1];
                }

                t[0] = temp;
            }
        }
        // Create a new tape file and rewrite it to clear it
        private void RewriteTape(int tapeIndex)
        {
            var path = f[tapeIndex].Path;
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            using var _ = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 1 << 20);
        }
        // Append a file to another file
        private static void AppendFile(string dst, string src)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(dst)!);
            using var w = new FileStream(dst, FileMode.Append, FileAccess.Write, FileShare.None, 1 << 20);
            using var r = new FileStream(src, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 20);
            r.CopyTo(w);
        }
        // Read the next record from a file
        private static Record? ReadNext(StreamReader r)
        {
            var line = r.ReadLine();
            return line is null ? null : Record.Parse(line);
        }
    }
}