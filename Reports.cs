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
    public partial class Reports : Form
    {
        public Reports()
        {
            InitializeComponent();
        }

        private void Reports_Load(object sender, EventArgs e)
        {
            dateTimePicker1.Value = DateTime.Today.AddDays(-2);
            dateTimePicker2.Value = DateTime.Today;
            DataGridFill();
        }

        private void DataGridFill()
        {
            DataSet ds;
            string date;
            string table = "history_view";

            if (checkBox1.Checked)
            {
                date = "AND history_adding_time  BETWEEN '0000-00-00 00:00:00' and '" + DateTime.Today.ToString("yyyy-MM-dd") + " 23:59:59'";
            }
            else
            {
                date = "AND history_adding_time  BETWEEN '" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + " 00:00:00' and '" + dateTimePicker2.Value.ToString("yyyy-MM-dd") + " 23:59:59'";
            }

            if (checkBox2.Checked)
            {
                string columns = "product_name, product_barcode, history_status, SUM(stock_number), location, history_adding_time";
                string groupBy = "GROUP BY  product_barcode, history_status";
                ds = DatabaseClass.GridFill(columns, table, textBox1.Text, textBox2.Text, date, groupBy);
            }
            else
            {
                ds = DatabaseClass.GridFill("*", table, textBox1.Text, textBox2.Text, date, "");
            }

            System.Data.DataTable dt = ds.Tables["*"];
            foreach (DataRow dr in dt.Rows) // search whole table
            {
                if (!Convert.ToBoolean(dr["history_status"])) // if id==2
                {
                    dr["stock_number"] = "-" + dr["stock_number"]; //change the name
                                                //break; break or not depending on you
                }
            }

            dataGridView1.DataSource = dt;
            dataGridView1.Columns[0].HeaderText = "Name";
            dataGridView1.Columns[1].HeaderText = "Barcode";
            dataGridView1.Columns[3].HeaderText = "Number";
            dataGridView1.Columns[4].HeaderText = "Location";
            dataGridView1.Columns[5].HeaderText = "Add Date";
            dataGridView1.Columns[2].Visible = false;
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dataGridView1.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView1.Sort(this.dataGridView1.Columns[5], ListSortDirection.Descending);

            dataGridView1.Refresh();
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            foreach (DataGridViewRow Myrow in dataGridView1.Rows)
            {     //Here 2 cell is target value and 1 cell is Volume
                if (Myrow.Cells[2].Value == DBNull.Value)
                {
                    Myrow.DefaultCellStyle.BackColor = Color.White;
                }
                else if (Convert.ToBoolean(Myrow.Cells[2].Value))// Or your condition 
                {
                    Myrow.DefaultCellStyle.BackColor = Color.Lime;
                }
                else
                {
                    Myrow.DefaultCellStyle.BackColor = Color.Red;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                dateTimePicker1.Enabled = false;
                dateTimePicker2.Enabled = false;
            }
            else
            {
                dateTimePicker1.Enabled = true;
                dateTimePicker2.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataGridFill();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            DataGridFill();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            DataGridFill();
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



            List<DataGridViewColumn> listVisible = new List<DataGridViewColumn>();
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                if (col.Visible)
                    listVisible.Add(col);
            }

            for (int i = 0; i < listVisible.Count; i++)
            {
                oSheet.Cells[1, i + 1] = listVisible[i].HeaderText;
            }

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                for (int j = 0; j < listVisible.Count; j++)
                {
                    oSheet.Cells[i + 2, j + 1] = dataGridView1.Rows[i].Cells[listVisible[j].Name].Value.ToString();
                    oSheet.Cells[i + 2, j + 1].Interior.Color = System.Drawing.ColorTranslator.ToOle(dataGridView1.Rows[i].DefaultCellStyle.BackColor);

                }
            }
        }




    }
}
