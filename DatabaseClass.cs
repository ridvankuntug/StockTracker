using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;
using System.Data;
using System.Reflection.Emit;

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
                    cmd.Connection = con;
                    SQLiteTransaction myTrans;

                    // Start a local transaction
                    myTrans = con.BeginTransaction();
                    // Assign transaction object for a pending local transaction
                    cmd.Transaction = myTrans;

                    try
                    {
                        cmd.CommandText = @"
                            CREATE TABLE IF NOT EXISTS products (
                            product_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, 
                            product_barcode INTEGER NOT NULL UNIQUE, 
                            product_name VARCHAR(55) NOT NULL UNIQUE,
                            product_first_add_time TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP);

                            CREATE TABLE IF NOT EXISTS locations (
                            location_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, 
                            location_a VARCHAR(3) NOT NULL,
                            location_b VARCHAR(3) NOT NULL,
                            location_c VARCHAR(3) NOT NULL,
                            location VARCHAR(3) NOT NULL);

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
                            location_id INTEGER NOT NULL REFERENCES locations(location_id) ON UPDATE CASCADE,
                            history_adding_time TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP);

                            CREATE VIEW IF NOT EXISTS history_view
                            AS
                            SELECT product_name, product_barcode, history_status, stock_number, location, history_adding_time FROM history
                            JOIN products ON products.product_id = history.product_id
                            JOIN locations ON locations.location_id = history.location_id;

                            CREATE VIEW IF NOT EXISTS in_history_view
                            AS
                            SELECT product_name, product_barcode, stock_number, location, history_adding_time FROM history
                            JOIN products ON products.product_id = history.product_id
                            JOIN locations ON locations.location_id = history.location_id
                            WHERE history_status = '1';

                            CREATE VIEW IF NOT EXISTS out_history_view
                            AS
                            SELECT product_name, product_barcode, stock_number, location, history_adding_time FROM history
                            JOIN products ON products.product_id = history.product_id
                            JOIN locations ON locations.location_id = history.location_id
                            WHERE history_status = '0';
                                            
                            CREATE VIEW IF NOT EXISTS stock_view
                            AS
                            SELECT product_name, product_barcode, SUM(stock_number), GROUP_CONCAT(location ,', ') FROM stock 
                            JOIN products ON products.product_id = stock.product_id
                            JOIN locations ON locations.location_id = stock.location_id 
                            GROUP BY product_barcode;
                            

                            ";
                                            
                        cmd.ExecuteNonQuery();
                        myTrans.Commit();
                    }
                    catch (SQLiteException SQLiteThrow)
                    {
                        myTrans.Rollback();
                        MessageBox.Show("CreateTable Message: " + SQLiteThrow.Message + "\n");
                    }
                    finally
                    {
                        con.Close();
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

                        cmd.CommandText = @"INSERT INTO locations (location_a, location_b, Location_c, location) 
                                            VALUES (@Location1, @Location2, @Location3, @Location)";
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("Location1", i.ToString());
                        cmd.Parameters.AddWithValue("Location2", j);
                        cmd.Parameters.AddWithValue("Location3", k);
                        cmd.Parameters.AddWithValue("Location", i.ToString() + j + k);
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

        public static DataSet GridFill(string columns, string viewTable, string searchArgName, string searchArgBarcode, string date, string groupBy)
        {
            try
            {
                ConnectDatabase();
                using (con)
                {
                    string cmdText;

                    if (groupBy == "")
                    {
                        cmdText = string.Format("SELECT " + columns + " FROM " + viewTable + " WHERE product_name LIKE '" + searchArgName + "%' AND product_barcode LIKE '" + searchArgBarcode + "%'" + date);
                    }
                    else
                    {
                        cmdText = string.Format("SELECT " + columns + " FROM " + viewTable + " WHERE product_name LIKE '" + searchArgName + "%' AND product_barcode LIKE '" + searchArgBarcode + "%'" + date + groupBy);

                    }

                    using (SQLiteDataAdapter cmd = new SQLiteDataAdapter(cmdText, con))
                    {
                        using (DataSet ds = new DataSet())
                        {
                            cmd.Fill(ds, "*");
                            return ds;
                con.Close();
                        }
                    }
                }
            }
            catch (SQLiteException SQLiteThrow)
            {
                MessageBox.Show("GridFill Message: " + SQLiteThrow.Message);
                return null;
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

                            INSERT INTO history (product_id, history_status, location_id, stock_number) 
                            VALUES 
                                ((SELECT product_id FROM products WHERE product_barcode = @Barcode), 
                                '1', 
                                (SELECT location_id FROM locations WHERE 
                                    location_a = @Location1 AND location_b = @Location2 AND location_c = @Location3),
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

                            INSERT INTO history (product_id, history_status, location_id, stock_number) 
                            VALUES 
                                ((SELECT product_id FROM products WHERE product_barcode = @Barcode), 
                                '1', 
                                (SELECT location_id FROM locations WHERE 
                                    location_a = @Location1 AND location_b = @Location2 AND location_c = @Location3),
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

        public static List<List<string>> GetStockLocations(int Barcode)
        {
            try
            {
                ConnectDatabase();
                List<string> location = new List<string>();
                List<string> number = new List<string>();
                using (con)
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(con))
                    {
                        cmd.CommandText = @"SELECT location, stock_number FROM locations 
                            JOIN stock ON stock.location_id = locations.location_id
                            WHERE stock.product_id IN (SELECT product_id FROM products WHERE product_barcode = @Barcode)
                        ";
                        cmd.Parameters.AddWithValue("Barcode", Barcode);
                        using (SQLiteDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                location.Add(rdr.GetString(0));
                                number.Add(rdr.GetInt32(1).ToString());
                            }

                            List<List<string>> locations = new List<List<string>> {location, number};
                            return locations;
                            con.Close();
                        }
                    }
                }
            }
            catch (SQLiteException SQLiteThrow)
            {
                MessageBox.Show("GetStockLocations Message: " + SQLiteThrow.Message + "\n");
                return null;
            }
        }

        public static bool OutInvIsExist(int Barcode, string Location)
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
                                    location = @Location)";
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("Barcode", Barcode);
                        cmd.Parameters.AddWithValue("Location", Location);
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
                MessageBox.Show("OutInvIsExist Message: " + SQLiteThrow.Message + "\n");
                return false;
            }
        }

        public static bool EnoughInStock(int Barcode, string Location, int Number)
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
                                    location = @Location)
                                AND stock_number > @number
                        ";
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("Barcode", Barcode);
                        cmd.Parameters.AddWithValue("Location", Location);
                        cmd.Parameters.AddWithValue("Number", Number);
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
                MessageBox.Show("EnoughInStock Message: " + SQLiteThrow.Message + "\n");
                return false;
            }
        }

        public static bool IsStockZero(int Barcode, string Location, int Number)
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
                                    location = @Location)
                                AND stock_number = @number
                        ";
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("Barcode", Barcode);
                        cmd.Parameters.AddWithValue("Location", Location);
                        cmd.Parameters.AddWithValue("Number", Number);
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
                MessageBox.Show("IsStockZero Message: " + SQLiteThrow.Message + "\n");
                return false;
            }
        }

        public static void OutInventoryUpdate(int Barcode, int Number, string Location)
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
                            UPDATE stock SET stock_number = stock_number - @Number WHERE 
                                stock_id IN (SELECT stock_id FROM stock WHERE 
                                product_id IN (SELECT product_id FROM products WHERE product_barcode = @Barcode)
                                AND
                                location_id IN (SELECT location_id FROM locations WHERE 
                                    location = @Location));

                            INSERT INTO history (product_id, history_status, location_id, stock_number) 
                            VALUES 
                                ((SELECT product_id FROM products WHERE product_barcode = @Barcode), 
                                '0', 
                                (SELECT location_id FROM locations WHERE 
                                    location = @Location),
                                @Number)
                        ";
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("Barcode", Barcode);
                        cmd.Parameters.AddWithValue("Location", Location);
                        cmd.Parameters.AddWithValue("Number", Number);
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
                MessageBox.Show("Updated Out Inventory.");
            }
            catch (SQLiteException SQLiteThrow)
            {
                MessageBox.Show("OutInventory Message: " + SQLiteThrow.Message);
            }
        }

        public static void OutInventoryDelete(int Barcode, int Number, string Location)
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
                            DELETE FROM stock WHERE 
                                stock_id IN (SELECT stock_id FROM stock WHERE 
                                product_id IN (SELECT product_id FROM products WHERE product_barcode = @Barcode)
                                AND
                                location_id IN (SELECT location_id FROM locations WHERE 
                                    location = @Location));

                            INSERT INTO history (product_id, history_status, location_id, stock_number) 
                            VALUES 
                                ((SELECT product_id FROM products WHERE product_barcode = @Barcode), 
                                '0', 
                                (SELECT location_id FROM locations WHERE 
                                    location = @Location),
                                @Number)
                        ";
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("Barcode", Barcode);
                        cmd.Parameters.AddWithValue("Location", Location);
                        cmd.Parameters.AddWithValue("Number", Number);
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
                MessageBox.Show("Delete Out Inventory.");
            }
            catch (SQLiteException SQLiteThrow)
            {
                MessageBox.Show("OutInventoryDelete Message: " + SQLiteThrow.Message);
            }
        }


    }
}
