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
            DataSet ds = DatabaseClass.GridFill("history_view");
            dataGridView1.DataSource = ds.Tables["*"];
            dataGridView1.Columns[0].HeaderText = "Name";
            dataGridView1.Columns[1].HeaderText = "Barcode";
            dataGridView1.Columns[3].HeaderText = "Number";
            dataGridView1.Columns[4].HeaderText = "Date";
            dataGridView1.Columns[2].Visible = false;
            dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


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
                    Myrow.DefaultCellStyle.BackColor = Color.Green;
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
    }
}
