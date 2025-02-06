using System.Data.SQLite;

namespace CoffeeLogger
{
    public class BeanDB
    {
        private SQLController dbController;

        public BeanDB(SQLController dbController)
        {
            this.dbController = dbController;
        }

        public void AddBean(string beanName, string? rosterName, string? origin, RoastLevel roast = RoastLevel.None)//this it not it
        {
            using (dbController.db = new SQLiteConnection($"Data Source = {dbController._pathToFile}; Version = 3;"))
            {
                dbController.db.Open();

                using (SQLiteCommand com = new SQLiteCommand("INSERT INTO CoffeeBeans (Name, RoasterName, Origin, RoastLevelEnum) VALUES (@name, @roasterName, @origin,@roast)", dbController.db))
                {

                    com.Parameters.AddWithValue("@name", beanName);
                    com.Parameters.AddWithValue("@roasterName", rosterName);
                    com.Parameters.AddWithValue("@origin", origin);
                    com.Parameters.AddWithValue("@roast", (int)roast);
                    com.ExecuteNonQuery();
                }
            }
        }

        public string[] GetCoffeeNames()
        {
            using (dbController.db = new SQLiteConnection($"Data Source = {dbController._pathToFile}; Version = 3;"))
            {
                dbController.db.Open();
                SQLiteCommand com = new SQLiteCommand(@"SELECT Name FROM CoffeeBeans", dbController.db);
                SQLiteDataReader data = com.ExecuteReader();
                List<string> names = new List<string>();
                while (data.Read())
                {
                    names.Add(data["Name"].ToString());
                }
                return names.ToArray();
            }
        }
    }
}