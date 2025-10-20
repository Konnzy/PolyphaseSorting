namespace PolyphaseSorting
{
    public class GenerateFile
    {
        public static void GenerateFile10MB(string filePath)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\nGenerating file 10MB..");
            Console.ResetColor();
            
            var random = new Random();
            using (var writer = new StreamWriter(filePath))
            {
                for (int i = 0; i < 290000; i++)
                {
                    char letter = (char)random.Next('A', 'Z' + 1);
                    string randomDigits = GenerateRandomString(20, random);
                    string phoneNumber = $"380{random.Next(100000000, 1000000000)}";
                    writer.WriteLine($"{letter}-{randomDigits}-{phoneNumber}");
                }
            }
        }
        public static void GenerateFile100MB(string filePath)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\nGenerating file 100MB..");
            Console.ResetColor();
            
            var random = new Random();
            using (var writer = new StreamWriter(filePath))
            {
                for (int i = 0; i < 2900000; i++)
                {
                    char letter = (char)random.Next('A', 'Z' + 1);
                    string randomDigits = GenerateRandomString(20, random);
                    string phoneNumber = $"380{random.Next(100000000, 1000000000)}";
                    writer.WriteLine($"{letter}-{randomDigits}-{phoneNumber}");
                }
            }
        }
        public static void GenerateFile500MB(string filePath)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\nGenerating file 500MB..");
            Console.ResetColor();
            
            var random = new Random();
            using (var writer = new StreamWriter(filePath))
            {
                for (int i = 0; i < 14800000; i++)
                {
                    char letter = (char)random.Next('A', 'Z' + 1);
                    string randomDigits = GenerateRandomString(20, random);
                    string phoneNumber = $"380{random.Next(100000000, 1000000000)}";
                    writer.WriteLine($"{letter}-{randomDigits}-{phoneNumber}");
                }
            }
        }
        public static void GenerateFile1GB(string filePath)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\nGenerating file 1GB..");
            Console.ResetColor();
            
            var random = new Random();
            using (var writer = new StreamWriter(filePath))
            {
                for (int i = 0; i < 29000000; i++)
                {
                    char letter = (char)random.Next('A', 'Z' + 1);
                    string randomDigits = GenerateRandomString(20, random);
                    string phoneNumber = $"380{random.Next(100000000, 1000000000)}";
                    writer.WriteLine($"{letter}-{randomDigits}-{phoneNumber}");
                }
            }
        }
        private static string GenerateRandomString(int Length, Random rand)
        {
            const string letters = "abcdefghijklmnopqrstuwxyzABCDEFGHIJKLMNOPQRSTUXYZ";
            char[] chars = new char[Length];

            for (int i = 0; i < Length; i++)
            {
                chars[i] = letters[rand.Next(letters.Length)];
            }
            return new string(chars);
        }
    }
}