namespace CoffeeLogger
{
    public class CoffeeBeansManager
    {
        private SQLController db;

        public CoffeeBeansManager(SQLController db)
        {
            this.db = db;
        }

        private void AddNewCoffeeBeans(string name, string? roasterName, string? origin, RoastLevel roast = RoastLevel.None)
        {
            if (string.IsNullOrWhiteSpace(roasterName))
            { roasterName = null; }
            if (string.IsNullOrWhiteSpace(origin)) 
            { origin = null; }
            db.AddBean(name, roasterName, origin, roast);
        }

        private bool CoffeeNameIsTaken(string coffeeBeanName)
        {
            return db.GetCoffeeNames().Contains(coffeeBeanName);
        }

        public async Task AddNewCoffeeBeans()
        {
            while (true)
            {
                string? coffeeName, roasterName, origin;
                RoastLevel roast = RoastLevel.None;

                Console.WriteLine("\nEnter coffee beans name:\n");
                coffeeName = await ReadWithEsc();
                if (string.IsNullOrWhiteSpace(coffeeName))
                { return; }
                coffeeName = coffeeName.Trim();

                if (CoffeeNameIsTaken(coffeeName))
                {
                    Console.WriteLine("\nCoffee bean name is taken on the DB\n");
                    continue;
                }
                while (true)
                {
                    Console.WriteLine("\nEnter roaster name:  (Optional)\n");
                    roasterName = await ReadWithEsc();
                    if (roasterName == null)
                    {
                        break;
                    }
                    roasterName = roasterName.Trim();
                    while (true)
                    {
                        Console.WriteLine("\nEnter coffee origin:  (Optional)\n");
                        origin = await ReadWithEsc();
                        if (origin == null)
                        {
                            break;
                        }
                        origin = origin.Trim();
                        while (true)
                        {
                            string val;
                            Console.WriteLine("\nChoose roast level:  (Optional)\n1. Light roast\n2. Medium roast\n3. Dark roast");
                            val = await ReadWithEsc();
                            if (val == null)
                            {
                                break;
                            }
                            val = val.Trim();

                            if (string.IsNullOrWhiteSpace(val))
                            {
                                AddNewCoffeeBeans(coffeeName,roasterName,origin);
                                Console.WriteLine($"\nAdded {coffeeName} coffee beans to DB\n");
                                return;
                            }

                            if (!val.All(x => char.IsDigit(x)))
                            {
                                Console.WriteLine("\nPlease enter a number on leave it empty\n");
                                continue;
                            }
                            int num = int.Parse(val);
                            if (num < (int)RoastLevel.None || (int)RoastLevel.Dark < num)
                            {
                                Console.WriteLine("\nNumber is outside of range\n");
                                continue;
                            }
                            roast = (RoastLevel)num;
                            AddNewCoffeeBeans(coffeeName, roasterName, origin, roast);
                            Console.WriteLine($"\nAdded {coffeeName} coffee beans to DB\n");
                            return;
                        }
                    }
                }
            }
        }

        private async Task<string?> ReadWithEsc()
        {
            string? val;
            while (true)
            {
                Console.Write("\n>");
                val = await Program.ReadWithEsc();
                if (val == null)//pressed on escape
                {
                    break;
                }

                return val;
            }
            return null;
        }
    }
}