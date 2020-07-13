using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;

namespace StockTracker
{
    public partial class StockStatus : Form
    {
        public StockStatus()
        {
            InitializeComponent();
        }

        private void StockStatus_Load(object sender, EventArgs e)
        {
            //DataGridFill();
        }

        private void DataGridFill()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.DataSource = null;

            DataSet ds = DatabaseClass.GridFill("*", "stock_view", textBox1.Text, textBox2.Text, "", "");
            dataGridView1.DataSource = ds.Tables["*"];
            dataGridView1.Columns[0].HeaderText = "Name";
            dataGridView1.Columns[1].HeaderText = "Barcode";
            dataGridView1.Columns[2].HeaderText = "Number";
            dataGridView1.Columns[3].HeaderText = "Location";
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView1.Sort(this.dataGridView1.Columns[0], ListSortDirection.Ascending);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            DataGridFill();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            DataGridFill();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Excel.Application oExcel_15 = null;
            Excel.Workbook oBook = null;
            Excel.Sheets oSheetsColl = null;
            Excel.Worksheet oSheet = null;
            Excel.Range oRange = null;
            Object oMissing = System.Reflection.Missing.Value;

            oExcel_15 = new Excel.Application();

            oExcel_15.Visible = true;

            oExcel_15.UserControl = true;


            oBook = oExcel_15.Workbooks.Add(oMissing);

            oSheetsColl = oExcel_15.Worksheets;

            oSheet = (Excel.Worksheet)oSheetsColl.get_Item(1);


            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                oSheet.Cells[1, i + 1] = dataGridView1.Columns[i].HeaderText;
            }

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    oSheet.Cells[i + 2, j + 1] = dataGridView1.Rows[i].Cells[dataGridView1.Columns[j].Name].Value.ToString();
                }
            }
        }

        private void StockStatus_Activated(object sender, EventArgs e)
        {
            DataGridFill();
        }
    }
}
