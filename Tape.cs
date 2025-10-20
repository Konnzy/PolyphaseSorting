namespace PolyphaseSorting
{
    public class Tape
    {
        public string Path { get; }
        private StreamReader? reader;
        private StreamWriter? writer;

        public Tape(string path) => Path = path;

        public void OpenForWrite()
        {
            Close();
            writer = new StreamWriter(new FileStream(Path, FileMode.Create, FileAccess.Write));
        }

        public void OpenForRead()
        {
            Close();
            reader = new StreamReader(new FileStream(Path, FileMode.Open, FileAccess.Read));
        }

        public void WriteRecord(Record r) => writer?.WriteLine(r.ToString());

        public Record? ReadRecord()
        {
            string? line = reader?.ReadLine();
            return line == null ? null : Record.Parse(line);
        }

        public bool Eot => reader?.EndOfStream ?? true;

        private void Close()
        {
            writer?.Close();
            reader?.Close();
        }
    }
}