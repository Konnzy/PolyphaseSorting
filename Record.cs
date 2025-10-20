namespace PolyphaseSorting
{
    public class Record
    {
        public char Key { get; set; }
        public string? Data { get; set; }

        public string? PhoneNumber { get; set; }

        // Parse a line from text into a Record object
        public static Record Parse(string line)
        {
            var parts = line.Split('-');
            return new Record
            {
                Key = parts[0][0],
                Data = parts.ElementAtOrDefault(1),
                PhoneNumber = parts.ElementAtOrDefault(2)
            };
        }

        // Convert record back to text format
        public override string ToString()
        {
            return $"{Key}-{Data}-{PhoneNumber}";
        }
    }

    public sealed class RecordComparer : IComparer<Record>
    {
        public static readonly RecordComparer Instance = new();

        public int Compare(Record? x, Record? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return -1;
            if (y is null) return 1;
            int c = x.Key.CompareTo(y.Key);
            if (c != 0) return c;
            c = string.CompareOrdinal(x.Data, y.Data);
            if (c != 0) return c;
            return string.CompareOrdinal(x.PhoneNumber, y.PhoneNumber);
        }
    }
}