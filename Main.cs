using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Data.SQLite;


namespace CoffeeLogger
{
    public class Main
    {
        public SQLController db;

        public MainWindow()
        {
            db = new SQLController();
            db.Activate();
            Init();

        }

        private void Init()
        {

        }


        private void ClearDB(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine("CLEAR DB");
            db.ClearDB();
        }

        private void AddNewGrinder(object sender, MouseButtonEventArgs e)
        {
            string grinderName = textBox.Text.Trim();
            string[] grinderNamesOnDB = db.GetGrinderNames();

            if (grinderNamesOnDB.Contains(grinderName))
            {
                Console.WriteLine("Grinder name is taken on the DB");
                return;
            }

            db.AddGrinder(grinderName);
            Console.WriteLine($"Grinders on DB: {string.Join(", ", db.GetGrinderNames())}");
        }

        private void AddNewBrewer(object sender, MouseButtonEventArgs e)
        {
            string brewerName = textBox.Text.Trim();
            string[] brewerNames = db.GetBrewerNames();

            if (brewerNames.Contains(brewerName))
            {
                Console.WriteLine("Brewer name is taken on the DB");
                return;
            }

            db.AddBrewer(brewerName);
        }

        private void AddNewCoffee(object sender, MouseButtonEventArgs e)//Nothing here
        {
            string coffeeBeanName = textBox.Text.Trim();
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