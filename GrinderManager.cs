using System.Data.SQLite;

namespace CoffeeLogger
{
    public class GrinderManager
    {
        private readonly SQLController db;
        public GrinderManager(SQLController dbRef) 
        {
            db = dbRef;
        }

        private bool AddNewGrinder(string name, string format)
        {

            if (IsGrinderRegistered(name))
            {
                Console.WriteLine("\n\nGrinder name is taken on the DB\n");
                return false;
            }

            db.Grinders.AddGrinder(name, format);
            Console.WriteLine($"\n\nGrinders on DB: {string.Join(", ", db.Grinders.GetGrinderNames())}");
            return true;
        }


        public async Task<string?> AddNewGrinderFromConsole()
        {
            while (true)
            {
                string? name, format;
                while (true)
                {
                    Console.Write("\nEnter grinder name:\n\n>");
                    name = await Program.ReadWithEsc();
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        return null;
                    }
                    name = name.Trim();
                    if (!name.All(x => char.IsAsciiLetter(x) || x == ' ' || x == '(' || x == ')'))
                    {
                        Console.WriteLine("Invalid name\n");
                        continue;
                    }
                    if (IsGrinderRegistered(name))
                    {
                        Console.WriteLine("\n\nGrinder name is taken on the DB\n");
                        continue;
                    }
                    break;

                }
                Console.WriteLine("\n\nEnter dial format:\nExample: 4,5,9  \n\nExplanation: \n4 is the highest possible rotation, it cannot go above it or below zero.\n5 is the highest large mark before a full rotation, as on a full rotation it returns to 0 and adds 1 to the rotation.\n9 is the highest small mark, as when instead of going to 10 it returns to 0 and adds 1 to the larger mark.\n\n");
                while (true)
                {
                    Console.Write('>');
                    format = await Program.ReadWithEsc();
                    if (string.IsNullOrWhiteSpace(format))
                    {
                        break;
                    }
                    if (!format.All(x => char.IsDigit(x) || x == ','))
                    {
                        Console.WriteLine("\n\nInvalid format\n\nEnter dial format:\n");
                        continue;
                    }
                    format = new GrindDial(format).GetDialFormatString();
                    if (format == null)
                    {
                        continue;
                    }
                    AddNewGrinder(name, format);
                    return name;
                }

            }
        }

        public bool IsGrinderRegistered(string name)
        {
            string[] grinderNamesOnDB = db.Grinders.GetGrinderNames();
            return grinderNamesOnDB.Contains(name);
        }

        public string GetGrinderDialFormatOrNull(string grinderName)
        {
            if (!IsGrinderRegistered(grinderName))
            {
                Console.WriteLine("Grinder doesn't exist");
                return null;
            }
            return db.Grinders.GetGrinderDialFormat(grinderName);
        }

        public bool IsSettingCompatible(string grinderName, GrindDial newSetting)
        {
            string format = GetGrinderDialFormatOrNull(grinderName);

            int?[] formatNums = new GrindDial(format).GetDialFormatIntArr();
            int?[] settingNums = newSetting.GetDialFormatIntArr();

            int formatLength = formatNums.Length;

            if (formatLength != settingNums.Length || settingNums.All(x => x < 1)) 
            { return false; }

            for (int i = 0; i < formatLength; i++)
            {
                if (formatNums[i] < settingNums[i]) 
                { return false; }
            }
            return true;
        }

        public async Task<string?> ChooseGrinder()
        {
            string[] names = db.Grinders.GetGrinderNames();
            int namesLength = names.Length;

            Console.WriteLine("\n\nChoose a grinder:\n");
            Console.WriteLine("0. New grinder\n");
            for (int i = 0; namesLength > i; i++)
            {
                Console.WriteLine($"{i + 1}. {names[i]}");
            }
            string? inputNum;
            while (true)
            {
                Console.Write("\nEnter grinder's number:\n\n>");
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
                    string? name = await AddNewGrinderFromConsole();
                    if (name == null) 
                    { continue; }
                    return name;
                }
                return names[num - 1];
            }
        }
    }
}