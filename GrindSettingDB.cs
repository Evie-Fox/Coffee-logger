using System.Data.SQLite;

namespace CoffeeLogger
{
    public class GrindSettingDB
    {
        public void AddGrindSetting(string grinderName, string grindSetting)
        {
            using (db = new SQLiteConnection($"Data Source = {_pathToFile}; Version = 3;"))
            {
                db.Open();

                using (SQLiteCommand com = new SQLiteCommand("INSERT INTO GrindSettings (GrinderName, GrindSetting) VALUES (@name, @setting)", db))
                {
                    com.Parameters.AddWithValue("@name", grinderName);
                    com.Parameters.AddWithValue("@setting", grindSetting);
                    com.ExecuteNonQuery();
                }
            }
        }

        public bool IsGrindSettingTaken(string grinderName, string grindSetting)
        {
            using (db = new SQLiteConnection($"Data Source = {_pathToFile}; Version = 3;"))
            {
                db.Open();

                long count;
                using (SQLiteCommand com = new SQLiteCommand("Select COUNT(*) FROM GrindSettings WHERE @name = GrinderName AND @setting = GrindSetting", db))
                {
                    com.Parameters.AddWithValue("@name", grinderName);
                    com.Parameters.AddWithValue("@setting", grindSetting);
                    count = (long)com.ExecuteScalar();
                }
                return count > 0;
            }
        }
    }
}