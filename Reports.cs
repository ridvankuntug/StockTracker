using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


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
            comboBox1.Items.Insert(0, "Both");
            comboBox1.Items.Insert(1, "In");
            comboBox1.Items.Insert(2, "Out");
            comboBox1.SelectedIndex = 0;

            dateTimePicker1.Value = DateTime.Today.AddDays(-2);
            dateTimePicker2.Value = DateTime.Today;
            //DataGridFill();
        }

        private void Reports_Activated(object sender, EventArgs e)
        {
            DataGridFill();
        }

        private void DataGridFill()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.DataSource = null;

            DataSet ds;
            string table;
            string date;
            string history_status;

            if (checkBox1.Checked)
            {
                date = "AND history_adding_time  BETWEEN '0000-00-00 00:00:00' and '" + DateTime.Today.ToString("yyyy-MM-dd") + " 23:59:59'";
            }
            else
            {
                date = "AND history_adding_time  BETWEEN '" + dateTimePicker1.Value.ToString("yyyy-MM-dd") + " 00:00:00' and '" + dateTimePicker2.Value.ToString("yyyy-MM-dd") + " 23:59:59'";
            }

            if (comboBox1.SelectedIndex == 1)
            {
                table = "in_history_view";
            }
            else if(comboBox1.SelectedIndex == 2)
            {
                table = "out_history_view";

            }
            else
            {
                table = "history_view";

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
                    dr[3] = "-" + dr[3]; //change the name
                                                //break; break or not depending on you
                }
            }

            dataGridView1.DataSource = dt;
            dataGridView1.Columns[0].HeaderText = "Name";
            dataGridView1.Columns[1].HeaderText = "Barcode";
            dataGridView1.Columns[2].Visible = false;
            dataGridView1.Columns[3].HeaderText = "Number";
            dataGridView1.Columns[4].HeaderText = "Location";
            dataGridView1.Columns[5].HeaderText = "Add Date";
            dataGridView1.Columns["product_name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView1.Sort(this.dataGridView1.Columns[5], ListSortDirection.Descending);

            dataGridView1.Refresh();
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (checkBox3.Checked)
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
            if (MethodsClass.ExportToExcel(dataGridView1))
            {
                MessageBox.Show("Saved");
            }
            else
            {
                MessageBox.Show("Can't saved!");
            }
        }

        
    }
}
