using System.Data.SQLite;

namespace CoffeeLogger
{
    public class GrinderDB
    {
        private readonly SQLController dbController;

        public GrinderDB(SQLController db)
        {
            dbController = db;
        }
        public void AddGrinder(string grinderName, string format)
        {
            using (dbController.db = new SQLiteConnection($"Data Source = {dbController._pathToFile}; Version = 3;"))
            {
                dbController.db.Open();

                using (SQLiteCommand com = new SQLiteCommand("INSERT INTO Grinders (Name, DialFormat) VALUES (@name, @format)", dbController.db))
                {

                    com.Parameters.AddWithValue("@name", grinderName);
                    com.Parameters.AddWithValue("@format", format);
                    com.ExecuteNonQuery();
                }
            }
        }


        public string GetGrinderDialFormat(string grinderName)
        {
            using (dbController.db = new SQLiteConnection($"Data Source = {dbController._pathToFile}; Version = 3;"))
            {
                dbController.db.Open();
                SQLiteCommand com = new SQLiteCommand(@$"
                
                SELECT DialFormat
                FROM Grinders
                WHERE Name = @grinderName
                
                ", dbController.db);

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
            using (dbController.db = new SQLiteConnection($"Data Source = {dbController._pathToFile}; Version = 3;"))
            {
                dbController.db.Open();
                SQLiteCommand com = new SQLiteCommand(@"

                SELECT Name
                FROM Grinders

                ", dbController.db);
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