namespace CoffeeLogger
{
    public class CoffeeBeansManager
    {

        private void AddNewCoffee(string name, string? roasterName = null, string? origin = null, RoastLevel roast = RoastLevel.None)//Nothing here
        {
            CoffeeNameIsTaken(name);
            db.AddBean(name, roasterName, origin, RoastLevel.None);
        }

        private void CoffeeNameIsTaken(string coffeeBeanName)
        {
            string[] coffeeNames = db.GetCoffeeNames();
            if (coffeeNames.Contains(coffeeBeanName))
            {
                Console.WriteLine("Coffee bean name is taken on the DB");
                return;
            }
        }
    }
}