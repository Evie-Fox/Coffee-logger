
namespace CoffeeLogger
{
    public class BrewManager
    {
        private SQLController db;
        private GrinderManager grinderMan;
        private CoffeeBeansManager coffeeBeansMan;
        private BrewerManager brewerMan;
        public BrewManager(SQLController db, GrinderManager grinderMan, CoffeeBeansManager coffeeBeansMan, BrewerManager brewerMan)
        {
            this.db = db;
            this.grinderMan = grinderMan;
            this.coffeeBeansMan = coffeeBeansMan;
            this.brewerMan = brewerMan;
        }

    }
}
