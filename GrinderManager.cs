using System.Data.SQLite;

namespace CoffeeLogger
{
    public class GrinderManager
    {
        private SQLController db;
        public GrinderManager(SQLController dbRef) 
        {
            db = dbRef;
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


        public async Task AddNewGrinderFromConsole()
        {
            while (true) //allows to return to entering a name
            {
                string? name, format;
                while (true)
                {
                    Console.Write("\nEnter grinder name:\n>");
                    name = await Program.ReadWithEsc();
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        return;
                    }
                    name = name.Trim();
                    if (!name.All(x => char.IsAsciiLetter(x) || x == ' ' || x == '(' || x == ')'))
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
                    format = new GrindDial(format).GetDialFormat();
                    AddNewGrinder(name, format);

                    Console.WriteLine(GetGrinderDialFormat(name) + " is the format!");
                    /////////
                    return;
                }

            }
        }
        private bool GrinderNameIsTaken(string name)
        {
            string[] grinderNamesOnDB = db.GetGrinderNames();
            return grinderNamesOnDB.Contains(name);
        }
        public string GetGrinderDialFormat(string grinderName)
        {
            if (!GrinderNameIsTaken(grinderName))
            {
                Console.WriteLine("Grinder doesn't exist");
                return null;
            }
            return db.GetGrinderDialFormat(grinderName);
        }
    }
}