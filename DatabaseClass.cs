﻿using System;
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
                                            product_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, 
                                            product_barcode INTEGER NOT NULL UNIQUE, 
                                            product_name VARCHAR(55) NOT NULL UNIQUE,
                                            product_first_add_time TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP);

                                            CREATE TABLE IF NOT EXISTS locations (
                                            location_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, 
                                            location_a VARCHAR(3) NOT NULL,
                                            location_b VARCHAR(3) NOT NULL,
                                            location_c VARCHAR(3) NOT NULL);

                                            CREATE TABLE IF NOT EXISTS stock (
                                            stock_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, 
                                            product_id INTEGER NOT NULL REFERENCES products(product_id) ON UPDATE CASCADE,
                                            location_id INTEGER NOT NULL REFERENCES locations(location_id) ON UPDATE CASCADE,
                                            stock_number INTEGER NOT NULL);

                                            CREATE TABLE IF NOT EXISTS history (
                                            history_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, 
                                            product_id INTEGER NOT NULL REFERENCES products(product_id) ON UPDATE CASCADE,
                                            history_status INTEGER NOT NULL,
                                            stock_number INTEGER NOT NULL,
                                            history_adding_time TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP)
                                            
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
                        cmd.CommandText = @"SELECT * FROM products WHERE product_barcode = @Barcode";
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
                MessageBox.Show("Saved.");
            }
            catch (SQLiteException SQLiteThrow)
            {
                string stringCutted = SQLiteThrow.Message.Split(' ').Last();
                if(SQLiteThrow.ErrorCode == 19)
                {
                    if (Equals(stringCutted, "products.product_name"))
                    {
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

        public static string[] GetLocationList()
        {
            try
            {
                ConnectDatabase();
                string[] locations = new string[3];
                using (con)
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(con))
                    {
                        cmd.CommandText = @"SELECT location_a, location_b, location_c FROM locations";
                        using (SQLiteDataReader rdr = cmd.ExecuteReader())
                        {
                            int i = 0;
                            while (rdr.Read())
                            {
                                locations[0] = locations[0] + rdr.GetString(0);
                                locations[1] = rdr.GetString(1);
                                locations[2] = rdr.GetString(2);
                                i++;
                            }
                            return locations;
                            con.Close();
                        }
                    }
                }
            }
            catch (SQLiteException SQLiteThrow)
            {
                MessageBox.Show("GetLocationList Message: " + SQLiteThrow.Message + "\n");
                return null;
            }
        }

        public static string WhereIsProduct(int Barcode)
        {
            try
            {
                ConnectDatabase();
                string locations = null;
                using (con)
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(con))
                    {
                        cmd.CommandText = @"Select location_a, location_b, location_c from locations WHERE 
                                                location_id IN (SELECT location_id FROM stock WHERE 
                                                    product_id IN (SELECT product_id FROM products WHERE 
                                                        product_barcode = @Barcode))";
                        cmd.Parameters.AddWithValue("Barcode", Barcode);
                        using (SQLiteDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                locations = locations + " " + rdr.GetString(0) + rdr.GetString(1) + rdr.GetString(2);
                            }
                            return locations;
                            con.Close();
                        }
                    }
                }
            }
            catch (SQLiteException SQLiteThrow)
            {
                MessageBox.Show("WhereIsProduct Message: " + SQLiteThrow.Message);
                return null;
            }
        }

        public static string ProductName(int Barcode)
        {
            try
            {
                ConnectDatabase();
                string name = null;
                using (con)
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(con))
                    {
                        cmd.CommandText = @"SELECT product_name FROM products WHERE product_barcode = @Barcode";
                        cmd.Parameters.AddWithValue("Barcode", Barcode);
                        using (SQLiteDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                name = rdr.GetString(0);
                            }
                            return name;
                            con.Close();
                        }
                    }
                }
            }
            catch (SQLiteException SQLiteThrow)
            {
                MessageBox.Show("ProductName Message: " + SQLiteThrow.Message);
                return null;
            }
        }

        public static bool InInvIsExist(int Barcode, string[] Location)
        {
            try
            {
                ConnectDatabase();
                using (con)
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(con))
                    {
                        cmd.CommandText = @"
                            SELECT stock_id FROM stock WHERE 
                                product_id IN (SELECT product_id FROM products WHERE product_barcode = @Barcode)
                                AND
                                location_id IN(SELECT location_id FROM locations WHERE 
                                    location_a = @Location1 AND location_b = @Location2 AND location_c = @Location3)";
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("Barcode", Barcode);
                        cmd.Parameters.AddWithValue("Location1", Location[0]);
                        cmd.Parameters.AddWithValue("Location2", Location[1]);
                        cmd.Parameters.AddWithValue("Location3", Location[2]);
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
                MessageBox.Show("InInvIsExist Message: " + SQLiteThrow.Message + "\n");
                return false;
            }
        }

        public static void InInventoryInsert(int Barcode, int Number, string[] Location)
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

                        cmd.CommandText = @"
                            INSERT INTO stock (product_id, location_id, stock_number) 
                            VALUES 
                                ((SELECT product_id FROM products WHERE product_barcode = @Barcode),
                                (SELECT location_id FROM locations WHERE 
                                    location_a = @Location1 AND location_b = @Location2 AND location_c = @Location3),
                                @Number);

                            INSERT INTO history (product_id, history_status, stock_number) 
                            VALUES 
                                ((SELECT product_id FROM products WHERE product_barcode = @Barcode), 
                                '1', 
                                @Number)
                        ";
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("Barcode", Barcode);
                        cmd.Parameters.AddWithValue("Location1", Location[0]);
                        cmd.Parameters.AddWithValue("Location2", Location[1]);
                        cmd.Parameters.AddWithValue("Location3", Location[2]);
                        cmd.Parameters.AddWithValue("Number", Number);
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
                MessageBox.Show("Saved.");
            }
            catch (SQLiteException SQLiteThrow)
            {
                    MessageBox.Show("InInventoryInsert Message: " + SQLiteThrow.Message);
            }
        }

        public static void InInventoryUpdate(int Barcode, int Number, string[] Location)
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

                        cmd.CommandText = @"
                            UPDATE stock SET stock_number = stock_number + @Number WHERE 
                                stock_id IN (SELECT stock_id FROM stock WHERE 
                                product_id IN (SELECT product_id FROM products WHERE product_barcode = @Barcode)
                                AND
                                location_id IN (SELECT location_id FROM locations WHERE 
                                    location_a = @Location1 AND location_b = @Location2 AND location_c = @Location3));

                            INSERT INTO history (product_id, history_status, stock_number) 
                            VALUES 
                                ((SELECT product_id FROM products WHERE product_barcode = @Barcode), 
                                '1', 
                                @Number)
                        ";
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("Barcode", Barcode);
                        cmd.Parameters.AddWithValue("Location1", Location[0]);
                        cmd.Parameters.AddWithValue("Location2", Location[1]);
                        cmd.Parameters.AddWithValue("Location3", Location[2]);
                        cmd.Parameters.AddWithValue("Number", Number);
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
                MessageBox.Show("Updated.");
            }
            catch (SQLiteException SQLiteThrow)
            {
                MessageBox.Show("InInventoryUpdate Message: " + SQLiteThrow.Message);
            }
        }


    }
}
