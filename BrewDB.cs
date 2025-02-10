using System.Data.SQLite;

namespace CoffeeLogger
{
    public class BrewDB
    {
        private readonly SQLController dbController;

        public BrewDB(SQLController dbController)
        {
            this.dbController = dbController;
        }

        public int GetBrewID(string grinderName, string coffeeBeansName, string brewerName, string grindSetting, string gramsPerLiter)
        {
            using (dbController.db = new SQLiteConnection($"Data Source = {dbController._pathToFile}; Version = 3;"))
            {
                dbController.db.Open();

                using (SQLiteCommand com = new SQLiteCommand(
                    "Select BrewID " +
                    "FROM Brews " +
                    "WHERE BrewerName = @brewerName " +
                    "AND CoffeeBeansName = @coffeeBeansName " +
                    "AND GrinderName = @grinderName " +
                    "AND GrindSetting = @grindSetting " +
                    "AND GramsPerLiter = @gramsPerLiter ",
                    dbController.db))
                {
                    com.Parameters.AddWithValue("@grinderName", grinderName);
                    com.Parameters.AddWithValue("@coffeeBeansName", coffeeBeansName);
                    com.Parameters.AddWithValue("@brewerName", brewerName);
                    com.Parameters.AddWithValue("@grindSetting", grindSetting);
                    com.Parameters.AddWithValue("@gramsPerLiter", gramsPerLiter);
                    SQLiteDataReader data = com.ExecuteReader();

                    string id = "";
                    while (data.Read())
                    {
                        id = data["BrewID"].ToString();
                    }
                    if (string.IsNullOrEmpty(id))
                    {
                        return -1;
                    }
                    return int.Parse(id);
                }
            }
        }

        public void InsertBrewToDB(string grinderName, string coffeeBeansName, string brewerName, string grindSetting, string gramsPerLiter)
        {
            using (dbController.db = new SQLiteConnection($"Data Source = {dbController._pathToFile}; Version = 3;"))
            {
                dbController.db.Open();

                using (SQLiteCommand com = new SQLiteCommand(
                    "INSERT INTO Brews (BrewerName, CoffeeBeansName, GrinderName, GrindSetting, GramsPerLiter) " +
                    "VALUES (@brewerName, @coffeeBeansName, @grinderName, @grindSetting, @gramsPerLiter)",
                    dbController.db))
                {
                    com.Parameters.AddWithValue("@grinderName", grinderName);
                    com.Parameters.AddWithValue("@coffeeBeansName", coffeeBeansName);
                    com.Parameters.AddWithValue("@brewerName", brewerName);
                    com.Parameters.AddWithValue("@grindSetting", grindSetting);
                    com.Parameters.AddWithValue("@gramsPerLiter", gramsPerLiter);
                    com.ExecuteNonQuery();
                }
            }
        }
    }
}