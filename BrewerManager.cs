namespace CoffeeLogger
{
    public class BrewerManager
    {
        private SQLController db;

        public BrewerManager(SQLController dbRef)
        {
            db = dbRef;
        }

        public async Task AddNewBrewer()
        {
            while (true)
            {
                string? brewerName;
                BrewMethod brewMethod = BrewMethod.None;

                Console.WriteLine("\nEnter Brewer name:\n");
                brewerName = await Program.ReadWithEsc();
                if (string.IsNullOrWhiteSpace(brewerName))
                { return; }
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
                        db.AddBrewer(brewerName, BrewMethod.None);
                        Console.WriteLine($"\nAdded {brewerName} brewer to DB\n");
                        return;
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
                    db.AddBrewer(brewerName, brewMethod);
                    Console.WriteLine($"\nAdded {brewerName} brewer to DB\n");
                    return;
                }
            }
        }

        private bool IsBrewerNameTaken(string brewerName)
        {
            return db.GetBrewerNames().Contains(brewerName);
        }
    }
}