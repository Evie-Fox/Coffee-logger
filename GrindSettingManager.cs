using System.Data.SQLite;

namespace CoffeeLogger
{
    public class GrindSettingManager
    {
        private readonly SQLController db;
        private readonly GrinderManager gm;

        public GrindSettingManager(SQLController db, GrinderManager gm)
        {
            this.db = db;
            this.gm = gm;
        }


        public async Task<string?> ChooseOrAddGrindSetting(string? grinderName, string beansName, string brewerName)
        {
            while (true)
            {
                if (grinderName == null)
                { return null; }

                string grinderFormat = gm.GetGrinderDialFormatOrNull(grinderName);
                if (grinderFormat == null)
                {
                    Console.WriteLine("\nSomething went wrong, grinder doesn't have a format.\n");
                    return null;
                }

                Console.WriteLine($"\nChose {grinderName} with the format of: {grinderFormat}\n\n");

                string?[] registeredGrinds = db.GrindSettings.GetBrewedGrindSettingsOrNull(grinderName, beansName, brewerName);
                int length = registeredGrinds.Length;

                Console.WriteLine("\n0. New grind\n");//!!=
                for (int i = 0; length > i; i++)
                {
                    Console.WriteLine($"{i + 1}. {registeredGrinds[i]}\n");
                }
                while (true)
                {
                    string? grindIndex = await Program.ReadWithEsc();
                    if (grindIndex == null)
                    {
                        return null;
                    }
                    if (!grindIndex.Replace(" ", string.Empty).All(x => char.IsDigit(x)))
                    {
                        Console.WriteLine("\nInvalid format, please choose one of the shown numbers\n");
                        continue;
                    }
                    int grindNum = int.Parse(grindIndex);
                    if (grindNum < 0 || grindNum > length)
                    {
                        Console.WriteLine("\nNumber is outside pf range, please choose one of the shown numbers\n");
                        continue;
                    }
                    if (grindNum != 0)
                    {
                        return registeredGrinds[grindNum - 1];
                    }
                    while (true)
                    {
                        string? newGrind = await AddNewGrindSetting(grinderName);
                        if (newGrind == null)
                        { break; }
                        if (db.IsBrewTaken(beansName, brewerName, grinderName, newGrind))
                        {
                            Console.WriteLine("\nGrind setting is already registered for this brew\n");
                            continue;
                        }
                        return newGrind;
                    }
                }
            }
        }

        private async Task<string?> AddNewGrindSetting(string grinderName)
        {
            string? newSetting, format = gm.GetGrinderDialFormatOrNull(grinderName);
            if (format == null)
            {
                Console.WriteLine("\nFormat not found\n");
                return null;
            }
            GrindDial newDialSetting;
            Console.WriteLine("\n\nEnter dial setting:\nExample: 4,5,9  \n\nExplanation: \n4 is the highest possible rotation, it cannot go above it or below zero.\n5 is the highest large mark before a full rotation, as on a full rotation it returns to 0 and adds 1 to the rotation.\n9 is the highest small mark, as when instead of going to 10 it returns to 0 and adds 1 to the larger mark.\n");
            while (true)
            {
                Console.WriteLine($"\nFormat: {format}\n");
                Console.Write("\n>");
                newSetting = await Program.ReadWithEsc();
                if (string.IsNullOrWhiteSpace(newSetting))
                {
                    break;
                }
                newDialSetting = new(newSetting);
                if (!newDialSetting.isValid)
                {
                    Console.WriteLine("\n\nInvalid format\n\nEnter dial format:\n");
                    continue;
                }
                
                if (!gm.IsSettingCompatible(grinderName, newDialSetting))
                {
                    Console.WriteLine("\ndial setting is not compatible with the grinder's format\n");
                    continue;
                }

                return newSetting;
            }
            return null;
        }
    }
}