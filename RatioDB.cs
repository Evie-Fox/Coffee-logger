using System.Data.SQLite;

namespace CoffeeLogger
{
    public class RatioDB
    {
        private SQLController dbController;
        public RatioDB(SQLController db)
        {
            this.dbController = db;
        }

        public string[] GetRatiosInBrews(string grinderName, string beansName, string brewerName, string grindSetting)
        {
            using (dbController.db = new SQLiteConnection($"Data Source = {dbController._pathToFile}; Version = 3;"))
            {
                dbController.db.Open();

                using (SQLiteCommand com = new SQLiteCommand(
                    "SELECT GramsPerLiter " +
                    "From Brews " +
                    "WHERE CoffeeBeansName = @coffeeBeansName AND BrewerName = @brewerName AND GrinderName = @grinderName  AND GrindSetting = @grindSetting",
                    dbController.db))
                {
                    
                    com.Parameters.AddWithValue("@grinderName", grinderName);
                    com.Parameters.AddWithValue("@coffeeBeansName", beansName);
                    com.Parameters.AddWithValue("@brewerName",brewerName);
                    com.Parameters.AddWithValue("@grindSetting", grindSetting);
                    SQLiteDataReader data = com.ExecuteReader();

                    List<string> ratios = new List<string>();

                    while (data.Read())
                    {
                        ratios.Add(data["GramsPerLiter"].ToString());
                    }
                    return ratios.ToArray();
                }
            }

        }
    }
}