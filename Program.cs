using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SQLite;
using CoffeeDocs;
using System.Xml.Linq;


namespace CoffeeLogger
{
    public class Program
    {
        public SQLController db;

        public Program()
        {
            db = new SQLController();
            db.Activate();
            Init();

        }

        private void Init()
        {

        }


        private void ClearDB()
        {
            Console.WriteLine("CLEAR DB");
            db.ClearDB();
        }

        private void AddNewGrinder(string name)
        {
            string grinderName = name;
            string[] grinderNamesOnDB = db.GetGrinderNames();

            if (grinderNamesOnDB.Contains(grinderName))
            {
                Console.WriteLine("Grinder name is taken on the DB");
                return;
            }

            db.AddGrinder(grinderName);
            Console.WriteLine($"Grinders on DB: {string.Join(", ", db.GetGrinderNames())}");
        }

        private void AddNewBrewer(string name)
        {
            string brewerName = name;
            string[] brewerNames = db.GetBrewerNames();

            if (brewerNames.Contains(brewerName))
            {
                Console.WriteLine("Brewer name is taken on the DB");
                return;
            }

            db.AddBrewer(brewerName);
        }

        private void AddNewCoffee(string name)//Nothing here
        {
            string coffeeBeanName = name;
            string[] coffeeNames = db.GetCoffeeNames();

            if (coffeeNames.Contains(coffeeBeanName))
            {
                Console.WriteLine("Coffee bean name is taken on the DB");
                return;
            }
            db.AddBean(coffeeBeanName, SQLController.RoastLevel.None);
        }
    }
}
