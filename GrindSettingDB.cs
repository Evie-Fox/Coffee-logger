﻿using System.Data.SQLite;

namespace CoffeeLogger
{
    public class GrindSettingDB
    {
        private SQLController dbController;

        public GrindSettingDB(SQLController dbController)
        {
            this.dbController = dbController;
        }

        public void AddGrindSetting(string grinderName, string grindSetting)
        {
            using (dbController.db = new SQLiteConnection($"Data Source = {dbController._pathToFile}; Version = 3;"))
            {
                dbController.db.Open();

                using (SQLiteCommand com = new SQLiteCommand("INSERT INTO GrindSettings (GrinderName, GrindSetting) VALUES (@name, @setting)", dbController.db))
                {
                    com.Parameters.AddWithValue("@name", grinderName);
                    com.Parameters.AddWithValue("@setting", grindSetting);
                    com.ExecuteNonQuery();
                }
            }
        }

        public bool IsGrindSettingTaken(string grinderName, string grindSetting)
        {
            using (dbController.db = new SQLiteConnection($"Data Source = {dbController._pathToFile}; Version = 3;"))
            {
                dbController.db.Open();

                long count;
                using (SQLiteCommand com = new SQLiteCommand("Select COUNT(*) FROM GrindSettings WHERE @name = GrinderName AND @setting = GrindSetting", dbController.db))
                {
                    com.Parameters.AddWithValue("@name", grinderName);
                    com.Parameters.AddWithValue("@setting", grindSetting);
                    count = (long)com.ExecuteScalar();
                }
                return count > 0;
            }
        }
        
        public string?[] GetRegisteredGrindSettingsOrNull(string grinderName, int coffeeBeansID)
        {

            using (dbController.db = new SQLiteConnection($"Data Source = {dbController._pathToFile}; Version = 3;"))
            {
                dbController.db.Open();
                List<string> settings = new();

                using (SQLiteCommand com = new SQLiteCommand("SELECT GrindSetting From Brews WHERE @grinderName = GrinderName"))
                {
                    com.Parameters.AddWithValue("@grinderName", grinderName);
                    SQLiteDataReader data = com.ExecuteReader();

                    while (data.Read())
                    {
                        settings.Add(data["GrindSetting"].ToString());
                    }
                    return settings.ToArray();

                }
            }
        }
    }
}