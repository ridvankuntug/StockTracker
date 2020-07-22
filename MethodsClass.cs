using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;

/*using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;*/

using ExcelLibrary.CompoundDocumentFormat;
using ExcelLibrary.SpreadSheet;//TODO

namespace StockTracker
{
    class MethodsClass
    {
        public static string CreateKey(string eMail)
        {
            string newInput = eMail + "raxacoricofallapatorius";
            string sha512 = CryptoClass.EncodeSHA512(newInput);
            string hash = CryptoClass.EncodeMd5(sha512);
            return hash;
        }

        public static bool LicenseCheck(string Key, string License)
        {
            if(Key == License)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ExportDatabase()
        {
            try
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                saveFileDialog1.InitialDirectory = path;
                saveFileDialog1.Filter = "Database Files (*.db)|*.db|All files (*.*)|*.*";
                saveFileDialog1.Title = "Select the location to back up the database file.";
                saveFileDialog1.DefaultExt = "db";
                saveFileDialog1.CheckPathExists = true;
                saveFileDialog1.FileName = "Stock" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".db";

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (saveFileDialog1.FileName != "")
                    {
                        File.Copy(@"Databases/Stock.db", saveFileDialog1.FileName, true);
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

                saveFileDialog1.Reset();
                return true;
            }
            catch (IOException IOThrow)
            {
                return false;
            }
        }

        public static bool ImportDatabase()
        {
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                openFileDialog1.InitialDirectory = path;
                openFileDialog1.Filter = "Database Files (*.db)|*.db";
                openFileDialog1.Title = "Select the location to import the database file.";
                openFileDialog1.DefaultExt = "db";
                openFileDialog1.CheckFileExists = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (openFileDialog1.FileName != "")
                    {
                        File.Copy(openFileDialog1.FileName, @"Databases/Stock.db", true);
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

                openFileDialog1.Reset();
                return true;
            }
            catch (IOException IOThrow)
            {
                MessageBox.Show(IOThrow.ToString());
                return false;
            }
        }

        public static bool ExportToExcel(DataGridView dataGridView)
        {           
            try
            {
                Workbook workbook = new Workbook();
                Worksheet worksheet = new Worksheet("First Sheet");
                worksheet.Cells.ColumnWidth[0, 1] = 3000;

                List<DataGridViewColumn> listVisible = new List<DataGridViewColumn>();
                foreach (DataGridViewColumn col in dataGridView.Columns)
                {
                    if (col.Visible)
                        listVisible.Add(col);
                }

                for (int i = 0; i < listVisible.Count; i++)
                {
                    worksheet.Cells[0, i] = new Cell(listVisible[i].HeaderText);
                }

                int rowCount = 1;
                for (int i = 0; i < dataGridView.Rows.Count; i++)
                {
                    for (int j = 0; j < listVisible.Count; j++)
                    {
                        worksheet.Cells[i + 1, j] = new Cell(dataGridView.Rows[i].Cells[listVisible[j].Name].Value.ToString());
                        rowCount++;
                    }
                }

                while(rowCount++ < 150)
                {
                    worksheet.Cells[rowCount, 0] = new Cell("");
                }

                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                saveFileDialog1.InitialDirectory = path;
                saveFileDialog1.Filter = "Excel Files (*.xls)|*.xls|All files (*.*)|*.*";
                saveFileDialog1.Title = "Select the location to export the Excel file.";
                saveFileDialog1.DefaultExt = "xls";
                saveFileDialog1.CheckPathExists = true;
                saveFileDialog1.FileName = "Report" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (saveFileDialog1.FileName != "")
                    {
                        workbook.Worksheets.Add(worksheet);
                        workbook.Save(saveFileDialog1.FileName);

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
            }
            catch (IOException IOThrow)
            {
                MessageBox.Show(IOThrow.ToString());
                return false;
            }
        }

    }

    class Regedit
    {
        public static string[] Read()
        {
            string[] LicanseInfo = new string[2];
            //opening the subkey  
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\RK\StockTracker");

            //if it does exist, retrieve the stored values  
            if (key != null)
            {
                LicanseInfo[0] = key.GetValue("EMail").ToString();
                LicanseInfo[1] = key.GetValue("LicenseKey").ToString();
                key.Close();
                return LicanseInfo;
            }
            else
            {
                return null;
            }
        }

        public static void Write(string EMail, string LicenseKey)
        {
            //accessing the CurrentUser root element  
            //and adding "OurSettings" subkey to the "SOFTWARE" subkey  
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\RK\StockTracker");

            //storing the values  
            key.SetValue("EMail", EMail);
            key.SetValue("LicenseKey", LicenseKey);
            key.Close();
        }
    }

    class CryptoClass
    {
        public static string EncodeMd5(string rawInput)
        {
            // Use input string to calculate MD5 hash
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(rawInput);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public static string EncodeSHA512(string rawInput)
        {
            // Use input string to calculate MD5 hash
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(rawInput);
                byte[] hashBytes = sha512.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }

}
