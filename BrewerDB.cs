using System.Data.SQLite;

namespace CoffeeLogger
{
    public class BrewerDB
    {
        private readonly SQLController dbController;

        public BrewerDB(SQLController dbController)
        {
            this.dbController = dbController;
        }

        public void AddBrewer(string brewerName, BrewMethod brewMethod = BrewMethod.None)
        {
            using (dbController.db = new SQLiteConnection($"Data Source = {dbController._pathToFile}; Version = 3;"))
            {
                dbController.db.Open();

                using (SQLiteCommand com = new SQLiteCommand("INSERT INTO Brewers (Name, BrewMethodEnum) VALUES (@name, @method)", dbController.db))
                {
                    com.Parameters.AddWithValue("@name", brewerName);
                    com.Parameters.AddWithValue("@method", brewMethod);
                    com.ExecuteNonQuery();
                }
            }
        }

        public string[] GetBrewerNames()
        {
            using (dbController.db = new SQLiteConnection($"Data Source = {dbController._pathToFile}; Version = 3;"))
            {
                dbController.db.Open();
                SQLiteCommand com = new SQLiteCommand(@" SELECT Name FROM Brewers", dbController.db);
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