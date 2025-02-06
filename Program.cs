using System.Text;

namespace CoffeeLogger
{
    public class Program
    {
        public SQLController db;

        public static Program instance;

        private GrinderManager grinderMam;
        private GrinderSettingManager grindSettingMan;
        private CoffeeBeansManager beanMan;
        private BrewerManager brewerMan;
        private BrewManager brewMan;

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

            grinderMam = new GrinderManager(db);
            grindSettingMan = new GrinderSettingManager(db, grinderMam);
            beanMan = new CoffeeBeansManager(db);
            brewerMan = new BrewerManager(db);
            brewMan = new BrewManager(db, grinderMam, beanMan, brewerMan);

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
                        await grinderMam.AddNewGrinderFromConsole();
                        break;
                    case "grinders":
                        Console.WriteLine("\n\n" + String.Join("\n", db.Grinders.GetGrinderNames()) + "\n");
                        break;

                    case "newgrind":
                        await grindSettingMan.AddNewGrindSetting();
                        break;

                    case "newbrewer" or "addbrewer":
                        await brewerMan.AddNewBrewer();
                        break;
                    case "brewers":
                        Console.WriteLine("\n\n" + String.Join("\n", db.Brewers.GetBrewerNames()) + "\n");
                        break;

                    case "newcoffee" or "newbeans" or "newcoffeebeans" or "addcoffee" or "addbeans":
                        await beanMan.AddNewCoffeeBeans();
                        break;
                    case "coffee" or "beans" or "coffeebeans":
                        Console.WriteLine("\n\n" + String.Join("\n", db.Beans.GetCoffeeNames()) + "\n");
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