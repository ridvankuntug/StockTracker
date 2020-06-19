using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace StockTracker
{
    public class DatabaseClass
    {
        public static SQLiteConnection con = null;

        public DatabaseClass()
        {

        }

        public static void ConnectDatabase()
        {
            try
            {
            con = new SQLiteConnection("Data Source=Databases/Stock.db;Version=3;");
            con.Open();
            }
            catch (SQLiteException SQLiteThrow)
            {
                MessageBox.Show("Message: " + SQLiteThrow.Message + "\n");
            }
        }

        public static void CreateDB()
        {
            try
            {
                if (!File.Exists("Databases/Stock.db"))
                {
                    if (!Directory.Exists("Databases"))
                    {
                        Directory.CreateDirectory("Databases");
                        Console.WriteLine("Director created.");
                    }
                    else
                    {
                        Console.WriteLine("Director already exist.");
                    }

                    SQLiteConnection.CreateFile("Databases/Stock.db");
                    Console.WriteLine("Database created.");
                }
                else
                {
                    Console.WriteLine("Database already exists.");
                }
            }
            catch (SQLiteException SQLiteThrow)
            {
                MessageBox.Show("Message: " + SQLiteThrow.Message + "\n");
            }
        }
        
        public static void CreateTable()
        {
            ConnectDatabase();
            using (con)
            {
                using (SQLiteCommand cmd = new SQLiteCommand(con))
                {
                    try
                    {
                        cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Barcodes (
                                            BarcodeID INTEGER PRIMARY KEY AUTOINCREMENT, 
                                            Barcode INTEGER NOT NULL UNIQUE);

                                            CREATE TABLE IF NOT EXISTS Inventory (
                                            InventoryID INTEGER PRIMARY KEY AUTOINCREMENT, 
                                            InventoryName VARCHAR(55) NOT NULL UNIQUE,
                                            InventoryAddingTime TEXT DEFAULT CURRENT_TIMESTAMP NOT NULL);

                                            CREATE TABLE IF NOT EXISTS Location (
                                            LocationID INTEGER PRIMARY KEY AUTOINCREMENT, 
                                            LocationA varchar(3) NOT NULL,
                                            LocationB varchar(3) NOT NULL,
                                            LocationC varchar(3) NOT NULL);

                                            CREATE TABLE IF NOT EXISTS Stock (
                                            StockID INTEGER PRIMARY KEY AUTOINCREMENT, 
                                            InventoryID INTEGER NOT NULL,
                                            LocationID INTEGER NOT NULL,
                                            StockNumber INTEGER NOT NULL);

                                            CREATE TABLE IF NOT EXISTS History (
                                            HistoryID INTEGER PRIMARY KEY AUTOINCREMENT, 
                                            InventoryID INTEGER NOT NULL,
                                            HistoryStatus INTEGER NOT NULL,
                                            StockNumber INTEGER NOT NULL,
                                            HistoryAddingTime TEXT DEFAULT CURRENT_TIMESTAMP NOT NULL)
                                            
                                            ";

                                            
                    
                        cmd.ExecuteNonQuery();

                        con.Close();
                    }
                    catch (SQLiteException SQLiteThrow)
                    {
                        MessageBox.Show("Message: " + SQLiteThrow.Message + "\n");
                    }
                }
            }
        }
    }
}
