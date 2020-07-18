using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StockTracker
{
    public partial class Home : Form
    {
        public Home()
        {
            InitializeComponent();
        }

        private void Home_Load(object sender, EventArgs e)
        {
            //DataGridFill();
        }

        private void DataGridFill()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.DataSource = null;

            string date = "AND history_adding_time  BETWEEN '" + DateTime.Today.AddDays(-2).ToString("yyyy-MM-dd") + " 00:00:00' and '" + DateTime.Today.ToString("yyyy-MM-dd") + " 23:59:59'";
            DataSet ds = DatabaseClass.GridFill("*", "history_view", "", "", date, "");

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
        }

        private void Home_Activated(object sender, EventArgs e)
        {
            DataGridFill();
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            foreach (DataGridViewRow Myrow in dataGridView1.Rows)
            {            //Here 2 cell is target value and 1 cell is Volume
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

        private void button1_Click(object sender, EventArgs e)
        {
            Form InInventoryForm = new InInventory();
            InInventoryForm.MdiParent = ParentForm;
            InInventoryForm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form OutInventory = new OutInventory();
            OutInventory.MdiParent = ParentForm;
            OutInventory.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form CreateBarcode = new CreateBarcode();
            CreateBarcode.MdiParent = ParentForm;
            CreateBarcode.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form StockStatus = new StockStatus();
            StockStatus.MdiParent = ParentForm;
            StockStatus.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form Reports = new Reports();
            Reports.MdiParent = ParentForm;
            Reports.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form ManageProducts = new ManageProducts();
            ManageProducts.MdiParent = ParentForm;
            ManageProducts.Show();
        }
    }
}
