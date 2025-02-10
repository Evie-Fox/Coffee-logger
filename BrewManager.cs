using System.Data.Entity.Infrastructure;
using System.Data.SQLite;

namespace CoffeeLogger
{
    public class BrewManager
    {
        private SQLController dbController;
        private GrinderManager grinderMan;
        private GrindSettingManager grindMan;
        private CoffeeBeansManager coffeeBeansMan;
        private BrewerManager brewerMan;
        private RatioManager ratioMan;

        public BrewManager(SQLController db, GrinderManager grinderMan, GrindSettingManager grindMan, CoffeeBeansManager coffeeBeansMan, BrewerManager brewerMan, RatioManager ratioMan)
        {
            this.dbController = db;
            this.grinderMan = grinderMan;
            this.coffeeBeansMan = coffeeBeansMan;
            this.grindMan = grindMan;
            this.brewerMan = brewerMan;
            this.ratioMan = ratioMan;
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
                                bool isNew = false;
                                string? gramsPerLiter = await ratioMan.ChooseOrAddRatio(grinderName, coffeeBeansName, brewerName, grindSetting);
                                if (gramsPerLiter == null)
                                { break; }
                                if (gramsPerLiter[0] == 'N')
                                {
                                    isNew = true; 
                                    gramsPerLiter = gramsPerLiter.Substring(1);
                                    dbController.Brew.InsertBrewToDB(grinderName, coffeeBeansName, brewerName, grindSetting, gramsPerLiter);
                                }
                                int brewID = dbController.Brew.GetBrewID(grinderName, coffeeBeansName, brewerName, grindSetting, gramsPerLiter);

                                Console.WriteLine($"\n\nCurrent Brew:\nCoffee beans: {coffeeBeansName}\nBrewer: {brewerName}\nGrinder: {grinderName}\nGrind setting: {grindSetting}\nGrams per liter: {gramsPerLiter}\n\n");
                                Console.WriteLine("\n\nAdd a log for the brew?  Y/N\n\n");
                                string? res = await Program.ReadWithEsc();
                                if (!string.IsNullOrEmpty(res)) 
                                {
                                    res = res.ToLower().Replace(" ", "");
                                    if (res == "y" || res == "yes")
                                    {
                                        //LOG
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