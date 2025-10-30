namespace PolyphaseSorting
{
    public static class InitialRuns
    {
        // Node with generation tag for replacement selection
        private sealed class Node
        {
            public Record Rec;
            public int Gen;
            public Node(Record r, int g) { Rec = r; Gen = g; }
        }

        // Simple binary min-heap by (Gen, Rec)
        private sealed class MinHeap
        {
            private readonly List<Node> a = new();
            private static readonly RecordComparer rc = RecordComparer.Instance;
            public int Count => a.Count;

            private static int Cmp(Node x, Node y)
            {
                int c = x.Gen.CompareTo(y.Gen);
                return c != 0 ? c : rc.Compare(x.Rec, y.Rec);
            }

            public void Push(Node x)
            {
                a.Add(x);
                int i = a.Count - 1;
                while (i > 0)
                {
                    int p = (i - 1) >> 1;
                    if (Cmp(a[i], a[p]) >= 0) break;
                    (a[i], a[p]) = (a[p], a[i]);
                    i = p;
                }
            }

            public Node Pop()
            {
                var root = a[0];
                var last = a[^1];
                a.RemoveAt(a.Count - 1);
                if (a.Count > 0)
                {
                    a[0] = last;
                    int i = 0;
                    while (true)
                    {
                        int l = i * 2 + 1, r = l + 1, m = i;
                        if (l < a.Count && Cmp(a[l], a[m]) < 0) m = l;
                        if (r < a.Count && Cmp(a[r], a[m]) < 0) m = r;
                        if (m == i) break;
                        (a[i], a[m]) = (a[m], a[i]);
                        i = m;
                    }
                }
                return root;
            }
        }

        public static List<string> CreateRuns(string inputPath, string runDir, int blockSize)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n[Phase 1] Creating runs..");
            Console.ResetColor();

            Directory.CreateDirectory(runDir);
            var runs = new List<string>();
            using var sr = new StreamReader(inputPath);

            var heap = new MinHeap();
            int gen = 0;
            int runIndex = 0;
            string? currentPath = null;
            StreamWriter? writer = null;

            // Seed heap with up to blockSize records
            for (int i = 0; i < blockSize; i++)
            {
                var line = sr.ReadLine();
                if (line is null) break;
                heap.Push(new Node(Record.Parse(line), gen));
            }

            if (heap.Count == 0)
                return runs;

            // Start first run
            currentPath = Path.Combine(runDir, $"run_{runIndex++:000}.tmp");
            writer = new StreamWriter(currentPath);

            while (heap.Count > 0)
            {
                var node = heap.Pop();
                if (node.Gen > gen)
                {
                    writer.Dispose();
                    runs.Add(currentPath!);
                    gen = node.Gen;
                    currentPath = Path.Combine(runDir, $"run_{runIndex++:000}.tmp");
                    writer = new StreamWriter(currentPath);
                }

                writer.WriteLine(node.Rec.ToString());

                var nextLine = sr.ReadLine();
                if (nextLine != null)
                {
                    var nextRec = Record.Parse(nextLine);
                    int nextGen = RecordComparer.Instance.Compare(nextRec, node.Rec) >= 0 ? gen : gen + 1;
                    heap.Push(new Node(nextRec, nextGen));
                }
            }

            writer.Dispose();
            runs.Add(currentPath!);

            return runs;
        }
    }
}
