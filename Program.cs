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
            string? input;
            while (true)
            {
                Console.Write("\n>");
                input = await ReadWithEsc();
                
                if (input == null)
                { return; }

                input = input.ToLower().Replace(" ", string.Empty);

                switch (input)
                {
                    case "newgrinder":
                        await AddNewGrinderFromConsole();
                        break;
                    case "grinders":
                        Console.WriteLine("\n\n" + String.Join("\n", db.GetGrinderNames()) + "\n");
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
                        Console.Write("\bCanceled\n");
                        return null;
                    }
                    if (rawKey == ConsoleKey.Enter)
                    {
                        return input.ToString();
                    }
                    if (rawKey == ConsoleKey.Backspace && input.Length != 0)
                    {
                        input.Remove(input.Length - 1, 1);
                        Console.Write("\b \b");
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
            while (true) //allows to return to entering a name
            {
                string? name, format;
                while (true)
                {
                    Console.Write("\nEnter grinder name:\n>");
                    name = await ReadWithEsc();
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        return;
                    }
                    name = name.Trim();
                    if (!name.All(x => char.IsAsciiLetter(x) || x == ' ' || x == '(' || x ==')'))
                    {
                        Console.WriteLine("Invalid name\n");
                        continue;
                    }
                    if (GrinderNameIsTaken(name))
                    {
                        Console.WriteLine("\n\nGrinder name is taken on the DB\n");
                        continue;
                    }
                    break;

                }
                Console.WriteLine("\n\nEnter dial format:\nExample: 4,5,9  \n\nExplanation: \n4 is the highest possible rotation,\n5 is the highest large mark before a full rotation, as on a full rotation it returns to 0\n9 is the highest small mark, as when instead of going to 10 it returns to 0 and adds 1 to the larger mark\n\n");
                while (true)
                {
                    Console.Write('>');
                    format = await ReadWithEsc();
                    if (string.IsNullOrWhiteSpace(format))
                    {
                        break;
                    }
                    if (!format.All(x => char.IsDigit(x) || x ==','))
                    {
                        Console.WriteLine("\n\nInvalid format\n\nEnter dial format:\n");
                        continue;
                    }
                    AddNewGrinder(name, format);
                    return;
                }

            }
        }

        private bool AddNewGrinder(string name, string format)
        {

            if (GrinderNameIsTaken(name))
            {
                Console.WriteLine("\n\nGrinder name is taken on the DB\n");
                return false;
            }

            db.AddGrinder(name, format);
            Console.WriteLine($"\n\nGrinders on DB: {string.Join(", ", db.GetGrinderNames())}");
            return true;
        }

        private bool GrinderNameIsTaken(string name)
        {
            string[] grinderNamesOnDB = db.GetGrinderNames();
            return grinderNamesOnDB.Contains(name);
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