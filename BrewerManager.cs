namespace CoffeeLogger
{
    public class BrewerManager
    {
        private SQLController db;
        public BrewerManager(SQLController dbRef) 
        {
            db = dbRef;
        }


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