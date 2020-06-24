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


        public static void ConnectDatabase()
        {
            try
            {
            con = new SQLiteConnection("Data Source=Databases/Stock.db;Version=3;");
            con.Open();
            }
            catch (SQLiteException SQLiteThrow)
            {
                MessageBox.Show("ConnectDatabase Message: " + SQLiteThrow.Message + "\n");
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
                MessageBox.Show("CreateDB Message: " + SQLiteThrow.Message + "\n");
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
                        cmd.CommandText = @"CREATE TABLE IF NOT EXISTS products (
                                            product_id INTEGER PRIMARY KEY AUTOINCREMENT, 
                                            product_barcode INTEGER NOT NULL UNIQUE, 
                                            product_name VARCHAR(55) NOT NULL UNIQUE,
                                            product_first_add_time TEXT DEFAULT CURRENT_TIMESTAMP NOT NULL);

                                            CREATE TABLE IF NOT EXISTS locations (
                                            location_id INTEGER PRIMARY KEY AUTOINCREMENT, 
                                            location_a VARCHAR(3) NOT NULL,
                                            location_b VARCHAR(3) NOT NULL,
                                            location_c VARCHAR(3) NOT NULL);

                                            CREATE TABLE IF NOT EXISTS stock (
                                            stock_id INTEGER PRIMARY KEY AUTOINCREMENT, 
                                            product_id INTEGER REFERENCES products(product_id) ON UPDATE CASCADE,
                                            location_id INTEGER REFERENCES locations(location_id) ON UPDATE CASCADE,
                                            stock_number INTEGER NOT NULL);

                                            CREATE TABLE IF NOT EXISTS history (
                                            history_id INTEGER PRIMARY KEY AUTOINCREMENT, 
                                            product_id INTEGER REFERENCES products(product_id) ON UPDATE CASCADE,
                                            historyStatus INTEGER NOT NULL,
                                            stock_number INTEGER NOT NULL,
                                            history_adding_time TEXT DEFAULT CURRENT_TIMESTAMP NOT NULL)
                                            
                                            ";

                                            
                    
                        cmd.ExecuteNonQuery();

                        con.Close();
                    }
                    catch (SQLiteException SQLiteThrow)
                    {
                        MessageBox.Show("CreateTable Message: " + SQLiteThrow.Message + "\n");
                    }
                }
            }
        }

        public static bool LocationTableCheck()
        {
            try
            {
                ConnectDatabase();
                using (con)
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(con))
                    {
                        int count;
                        cmd.CommandText = @"SELECT count(*) FROM locations";
                        cmd.Prepare();
                        using (SQLiteDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                count = rdr.GetInt32(0);
                                if (count > 0)
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                            con.Close();
                        }
                    }
                }
            }
            catch (SQLiteException SQLiteThrow)
            {
                MessageBox.Show("LocationTableCheck Message: " + SQLiteThrow.Message + "\n");
                return false;
            }
        }

        public static void LocationTableAddItem(char i, int j, int k)
        {

            try
            {
                ConnectDatabase();
                using (con)
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(con))
                    {
                        SQLiteTransaction transaction = null;
                        transaction = con.BeginTransaction();

                        cmd.CommandText = @"INSERT INTO locations (location_a, location_b, Location_c) 
                                                            VALUES (@Location1, @Location2, @Location3)";
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("Location1", i.ToString());
                        cmd.Parameters.AddWithValue("Location2", j);
                        cmd.Parameters.AddWithValue("Location3", k);
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
            }
            catch (SQLiteException SQLiteThrow)
            {
                MessageBox.Show("LocationTableAddItem Message: " + SQLiteThrow.Message + "\n");
            }
        }

        public static bool IsBarcodeExist(string Barcode)
        {
            try
            {
                ConnectDatabase();
                using (con)
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(con))
                    {
                        cmd.CommandText = @"SELECT * FROM barcodes WHERE barcode = @Barcode";
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("Barcode", Barcode);
                        using (SQLiteDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                if (rdr.GetInt32(0) != null)
                                {
                                    return true;
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                            con.Close();
                        }
                    }
                }
            }
            catch (SQLiteException SQLiteThrow)
            {
                MessageBox.Show("IsBarcodeExist Message: " + SQLiteThrow.Message + "\n");
                return false;
            }
        }

        public static void AddNewProduct(int Barcode, string Name)
        {
            try
            {
                ConnectDatabase();
                using (con)
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(con))
                    {
                        SQLiteTransaction transaction = null;
                        transaction = con.BeginTransaction();

                        cmd.CommandText = @"INSERT INTO products (product_barcode, product_name) 
                                                            VALUES (@Barcode, @Name)";
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("Barcode", Barcode);
                        cmd.Parameters.AddWithValue("Name", Name);
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
            }
            catch (SQLiteException SQLiteThrow)
            {
                string stringCutted = SQLiteThrow.Message.Split(' ').Last();
                if(SQLiteThrow.ErrorCode == 19)
                {
                    if (Equals(stringCutted, "products.product_name"))
                    {//TODO
                        MessageBox.Show("This product name already exist!");
                    }
                    else if(Equals(stringCutted, "products.product_barcode"))
                    {
                        MessageBox.Show("This product barcode already exist!");
                    }
                    else
                    {
                        MessageBox.Show("AddNewProduct Message19: " + SQLiteThrow.Message + "\n");
                    }
                }
                else
                {
                    MessageBox.Show("AddNewProduct Message: " + SQLiteThrow.Message + "\n");
                }
            }
        }

    }
}
