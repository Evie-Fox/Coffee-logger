namespace CoffeeLogger
{
    public class CoffeeBeansManager
    {
        private readonly SQLController db;

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
            db.Beans.AddBean(name, roasterName, origin, roast);
        }

        private bool CoffeeNameIsTaken(string coffeeBeanName)
        {
            return db.Beans.GetCoffeeNames().Contains(coffeeBeanName);
        }

        public async Task<string?> AddNewCoffeeBeans()
        {
            while (true)
            {
                string? coffeeName, roasterName, origin;
                RoastLevel roast = RoastLevel.None;

                Console.WriteLine("\nEnter coffee beans name:\n");
                coffeeName = await ReadWithEsc();
                if (string.IsNullOrWhiteSpace(coffeeName))
                { return null; }
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
                            string? val;
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
                                return coffeeName;
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
                            return coffeeName;
                        }
                    }
                }
            }
        }

        public async Task<string?> ChooseOrAddCoffeeBeans()
        {
            string? beansName;
            string[] names = db.Beans.GetCoffeeNames();
            int namesLength = names.Length;

            Console.WriteLine("\nChoose coffee beans:\n");

            Console.WriteLine($"0. New beans\n");
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
                    string? newBrewerName = await AddNewCoffeeBeans();
                    if (newBrewerName == null)
                    { continue; }
                    return newBrewerName;
                }
                return names[num - 1];
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