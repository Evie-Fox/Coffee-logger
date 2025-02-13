namespace CoffeeLogger
{
    public class RatioManager
    {
        private readonly SQLController db;

        public RatioManager(SQLController db)
        {
            this.db = db;
        }

        public async Task<int?> ChooseOrAddRatio(string grinderName, string beansName, string brewerName, string grindSetting)
        {
            int[] registeredRatios = db.Ratio.GetRatiosInBrews(grinderName, beansName, brewerName, grindSetting).Select(int.Parse).ToArray();
            Array.Sort(registeredRatios);
            int length = registeredRatios.Length;

            while (true)
            {
                Console.WriteLine("\n\nChoose grams per liter ratio:\n\n0. New Ratio\n");
                for (int i = 0; length > i; i++)
                {
                    Console.WriteLine($"{i + 1}. {registeredRatios[i]}\n");
                }
                string? ratioIndex = await Program.ReadWithEsc();
                if (ratioIndex == null)
                {
                    return null;
                }
                if (!ratioIndex.Replace(" ", string.Empty).All(x => char.IsDigit(x)))
                {
                    Console.WriteLine("\nInvalid format, please choose one of the shown numbers\n");
                    continue;
                }
                int index = int.Parse(ratioIndex);
                if (index < 0 || index > length)
                {
                    Console.WriteLine("\nNumber is outside pf range, please choose one of the shown numbers\n");
                    continue;
                }
                if (index != 0)
                {
                    return registeredRatios[index - 1];
                }
                while (true)
                {
                    int? newRatio = await AddNewRatioFromConsole(registeredRatios);
                    if (newRatio == null)
                    { break; }

                    return newRatio;
                }
            }
        }

        public async Task<int?> AddNewRatioFromConsole(int[] registeredRatios)
        {
            while (true)
            {
                Console.WriteLine("\n\nEnter a grams per liter of water\n");
                string? ratio = await Program.ReadWithEsc();
                if (ratio == null)
                { return null; }

                if (!ratio.Replace(" ", string.Empty).All(x => char.IsDigit(x)))
                {
                    Console.WriteLine("\nInvalid format, please choose one of the shown numbers\n");
                    continue;
                }
                int numRatio = int.Parse(ratio);
                if (registeredRatios.Contains(numRatio))
                {
                    Console.WriteLine("\nRatio is already registered\n");
                }
                return numRatio;
            }
        }
    }
}