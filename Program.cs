using System.Text;

namespace CoffeeLogger
{
    public class Program
    {
        public SQLController db;

        public static Program instance;

        private static void Main()
        {
            instance = new Program();
        }

        public Program()
        {
            Init();
        }

        private void Init()
        {
            db = new SQLController();
            db.Activate();
            ConsoleReader();
        }

        private async Task ConsoleReader()
        {
            string input = string.Empty;
            while (true)
            {
                Console.Write("\n>");
                input = Console.ReadLine().Trim().ToLower();
                Console.WriteLine("");

                switch (input)
                {
                    case "newgrinder":
                        await AddNewGrinderFromConsole();
                        break;
                    case "cleardb":
                        ClearDB(); 
                        break;

                    case "stop" or "quit":
                        return;

                    default:
                    case null or "":
                        Console.WriteLine("Invalid command");
                        break;
                }
            }
        }

        private void ClearDB()
        {
            Console.WriteLine("CLEAR DB");
            db.ClearDB();
        }

        private async Task<string?> ReadWithEsc()
        {
            StringBuilder input = new StringBuilder();

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
                    ConsoleKey rawKey = keyInfo.Key;
                    Char keyChar = keyInfo.KeyChar;

                    if (rawKey == ConsoleKey.Escape)
                    {
                        return null;
                    }
                    if (rawKey == ConsoleKey.Enter)
                    {
                        return input.ToString();
                    }
                    if (rawKey == ConsoleKey.Backspace && input.Length != 0)
                    {
                        input.Remove(input.Length - 1, 1);
                        Console.Write("\b \b"); //wat?
                        continue;
                    }
                    if (!char.IsControl(keyChar))
                    {
                        input.Append(keyChar);
                        Console.Write(keyChar);
                    }

                    
                }
            }
        }


        private async Task AddNewGrinderFromConsole()
        {
            while (true)
            {
                Console.Write("\nEnter grinder name:\n>");
                string? name = await ReadWithEsc();
                if (name == null)
                {
                    return;
                }
                if (string.IsNullOrWhiteSpace(name) || !name.Any(char.IsAsciiLetter))
                {
                    Console.WriteLine("Invalid name\n");
                    continue;
                }
                if (AddNewGrinder(name))
                {
                    return;
                }

            }
        }

        private bool AddNewGrinder(string name)
        {
            string grinderName = name;
            string[] grinderNamesOnDB = db.GetGrinderNames();

            if (grinderNamesOnDB.Contains(grinderName))
            {
                Console.WriteLine("\n\nGrinder name is taken on the DB\n");
                return false;
            }

            db.AddGrinder(grinderName);
            Console.WriteLine($"\n\nGrinders on DB: {string.Join(", ", db.GetGrinderNames())}");
            return true;
        }

        private void AddNewBrewer(string name)
        {
            string brewerName = name;
            string[] brewerNames = db.GetBrewerNames();

            if (brewerNames.Contains(brewerName))
            {
                Console.WriteLine("Brewer name is taken on the DB");
                return;
            }

            db.AddBrewer(brewerName);
        }

        private void AddNewCoffee(string name)//Nothing here
        {
            string coffeeBeanName = name;
            string[] coffeeNames = db.GetCoffeeNames();

            if (coffeeNames.Contains(coffeeBeanName))
            {
                Console.WriteLine("Coffee bean name is taken on the DB");
                return;
            }
            db.AddBean(coffeeBeanName, SQLController.RoastLevel.None);
        }
    }
}