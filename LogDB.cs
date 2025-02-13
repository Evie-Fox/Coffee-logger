using System.Data.SQLite;

namespace CoffeeLogger
{
    public class LogDB
    {

        private readonly SQLController dbController;

        public LogDB(SQLController dbController) 
        {
            this.dbController = dbController;
        }
        public void AddLog(int brewID, int extractionMeter, int score, string note)
        {
            using (dbController.db = new SQLiteConnection($"Data Source = {dbController._pathToFile}; Version = 3;"))
            {
                dbController.db.Open();


                using (SQLiteCommand com = new SQLiteCommand("INSERT INTO ResultsLog (BrewID, ExtractionMeter, Score, Note, Date) VALUES (@brewID, @extractionMeter, @score, @note, @date)", dbController.db))
                {

                    com.Parameters.AddWithValue("@brewID", brewID);
                    com.Parameters.AddWithValue("@extractionMeter", extractionMeter);
                    com.Parameters.AddWithValue("@score", score);
                    com.Parameters.AddWithValue("@note", note);
                    com.Parameters.AddWithValue("@date", DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                    com.ExecuteNonQuery();
                }
            }
        }
        public int? GetExistingLogID(int brewID)
        {
            using (dbController.db = new SQLiteConnection($"Data Source = {dbController._pathToFile}; Version = 3;"))
            {
                dbController.db.Open();


                using (SQLiteCommand com = new SQLiteCommand(@"
                    SELECT LogID 
                    FROM ResultsLog 
                    WHERE BrewID = @brewID", dbController.db))
                {
                    com.Parameters.AddWithValue("@brewID", brewID);
                    object foo = com.ExecuteScalar();
                    if (foo != null)
                    {
                        return int.Parse(foo.ToString());
                    }
                    return null;
                }
            }
        }
        public void DeleteLog(int logID)
        {

            using (dbController.db = new SQLiteConnection($"Data Source = {dbController._pathToFile}; Version = 3;"))
            {
                dbController.db.Open();

                using (SQLiteCommand com = new SQLiteCommand(@"
                    DELETE FROM ResultsLog 
                    WHERE LogID = @logID", dbController.db))
                {
                    com.Parameters.AddWithValue("@logID", logID);
                    com.ExecuteNonQuery();
                }
            }
        }
    }
}