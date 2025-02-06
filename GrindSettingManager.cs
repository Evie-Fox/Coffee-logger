
using System.Data.SQLite;

namespace CoffeeLogger
{
    public class GrindSettingManager
    {
        private SQLController db;
        private GrinderManager gm;

        public GrindSettingManager(SQLController db, GrinderManager gm)
        {
            this.db = db;
            this.gm = gm;
        }

        public async Task AddNewGrindSetting()
        {
            while (true)
            {
                string? grinderName;

                grinderName = await gm.ChooseGrinder();

                if (grinderName == null)
                { return; }

                string grinderFormat = gm.GetGrinderDialFormatOrNull(grinderName);
                if (grinderFormat == null)
                {
                    Console.WriteLine("\nSomething went wrong, grinder doesn't have a format.\n");
                    return;
                }

                Console.WriteLine($"\nChose {grinderName} with the format of: {grinderFormat}");

                Console.WriteLine("\n\nEnter dial setting:\nExample: 4,5,9  \n\nExplanation: \n4 is the highest possible rotation, it cannot go above it or below zero.\n5 is the highest large mark before a full rotation, as on a full rotation it returns to 0 and adds 1 to the rotation.\n9 is the highest small mark, as when instead of going to 10 it returns to 0 and adds 1 to the larger mark.\n");
                while (true)
                {
                    string? newSetting = await ChooseGrindSetting(grinderName);
                    if (newSetting == null)
                    { return; }
                    if (db.GrindSettings.IsGrindSettingTaken(grinderName, newSetting))
                    {
                        Console.WriteLine("\nGrind setting already exists\n");
                        continue;
                    }
                    Console.WriteLine($"\n {newSetting} is compatible with {grinderName}.\n");
                    db.GrindSettings.AddGrindSetting(grinderName, newSetting);
                    return;
                }
            }
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

                Console.WriteLine("\n0. New grind\n");
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
                    }
                    if (grindNum != 0)
                    {
                        return registeredGrinds[grindNum - 1];
                    }
                    string? newGrind = await ChooseGrindSetting(grinderName);
                    if (newGrind == null)
                    { continue; }
                    return newGrind;
                }

            }
        }

        private async Task<string?> ChooseGrindSetting(string grinderName)
        {
            string? newSetting;
            while (true)
            {   string? format = gm.GetGrinderDialFormatOrNull(grinderName);
                if (format == null)
                {
                    Console.WriteLine("\nFormat not found\n");
                    return null; 
                }
                Console.WriteLine($"\nFormat: {format}\n");
                Console.Write("\n>");
                newSetting = await Program.ReadWithEsc();
                if (string.IsNullOrWhiteSpace(newSetting))
                {
                    break;
                }
                if (!newSetting.All(x => char.IsDigit(x) || x == ','))
                {
                    Console.WriteLine("\n\nInvalid format\n\nEnter dial format:\n");
                    continue;
                }
                GrindDial newDialSetting = new(newSetting);

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
