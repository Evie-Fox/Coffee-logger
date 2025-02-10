using System.Data.SQLite;

namespace CoffeeLogger
{
    public class SQLController
    {
        public GrinderDB Grinders { get; private set; }
        public GrindSettingDB GrindSettings { get; private set; }
        public BrewerDB Brewers { get; private set; }
        public BeanDB Beans { get; private set; }
        public RatioDB Ratio { get; private set; }
        public BrewDB Brew { get; private set; }
        public SQLiteConnection db;

        public string _pathToFile { get; private set; } = @"..\..\..\DB\MainDB.db";

        private string _pathToDir = @"..\..\..\DB\";

        public SQLController()

        {
            Grinders = new GrinderDB(this);
            GrindSettings = new GrindSettingDB(this);
            Brewers = new BrewerDB(this);
            Beans = new BeanDB(this);
            Ratio = new RatioDB(this);
            Brew = new BrewDB(this);
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

                using (SQLiteCommand com = new("PRAGMA foreign_keys = ON", db))
                {
                    com.ExecuteNonQuery();
                }

                SQLiteCommand beansTable = new SQLiteCommand(@"
            CREATE TABLE CoffeeBeans(
            Name VARCHAR(255) PRIMARY KEY,
            RoasterName VARCHAR(255),
            Origin VARCHAR(255),
            RoastLevelEnum INT
            )", db);

                beansTable.ExecuteNonQuery();

                SQLiteCommand brewersTable = new SQLiteCommand(@"
            CREATE TABLE Brewers(
            Name VARCHAR(255) PRIMARY KEY,
            BrewMethodEnum INT NOT NULL
            )", db);

                brewersTable.ExecuteNonQuery();

                SQLiteCommand GrinderTable = new SQLiteCommand(@"
            CREATE TABLE Grinders(
            Name VARCHAR(255) PRIMARY KEY,
            DialFormat VARCHAR(255) NOT NULL
            )", db);

                GrinderTable.ExecuteNonQuery();

                SQLiteCommand BrewsTable = new SQLiteCommand(@"
            CREATE TABLE Brews(
            BrewID INTEGER PRIMARY KEY,
            CoffeeBeansName VARCHAR(255) NOT NULL,
            BrewerName VARCHAR(255) NOT NULL,
            GrindSetting VARCHAR(255) NOT NULL,
            GrinderName VARCHAR(255) NOT NULL,
            GramsPerLiter INT NOT NULL,
            UNIQUE (CoffeeBeansName, BrewerName, GrinderName, GrindSetting, GramsPerLiter),
            FOREIGN KEY (CoffeeBeansName) REFERENCES CoffeeBeans(Name),
            FOREIGN KEY (BrewerName) REFERENCES Brewers(Name),
            FOREIGN KEY (GrinderName) REFERENCES Grinders(Name) ON DELETE CASCADE
            )", db);

                BrewsTable.ExecuteNonQuery();

                SQLiteCommand LogsTable = new SQLiteCommand(@"
            CREATE TABLE ResultsLog(
            BrewID INT PRIMARY KEY,
            ExtractionMeter INT NOT NULL,
            Score INT NOT NULL,
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

        public bool IsBrewTaken(string coffeeBeansName, string brewerName, string grinderName, string grindSetting)
        {
            using (db = new SQLiteConnection($"Data Source = {_pathToFile}; Version = 3;"))
            {
                db.Open();

                using (SQLiteCommand com = new SQLiteCommand(@"SELECT COUNT(*) FROM Brews WHERE @bean = CoffeeBeansName AND @brewer = BrewerName AND @grinder = GrinderName AND @grindSetting = GrindSetting", db))
                {
                    com.Parameters.AddWithValue("@bean", coffeeBeansName);
                    com.Parameters.AddWithValue("@brewer", brewerName);
                    com.Parameters.AddWithValue("@grinder", grinderName);
                    com.Parameters.AddWithValue("@grindSetting", grindSetting);
                    long count = (long)com.ExecuteScalar();
                    return count > 0;
                }
            }
        }
    }
}