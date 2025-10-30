namespace PolyphaseSorting
{
    public class Tape
    {
        public string Path { get; }
        private StreamReader? reader;
        private StreamWriter? writer;

        public Tape(string path) => Path = path;
    }
}
