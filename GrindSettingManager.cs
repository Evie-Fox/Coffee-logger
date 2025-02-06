
namespace CoffeeLogger
{
    class GrindSettingManager
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

        private async Task<string?> ChooseGrindSetting(string grinderName)
        {
            string? newSetting;
            while (true)
            {
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
