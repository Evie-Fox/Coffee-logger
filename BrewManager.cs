using System.ComponentModel;

namespace CoffeeLogger
{
    public class BrewManager
    {
        private SQLController db;
        private GrinderManager grinderMan;
        private GrindSettingManager grindMan;
        private CoffeeBeansManager coffeeBeansMan;
        private BrewerManager brewerMan;

        public BrewManager(SQLController db, GrinderManager grinderMan, GrindSettingManager grindMan, CoffeeBeansManager coffeeBeansMan, BrewerManager brewerMan)
        {
            this.db = db;
            this.grinderMan = grinderMan;
            this.coffeeBeansMan = coffeeBeansMan;
            this.grindMan = grindMan;
            this.brewerMan = brewerMan;
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
                        Console.WriteLine("!!");
                        while (true)
                        {
                            string? grindSetting = await grindMan.ChooseOrAddGrindSetting(grinderName, coffeeBeansName, brewerName);
                            if (grindSetting == null)
                            { break; }
                            while (true)
                            {
                                Console.WriteLine("reached end!!");
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}