using System.Data.SQLite;

namespace CoffeeLogger
{
    public class BeanDB
    {


        public void AddBean(string beanName, string? rosterName, string? origin, RoastLevel roast = RoastLevel.None)//this it not it
        {
            using (db = new SQLiteConnection($"Data Source = {_pathToFile}; Version = 3;"))
            {
                db.Open();

                using (SQLiteCommand com = new SQLiteCommand("INSERT INTO CoffeeBeans (Name, RoasterName, Origin, RoastLevelEnum) VALUES (@name, @roasterName, @origin,@roast)", db))
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
            using (db = new SQLiteConnection($"Data Source = {_pathToFile}; Version = 3;"))
            {
                db.Open();
                SQLiteCommand com = new SQLiteCommand(@"

                SELECT Name
                FROM CoffeeBeans

                ", db);
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