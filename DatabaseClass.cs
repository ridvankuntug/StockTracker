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
                            product_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 
                            product_barcode INTEGER NOT NULL UNIQUE, 
                            product_name VARCHAR(55) NOT NULL UNIQUE,
                            product_first_add_time TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP);

                            CREATE TABLE IF NOT EXISTS locations (
                            location_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 
                            location_a VARCHAR(3) NOT NULL,
                            location_b VARCHAR(3) NOT NULL,
                            location_c VARCHAR(3) NOT NULL,
                            location VARCHAR(3) NOT NULL UNIQUE);

                            CREATE TABLE IF NOT EXISTS stock (
                            stock_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 
                            product_id INTEGER NOT NULL REFERENCES products(product_id) ON UPDATE CASCADE,
                            location_id INTEGER NOT NULL REFERENCES locations(location_id) ON UPDATE CASCADE,
                            stock_number INTEGER NOT NULL);

                            CREATE TABLE IF NOT EXISTS history (
                            history_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, 
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
                            SELECT product_name, product_barcode, SUM(stock_number), GROUP_CONCAT(location || ': ' || stock_number, ', ') FROM stock 
                            JOIN products ON products.product_id = stock.product_id
                            JOIN locations ON locations.location_id = stock.location_id 
                            GROUP BY product_barcode
                            

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

        public static bool ProductsTableCheck()
        {
            try
            {
                ConnectDatabase();
                using (con)
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(con))
                    {
                        int count;
                        cmd.CommandText = @"SELECT count(*) FROM products";
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

        public static void LocationTableAddItem(char Location_A, int Location_B, int Location_C)
        {
            System.Threading.Thread threadadd = new System.Threading.Thread(new System.Threading.ThreadStart(add));
            Control.CheckForIllegalCrossThreadCalls = false;    // THREAD ÇAKIŞMASINI ENGELLER
            threadadd.Priority = System.Threading.ThreadPriority.Highest;
            threadadd.Start();

            void add()
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

                            char i = 'A';
                            int j = 1, k = 1;
                            int x = 0;

                            string values = "";

                            for (i = 'A'; i <= Location_A; i++)
                            {
                                for (j = 1; j <= Location_B; j++)
                                {
                                    for (k = 1; k <= Location_C; k++)
                                    {
                                        if (x == 0)
                                        {
                                            x++;
                                        }
                                        else
                                        {
                                            values = values + ", ";
                                        }
                                        values = values + "('" + i + "', '" + +j + "', '" + k + "', '" + i + j + k + "')";
                                    }
                                }
                            }

                            cmd.CommandText = @"INSERT OR IGNORE INTO locations (location_a, location_b, Location_c, location) 
                                            VALUES " + values;
                            cmd.Prepare();
                            cmd.ExecuteNonQuery();
                            transaction.Commit();
                        }
                    }
                }
                catch (SQLiteException SQLiteThrow)
                {
                    MessageBox.Show("LocationTableAddItem Message: " + SQLiteThrow.Message + "\n");
                }
                finally
                {
                    Application.Restart();
                }
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

        public static bool IsBarcodeExist(string Barcode, string ProductID)
        {
            if (ProductID != "")
            {
                ProductID = "AND product_id != '" + ProductID + "'";
            }

            try
            {
                ConnectDatabase();
                using (con)
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(con))
                    {
                        cmd.CommandText = @"SELECT * FROM products WHERE product_barcode = @Barcode " + ProductID; ;
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

        public static bool IsProductNameExist(string Name, string ProductID)
        {
            if (ProductID != "")
            {
                ProductID = "AND product_id != '" + ProductID + "'";
            }

            try
            {
                ConnectDatabase();
                using (con)
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(con))
                    {
                        cmd.CommandText = @"SELECT * FROM products WHERE product_name = @Name " + ProductID;
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("Name", Name);
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
                ManageProducts.labelChange("Added!", Barcode + " \n  " + Name);
            }
            catch (SQLiteException SQLiteThrow)
            {
                string stringCutted = SQLiteThrow.Message.Split(' ').Last();
                if (SQLiteThrow.ErrorCode == 19)
                {
                    if (Equals(stringCutted, "products.product_name"))
                    {
                        MessageBox.Show("This product name already exist!");
                    }
                    else if (Equals(stringCutted, "products.product_barcode"))
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

        public static void EditProduct(int ProductID, int Barcode, string Name)
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

                        cmd.CommandText = @"UPDATE products SET product_barcode = @Barcode, product_name = @Name
                                            WHERE product_id = @ProductID";
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("Barcode", Barcode);
                        cmd.Parameters.AddWithValue("Name", Name);
                        cmd.Parameters.AddWithValue("ProductID", ProductID);
                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
                MessageBox.Show("Saved.");
            }
            catch (SQLiteException SQLiteThrow)
            {
                string stringCutted = SQLiteThrow.Message.Split(' ').Last();
                if (SQLiteThrow.ErrorCode == 19)
                {
                    if (Equals(stringCutted, "products.product_name"))
                    {
                        MessageBox.Show("This product name already exist!");
                    }
                    else if (Equals(stringCutted, "products.product_barcode"))
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
                        InInventory.labelChange("Added!", Barcode + " \n  " + Number + " \n  " + Location[0] + Location[1] + Location[2]);
                    }
                }

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
                InInventory.labelChange("Added on!", Barcode + " \n  " + Number +" \n  " + Location[0] + Location[1] + Location[2]);

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

                            List<List<string>> locations = new List<List<string>> { location, number };
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
                OutInventory.labelChange("Out!", Barcode + " \n  " + Number + " \n  " + Location);
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
                OutInventory.labelChange("Removed!", Barcode + " \n  " + Number + " \n  " + Location);
            }
            catch (SQLiteException SQLiteThrow)
            {
                MessageBox.Show("OutInventoryDelete Message: " + SQLiteThrow.Message);
            }
        }

        public static void DeleteOlderThen(string Time)
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
                            DELETE FROM history WHERE 
                                history_adding_time < DATE('NOW', '-" + Time + @" YEAR');
                        ";
                        cmd.Prepare();
                        int effectedRowsCount = cmd.ExecuteNonQuery();
                        transaction.Commit();
                        if (effectedRowsCount > 0)
                            MessageBox.Show("Delete older the " + Time + " year history. \n"
                                + effectedRowsCount + " Rows effected.");
                    }
                }
            }
            catch (SQLiteException SQLiteThrow)
            {
                MessageBox.Show("DeleteOlderThen Message: " + SQLiteThrow.Message);
            }
        }


        public static string[] MinLocation()
        {
            string[] location = new string[3];
            try
            {
                ConnectDatabase();
                using (con)
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(con))
                    {
                        cmd.CommandText = @"
                            SELECT MAX(location_a), MAX(location_b), MAX(location_c) FROM locations
                        ";
                        cmd.Prepare();
                        using (SQLiteDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                location[0] = rdr.GetString(0);
                                location[1] = rdr.GetString(1);
                                location[2] = rdr.GetString(2);
                            }
                            else
                            {
                                location[0] = "A";
                                location[1] = "1";
                                location[2] = "1";
                            }
                            con.Close();
                            return location;
                        }
                    }
                }
            }
            catch (SQLiteException SQLiteThrow)
            {
                MessageBox.Show("IsStockZero Message: " + SQLiteThrow.Message + "\n");
                return null;
            }
        }

        public static bool DropHistoryStock()
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
                            DELETE FROM history;
                            DELETE FROM SQLITE_SEQUENCE WHERE NAME=history;
                            
                            DELETE FROM stock;
                            DELETE FROM SQLITE_SEQUENCE WHERE NAME=stock;

                            ";

                        cmd.ExecuteNonQuery();
                        myTrans.Commit();

                        return true;
                    }
                    catch (SQLiteException SQLiteThrow)
                    {
                        myTrans.Rollback();

                        return false;
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
        }

        public static bool DropHistoryStockLocation()
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
                            DELETE FROM history;
                            DELETE FROM SQLITE_SEQUENCE WHERE NAME=history;
                            
                            DELETE FROM stock;
                            DELETE FROM SQLITE_SEQUENCE WHERE NAME=stock;
                            
                            DELETE FROM locations;
                            DELETE FROM SQLITE_SEQUENCE WHERE NAME=locations;

                            ";

                        cmd.ExecuteNonQuery();
                        myTrans.Commit();

                        return true;
                    }
                    catch (SQLiteException SQLiteThrow)
                    {
                        myTrans.Rollback();

                        return false;
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
        }


    }
}
