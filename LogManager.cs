namespace CoffeeLogger
{
    public class LogManager
    {
        private readonly SQLController dbController;

        public LogManager(SQLController dbController)
        {
            this.dbController = dbController;
        }

        public async Task AddNewLogFromConsole(int brewID)
        {
            string? val;
            string notes;
            int extractionMeter, score, temperature;

            int? logId = dbController.Log.GetExistingLogID(brewID);
            if (logId != null)
            {
                Console.WriteLine("\nA log with identical parameters already exist, would you like to delete it? (Y/N)\n\n");
                val = await Program.ReadWithEsc();
                if (string.IsNullOrEmpty(val))
                { return; }
                val = val.Trim().ToLower();
                if (val != "yes" && val != "y")
                { return; }
                Console.WriteLine("\nDeleted old log\n");
                dbController.Log.DeleteLog((int)logId);
            }
            Console.WriteLine("\nExtraction meter explanation:\n\n" +
            "      -5    -4    -3    -2    -1    0    +1    +2    +3    +4    +5 \n" +
            "     [Sour ---------------------- Ideal ---------------------- Bitter] \n");
            Console.WriteLine("\n\nEnter extraction meter: (whole numbers)\n\n");
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
                    val = "";
                    while (true)
                    {
                        val = await Program.ReadWithEsc(val);
                        if (val == null)
                        {
                            break;
                        }
                        if (val.Length > 255)
                        {
                            Console.WriteLine("\n\nNotes are too long, please cut it down.\n");
                            Console.Write("\n" + val);
                            continue; //TODO Drews it but can't delete
                        }
                        notes = val.Trim();
                        dbController.Log.AddLog(brewID, extractionMeter, score, notes);

                        Console.WriteLine("\n\nLogged brew.\n\n");
                        return;
                    }
                }
            }
        }
    }
}