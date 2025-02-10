namespace CoffeeLogger
{
    public class LogManager
    {
        private readonly SQLController dbController;

        public LogManager(SQLController dbController)
        {
            this.dbController = dbController;
        }

        public async Task<string?> AddNewLogFromConsole(int brewID)
        {
            string? val;
            string notes;
            int extractionMeter, score, temperature;
            while (true)
            {
                Console.Write("\n\nEnter brew temperature: (0c-100c, whole numbers)\n\n>");
                val = await Program.ReadWithEsc();
                if (string.IsNullOrWhiteSpace(val))
                {
                    return null;
                }
                if (!val.All(char.IsAsciiDigit))
                {
                    Console.WriteLine("Invalid format, use only whole numbers\n\n");
                    continue;
                }
                temperature = int.Parse(val);
                if (temperature > 100 || temperature < 0)
                {
                    Console.WriteLine("\nNot a realistic temperature\n");
                    continue;
                }
                Console.WriteLine("\nExtraction meter explanation:\n\n" +
                "      -5    -4    -3    -2    -1    0    +1    +2    +3    +4    +5 \n" +
                "     [Sour ---------------------- Ideal ---------------------- Bitter] \n");
                Console.WriteLine("\nEnter extraction meter: (whole numbers)\n\n");
                while (true)
                {

                    val = await Program.ReadWithEsc();
                    if (string.IsNullOrWhiteSpace(val))
                    {
                        break;
                    }
                    if (!val.All(x => char.IsAsciiDigit(x) || x == '-') && !val.Substring(1).Contains('-'))
                    {
                        Console.WriteLine("\n\nInvalid format, use only whole numbers from -5 to 5\n\n");
                        continue;
                    }
                    extractionMeter = int.Parse(val);
                    if (extractionMeter > 5 || extractionMeter < -5)
                    {
                        Console.WriteLine("\n\nOutside of range, use only whole numbers from -5 to 5\n\n");
                        continue;
                    }
                    Console.WriteLine("\n\nEnter score: (0 - 10, whole numbers)\n\n");
                    while (true)
                    {
                        val = await Program.ReadWithEsc();
                        if (string.IsNullOrWhiteSpace(val))
                        {
                            break;
                        }
                        if (!val.All(char.IsAsciiDigit))
                        {
                            Console.WriteLine("Invalid format, use only whole numbers\n\n");
                            continue;
                        }
                        score = int.Parse(val);
                        if (score > 10 || score < 0)
                        {
                            Console.WriteLine("\nOutside of range (0 - 10)\n");
                            continue;
                        }
                        Console.WriteLine("\nEnter notes: (optional, leave empty. keep below 255 characters)\n\n");
                        while (true)
                        {
                            val = await Program.ReadWithEsc();
                            if (val == null)
                            {
                                break;
                            }
                            if (val.Length > 255)
                            {
                                Console.WriteLine("\n\nNotes are too long, please cut it down.\n");
                                Console.Write("\n" + val);
                                continue; //TODO chack if this writes the text 
                            }
                            notes = val.Trim();

                            //INSERT Log
                        }

                    }
                }
            }
        }
    }
}