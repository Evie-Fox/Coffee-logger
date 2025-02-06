namespace CoffeeLogger
{
    public class BrewerManager
    {
        private SQLController db;

        public BrewerManager(SQLController dbRef)
        {
            db = dbRef;
        }

        public async Task<string?> AddNewBrewer()
        {
                string? brewerName;
                BrewMethod brewMethod = BrewMethod.None;
            while (true)
            {

                Console.WriteLine("\nEnter Brewer name:\n");
                brewerName = await Program.ReadWithEsc();
                if (string.IsNullOrWhiteSpace(brewerName))
                { return null; }
                brewerName = brewerName.Trim();

                if (IsBrewerNameTaken(brewerName))
                {
                    Console.WriteLine("\nBrewer name is taken on the DB\n");
                    continue;
                }
                while (true)
                {
                    string? val;
                    Console.WriteLine("\nChoose roast level:  (Optional)\n1. Percolation\n2. Infusion\n3. Steep and release");
                    val = await Program.ReadWithEsc();
                    if (val == null)
                    {
                        break;
                    }
                    val = val.Trim();

                    if (string.IsNullOrWhiteSpace(val))
                    {
                        db.Brewers.AddBrewer(brewerName, BrewMethod.None);
                        Console.WriteLine($"\nAdded {brewerName} brewer to DB\n");
                        return brewerName;
                    }

                    if (!val.All(x => char.IsDigit(x)))
                    {
                        Console.WriteLine("\nPlease enter a number on leave it empty\n");
                        continue;
                    }
                    int num = int.Parse(val);
                    if (num < (int)BrewMethod.None || (int)BrewMethod.SteepAndRelease < num)
                    {
                        Console.WriteLine("\nNumber is outside of range\n");
                        continue;
                    }
                    brewMethod = (BrewMethod)num;
                    db.Brewers.AddBrewer(brewerName, brewMethod);
                    Console.WriteLine($"\nAdded {brewerName} brewer to DB\n");
                    return brewerName;
                }
            }
        }
        public async Task<string?> ChooseOrAddBrewer()
        {
            string[] names = db.Brewers.GetBrewerNames();
            int namesLength = names.Length;

            Console.WriteLine("\nChoose a brewer:\n");

            Console.WriteLine($"0. New brewer\n");
            for (int i = 0; namesLength > i; i++)
            {
                Console.WriteLine($"{i + 1}. {names[i]}\n");
            }



            string? inputNum;
            while (true)
            {
                inputNum = await Program.ReadWithEsc();
                if (string.IsNullOrWhiteSpace(inputNum))
                {
                    return null;
                }
                inputNum = inputNum.Trim();
                if (!inputNum.All(x => char.IsAsciiDigit(x)))
                {
                    Console.WriteLine("Invalid number\n");
                    continue;
                }
                int num = int.Parse(inputNum);
                if (num < 0 || num > namesLength)
                {
                    Console.WriteLine("\n\nOutside of range\n");
                    continue;
                }
                if (num == 0)
                {
                    string? newBrewerName = await AddNewBrewer();
                    if ( newBrewerName == null)
                    { continue; }
                    return newBrewerName;
                }
                return names[num - 1];
            }
        }

        private bool IsBrewerNameTaken(string brewerName)
        {
            return db.Brewers.GetBrewerNames().Contains(brewerName);
        }
    }
}