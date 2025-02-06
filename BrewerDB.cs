using System.Data.SQLite;

namespace CoffeeLogger
{
    public class BrewerDB
    {
        public void AddBrewer(string brewerName, BrewMethod brewMethod = BrewMethod.None)
        {
            using (db = new SQLiteConnection($"Data Source = {_pathToFile}; Version = 3;"))
            {
                db.Open();

                using (SQLiteCommand com = new SQLiteCommand("INSERT INTO Brewers (Name, BrewMethodEnum) VALUES (@name, @method)", db))
                {
                    com.Parameters.AddWithValue("@name", brewerName);
                    com.Parameters.AddWithValue("@method", brewMethod);
                    com.ExecuteNonQuery();
                }
            }
        }

        public string[] GetBrewerNames()
        {
            using (db = new SQLiteConnection($"Data Source = {_pathToFile}; Version = 3;"))
            {
                db.Open();
                SQLiteCommand com = new SQLiteCommand(@"

                SELECT Name
                FROM Brewers

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