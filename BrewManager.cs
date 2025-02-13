namespace CoffeeLogger
{
    public class BrewManager
    {
        private readonly SQLController dbController;
        private readonly GrinderManager grinderMan;
        private readonly GrindSettingManager grindMan;
        private readonly CoffeeBeansManager coffeeBeansMan;
        private readonly BrewerManager brewerMan;
        private readonly RatioManager ratioMan;
        private readonly LogManager logMan;

        public BrewManager(SQLController db, GrinderManager grinderMan, GrindSettingManager grindMan, CoffeeBeansManager coffeeBeansMan, BrewerManager brewerMan, RatioManager ratioMan, LogManager logMan)
        {
            this.dbController = db;
            this.grinderMan = grinderMan;
            this.coffeeBeansMan = coffeeBeansMan;
            this.grindMan = grindMan;
            this.brewerMan = brewerMan;
            this.ratioMan = ratioMan;
            this.logMan = logMan;
        }

        public async Task NewBrew()
        {
            while (true)
            {
                string? brewerName = await brewerMan.ChooseOrAddBrewer();
                if (brewerName == null)
                { break; }

                while (true)
                {
                    string? coffeeBeansName = await coffeeBeansMan.ChooseOrAddCoffeeBeans();
                    if (coffeeBeansName == null)
                    { break; }
                    while (true)
                    {
                        string? grinderName = await grinderMan.ChooseGrinder();
                        if (grinderName == null)
                        { break; }
                        while (true)
                        {
                            string? grindSetting = await grindMan.ChooseOrAddGrindSetting(grinderName, coffeeBeansName, brewerName);
                            if (grindSetting == null)
                            { break; }
                            while (true)
                            {
                                int? gramsPerLiter = await ratioMan.ChooseOrAddRatio(grinderName, coffeeBeansName, brewerName, grindSetting);
                                if (gramsPerLiter == null)
                                { break; }
                                int brewID = dbController.Brew.GetBrewID(grinderName, coffeeBeansName, brewerName, grindSetting, gramsPerLiter);

                                while (true)
                                {
                                    int temperature;
                                    Console.Write("\n\nEnter brew temperature: (0c-100c, whole numbers)\n\n>");
                                    string val = await Program.ReadWithEsc();
                                    if (string.IsNullOrWhiteSpace(val))
                                    {
                                        break;
                                    }
                                    if (!val.All(char.IsAsciiDigit))
                                    {
                                        Console.WriteLine("Invalid format, use only whole numbers\n\n");
                                        continue;
                                    }
                                    temperature = int.Parse(val);
                                    if (temperature > 100 || temperature < 0)
                                    {
                                        Console.WriteLine("\nNot a Outside liquid range\n");
                                        continue;
                                    }
                                    Console.WriteLine($"\n\nCurrent Brew:\nCoffee beans: {coffeeBeansName}\nBrewer: {brewerName}\nGrinder: {grinderName}\nGrind setting: {grindSetting}\nGrams per liter: {gramsPerLiter}\nTemperature: {temperature}\n\n");
                                    if (dbController.IsBrewTaken(coffeeBeansName, brewerName, grinderName, grindSetting, (int)gramsPerLiter, temperature))
                                    {
                                        Console.WriteLine("\nBrew is already registered, a new log will override an existing one\n");
                                    }
                                    else
                                    {
                                        dbController.Brew.InsertBrewToDB(grinderName, coffeeBeansName, brewerName, grindSetting, (int)gramsPerLiter, temperature);
                                    }
                                    Console.WriteLine("\n\nAdd a log for the brew?  Y/N\n\n");
                                    string? res = await Program.ReadWithEsc();
                                    if (!string.IsNullOrEmpty(res))
                                    {
                                        res = res.ToLower().Replace(" ", "");
                                        if (res == "y" || res == "yes")
                                        {
                                            await logMan.AddNewLogFromConsole(brewID);
                                        }
                                    }
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}