﻿using System.Data.SQLite;

namespace CoffeeLogger
{
    public class SQLController
    {
        public GrinderDB Grinders;
        public GrindSettingDB GrindSettings;
        public BrewerDB Brewers;
        public BeanDB Beans;

        private string _pathToDir = @"..\..\..\DB\";
        private string _pathToFile = @"..\..\..\DB\MainDB.db";
        private SQLiteConnection db;

        public SQLController()

        {
            Grinders = new GrinderDB();
            GrindSettings = new GrindSettingDB();
            Brewers = new BrewerDB();
            Beans = new BeanDB();
        }

        public void Activate()
        {
            _pathToFile = Path.GetFullPath(_pathToFile);
            if (!File.Exists(_pathToFile))
            {
                Console.WriteLine("DB not found\nCreating new DB");
                CreateDB(_pathToFile);
            }
            else
            {
                db = new SQLiteConnection($"Data Source = {_pathToFile}; Version = 3;");
            }

            //use it
        }

        public string WriteAllFileNames(string path)
        {
            if (!Directory.Exists(path))
            { return "Directory doesn't exist"; }
            string[] files = Directory.GetFiles(path);
            int length = files.Length;

            string[] names = new string[length];
            for (int i = 0; i < length; i++)
            {
                names[i] = Path.GetFileName(files[i]);
            }

            return ("Files in " + path + " : " + string.Join(", ", names));
        }

        public void CreateDB(string path)
        {
            Directory.CreateDirectory(Path.GetFullPath(_pathToDir));
            SQLiteConnection.CreateFile(path);

            using (db = new SQLiteConnection($"Data Source = {path}; Version = 3;"))
            {
                db.Open();

                SQLiteCommand beansTable = new SQLiteCommand(@"
            CREATE TABLE CoffeeBeans(
            CoffeeBeanID INTEGER PRIMARY KEY,
            Name VARCHAR(255) NOT NULL,
            RoasterName VARCHAR(255),
            Origin VARCHAR(255),
            RoastLevelEnum INT
            )", db);

                beansTable.ExecuteNonQuery();

                SQLiteCommand brewersTable = new SQLiteCommand(@"
            CREATE TABLE Brewers(
            BrewerID INTEGER PRIMARY KEY,
            Name VARCHAR(255) UNIQUE NOT NULL,
            BrewMethodEnum INT NOT NULL
            )", db);

                brewersTable.ExecuteNonQuery();

                SQLiteCommand GrinderTable = new SQLiteCommand(@"
            CREATE TABLE Grinders(
            Name VARCHAR(255) PRIMARY KEY,
            DialFormat VARCHAR(255) NOT NULL
            )", db);

                GrinderTable.ExecuteNonQuery();

                SQLiteCommand GrindSettingsTable = new SQLiteCommand(@"
            CREATE TABLE GrindSettings(
            GrinderName VARCHAR(255) NOT NULL,
            GrindSetting VARCHAR(255) NOT NULL,
            PRIMARY KEY (GrinderName, GrindSetting)
            FOREIGN KEY (GrinderName) REFERENCES Grinders(Name) ON DELETE CASCADE
            )", db);

                GrindSettingsTable.ExecuteNonQuery();

                SQLiteCommand BrewsTable = new SQLiteCommand(@"
            CREATE TABLE Brews(
            BrewID INTEGER PRIMARY KEY,
            CoffeeBeanID INT NOT NULL,
            BrewerID INT NOT NULL,
            GrindSetting VARCHAR(255) NOT NULL,
            GrinderName VARCHAR(255) NOT NULL,
            GramsPerLiter INT NOT NULL,
            UNIQUE (CoffeeBeanID, BrewerID, GrinderName, GrindSetting)
            FOREIGN KEY (CoffeeBeanID) REFERENCES CoffeeBeans(CoffeeBeanID),
            FOREIGN KEY (BrewerID) REFERENCES Brewers(BrewerID),
            FOREIGN KEY (GrinderName,GrindSetting) REFERENCES GrindSettings(GrinderName, GrindSetting)
            )", db);

                BrewsTable.ExecuteNonQuery();

                SQLiteCommand LogsTable = new SQLiteCommand(@"
            CREATE TABLE ResultsLog(
            BrewID INT PRIMARY KEY,
            ExtractionMeter INT NOT NULL,
            Temperature INT NOT NULL,
            TastingNotes VARCHAR(255) DEFAULT NULL,
            Date Int NOT NULL,
            FOREIGN KEY (BrewID) REFERENCES Brews(BrewID)
            )", db);

                LogsTable.ExecuteNonQuery();
            }
        }

        public void ClearDB()
        {
            using (db = new SQLiteConnection($"Data Source = {_pathToFile}; Version = 3;"))
            {
                db.Open();
                SQLiteCommand com = new SQLiteCommand(@" SELECT name FROM sqlite_master WHERE type='table'", db);

                SQLiteDataReader data = com.ExecuteReader();
                while (data.Read())
                {
                    string tableName = data.GetString(0);
                    if (tableName != "sqlite_sequence")
                    {
                        com = new SQLiteCommand($@"DELETE FROM {tableName}", db);
                        com.ExecuteNonQuery();
                    }
                }
            }
        }

        public bool IsBrewTaken(int CoffeeBeanID, int BrewerID, string GrinderName, string GrindSetting)
        {
            using (db = new SQLiteConnection($"Data Source = {_pathToFile}; Version = 3;"))
            {
                db.Open();

                using (SQLiteCommand com = new SQLiteCommand("SELECT COUNT(*) FROM Brews WHERE @beanID = CoffeeBeanID AND @brewerID = BrewerID AND @grinderName = GrinderName AND @grindSetting = GrindSetting"))
                {
                    com.Parameters.AddWithValue("@beanID", CoffeeBeanID);
                    com.Parameters.AddWithValue("@brewerID", BrewerID);
                    com.Parameters.AddWithValue("@grinderName", GrinderName);
                    com.Parameters.AddWithValue("@grindSetting", GrindSetting);
                    long count = (long)com.ExecuteScalar();
                    return count > 0;
                }
            }
        }
    }
}