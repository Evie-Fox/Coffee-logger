using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeDocs
{
    public class SQLController
    {
        private string path = @"..\..\DB\MainDB.db";
        private SQLiteConnection db;

        public void Activate()
        {
            path = Path.GetFullPath(path);
            if (!File.Exists(path))
            {
                Console.WriteLine("DB not found\nCreating new DB");
                CreateDB(path);
            }
            else
            {
                db = new SQLiteConnection($"Data Source = {path}; Version = 3;");
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
            Directory.CreateDirectory(Path.GetFullPath(@"..\..\DB"));
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
            Name VARCHAR(255) PRIMARY KEY
            )", db);

                GrinderTable.ExecuteNonQuery();

                SQLiteCommand GrindSettingsTable = new SQLiteCommand(@"
            CREATE TABLE GrindSettings(
            GrinderName VARCHAR(255) NOT NULL,
            GrindSetting INT NOT NULL,
            PRIMARY KEY ( GrinderName, GrindSetting)
            FOREIGN KEY (GrinderName) REFERENCES Grinders(Name)
            )", db);

                GrindSettingsTable.ExecuteNonQuery();

                SQLiteCommand BrewsTable = new SQLiteCommand(@"
            CREATE TABLE Brews(
            BrewID INTEGER PRIMARY KEY,
            CoffeeBeanID INT NOT NULL,
            BrewerID INT NOT NULL,
            GrindSetting INT NOT NULL,
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

            using (db = new SQLiteConnection($"Data Source = {path}; Version = 3;"))
            {
                db.Open();
                SQLiteCommand com = new SQLiteCommand(@" SELECT name FROM sqlite_master WHERE type='table'", db);

                SQLiteDataReader data = com.ExecuteReader();
                while (data.Read())
                {
                    string tableName = data.GetString(0);
                    if (tableName != "sqlite_sequence")
                    {
                        com = new SQLiteCommand($@"DELETE FROM {tableName}",db);
                        com.ExecuteNonQuery();
                    }
                }
            }
        }

        public string[] GetGrinderNames()
        {
            using (db = new SQLiteConnection($"Data Source = {path}; Version = 3;"))
            {
                db.Open();
                SQLiteCommand com = new SQLiteCommand(@"

                SELECT Name
                FROM Grinders

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
        

        public string[] GetBrewerNames()
        {
            using (db = new SQLiteConnection($"Data Source = {path}; Version = 3;"))
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

        public string[] GetCoffeeNames()
        {
            using (db = new SQLiteConnection($"Data Source = {path}; Version = 3;"))
            {
                db.Open();
                SQLiteCommand com = new SQLiteCommand(@"

                SELECT Name
                FROM CoffeeBeans

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

        public void AddGrinder(string grinderName)
        {
            using (db = new SQLiteConnection($"Data Source = {path}; Version = 3;"))
            {
                db.Open();

                using (SQLiteCommand com = new SQLiteCommand("INSERT INTO Grinders (Name) VALUES (@name)", db))
                {

                    com.Parameters.AddWithValue("@name", grinderName);
                    com.ExecuteNonQuery();
                }
            }
        }

        public void AddBrewer(string brewerName, BrewMethod brewMethod = BrewMethod.None)
        {
            using (db = new SQLiteConnection($"Data Source = {path}; Version = 3;"))
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
        public void AddBean(string beanName, RoastLevel roast)//this it not it
        {
            using (db = new SQLiteConnection($"Data Source = {path}; Version = 3;"))
            {
                db.Open();

                using (SQLiteCommand com = new SQLiteCommand("INSERT INTO CoffeeBeans (Name, RoastLevelEnum) VALUES (@name, @roast)", db))
                {

                    com.Parameters.AddWithValue("@name", beanName);
                    com.Parameters.AddWithValue("@roast", (int)roast);
                    com.ExecuteNonQuery();
                }
            }
        }
        public enum RoastLevel
        {
            None = 0,
            Light = 1,
            Medium = 2,
            Dark = 3
        }
        public enum BrewMethod
        {
            None = 0,
            Percolation = 1,
            Infusion = 2,
            SteepAndRelease = 3
        }
    }
}