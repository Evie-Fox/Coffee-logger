namespace CoffeeLogger
{
    public class BrewerManager
    {

        private void AddNewBrewer(string name)
        {
            BrewerNameIsTaken(name);
            db.AddBrewer(name);
        }

        private void BrewerNameIsTaken(string brewerName)
        {
            string[] brewerNames = db.GetBrewerNames();

            if (brewerNames.Contains(brewerName))
            {
                Console.WriteLine("Brewer name is taken on the DB");
                return;
            }
        }
    }
}