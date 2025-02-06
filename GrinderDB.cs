using System.Data.SQLite;

namespace CoffeeLogger
{
    public class GrinderDB
    {
        public void AddGrinder(string grinderName, string format)
        {
            using (db = new SQLiteConnection($"Data Source = {_pathToFile}; Version = 3;"))
            {
                db.Open();

                using (SQLiteCommand com = new SQLiteCommand("INSERT INTO Grinders (Name, DialFormat) VALUES (@name, @format)", db))
                {

                    com.Parameters.AddWithValue("@name", grinderName);
                    com.Parameters.AddWithValue("@format", format);
                    com.ExecuteNonQuery();
                }
            }
        }


        public string GetGrinderDialFormat(string grinderName)
        {
            using (db = new SQLiteConnection($"Data Source = {_pathToFile}; Version = 3;"))
            {
                db.Open();
                SQLiteCommand com = new SQLiteCommand(@$"
                
                SELECT DialFormat
                FROM Grinders
                WHERE Name = @grinderName
                
                ", db);

                com.Parameters.AddWithValue("@grinderName", grinderName);

                SQLiteDataReader data = com.ExecuteReader();
                string result = string.Empty;
                while (data.Read())
                {
                    result = data["DialFormat"].ToString();
                }
                return result;

            }
        }

        public string[] GetGrinderNames()
        {
            using (db = new SQLiteConnection($"Data Source = {_pathToFile}; Version = 3;"))
            {
                db.Open();
                SQLiteCommand com = new SQLiteCommand(@"

                SELECT Name
                FROM Grinders

                ", db);
                SQLiteDataReader data = com.ExecuteReader(); //<here it crushes
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