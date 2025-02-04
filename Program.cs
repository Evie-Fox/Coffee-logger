using System.Text;

namespace CoffeeLogger
{
    public class Program
    {
        public SQLController db;

        public static Program instance;

        private GrinderManager gm;
        private GrinderSettingManager gsm;

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

            gm = new GrinderManager(db);
            gsm = new GrinderSettingManager(db, gm);

            ConsoleReader();
            Console.WriteLine("\n\nShutting down");
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
                    case "cleardb":
                        ClearDB(); 
                        break;

                    case "newgrinder" or "addgrinder":
                        await gm.AddNewGrinderFromConsole();
                        break;
                    case "grinders":
                        Console.WriteLine("\n\n" + String.Join("\n", db.GetGrinderNames()) + "\n");
                        break;

                    case "newgrind":
                        await gsm.AddNewGrindSetting();
                        break;

                    case "newbrewer" or "addbrewer":
                        break;
                    case "brewers":
                        Console.WriteLine("\n\n" + String.Join("\n", db.GetBrewerNames()) + "\n");
                        break;

                    case "newcoffee" or "newbeans" or "newcoffeebeans" or "addcoffee" or "addbeans":
                        break;
                    case "coffee" or "beans" or "coffeebeans":
                        Console.WriteLine("\n\n" + String.Join("\n", db.GetCoffeeNames()) + "\n");
                        break;

                    case "stop" or "quit":
                        return;

                    default:
                    case null or "":
                        Console.WriteLine("\nInvalid command");
                        break;
                }
            }
        }

        private void ClearDB()
        {
            Console.WriteLine("CLEAR DB");
            db.ClearDB();
        }

        public static async Task<string?> ReadWithEsc()
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
    }
    public enum RoastLevel
    {
        None = 0,
        Light = 1,
        Medium = 2,
        Dark = 3
    }
    public enum BrewMethod
    {
        None = 0,
        Percolation = 1,
        Infusion = 2,
        SteepAndRelease = 3
    }
}