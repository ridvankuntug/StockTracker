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
using System.Threading;

namespace StockTracker
{
    public partial class ManageProducts : Form
    {
        public ManageProducts()
        {
            InitializeComponent();
        }

        private void ManageProducts_Load(object sender, EventArgs e)
        {
            //DataGridFill();

            label8.Visible = false;
            label7.Visible = false;
        }

        private void DataGridFill()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.DataSource = null;

            DataSet ds = DatabaseClass.GridFill("*", "products", textBox3.Text, textBox4.Text, "", "");
            dataGridView1.DataSource = ds.Tables["*"];

            dataGridView1.Columns[1].HeaderText = "Barcodes";
            dataGridView1.Columns[2].HeaderText = "Name";
            dataGridView1.Columns[3].HeaderText = "Add Date";
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns["product_name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            DataGridViewButtonColumn btn = new DataGridViewButtonColumn();
            dataGridView1.Columns.Add(btn);
            btn.Name = "dataGridButton";
            btn.HeaderText = "";
            btn.Text = "Edit";
            btn.UseColumnTextForButtonValue = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form CreateBarcode = new CreateBarcode();
            CreateBarcode.MdiParent = ParentForm;
            CreateBarcode.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Enter();
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Enter();
            }
        }

        public void Enter()
        {
            if (textBox1.Text.Length < 9)
            {
                MessageBox.Show("Barcode must be at 9 characters long.");
            }
            else if (textBox2.Text == "")
            {
                MessageBox.Show("Product name must not be empty.");
            }
            else if (DatabaseClass.IsBarcodeExist(textBox1.Text, ""))
            {
                MessageBox.Show("Barcode already exist.");
            }
            else if (DatabaseClass.IsProductNameExist(textBox2.Text, ""))
            {
                MessageBox.Show("Product name already exist.");
            }
            else
            {
                DatabaseClass.AddNewProduct(Int32.Parse(textBox1.Text), textBox2.Text);
                DataGridFill();
            }
        }

        public static void labelChange(string status, string label)
        {
            System.Threading.Thread threadlabel = new System.Threading.Thread(new System.Threading.ThreadStart(change));
            Control.CheckForIllegalCrossThreadCalls = false;    // THREAD ÇAKIŞMASINI ENGELLER
            threadlabel.Priority = System.Threading.ThreadPriority.Highest;
            threadlabel.Start();

            void change()
            {
                label7.Text = status;
                label7.ForeColor = Color.Green;
                label7.Visible = true;

                label8.Text = label;
                label8.ForeColor = Color.Green;
                label8.Visible = true;

                Thread.Sleep(10000);

                label7.Visible = false;
                label8.Visible = false;

                label7.Text = "";
                label8.Text = "";

            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(textBox1.Text, "  ^ [0-9]"))
            {
                textBox1.Text = "";
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void ManageProducts_Activated(object sender, EventArgs e)
        {
            DataGridFill();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            DataGridFill();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            DataGridFill();
        }

        private void button4_Click(object sender, EventArgs e)
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

        public static string[] CellID = new string[3];

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4)
            {
                CellID[0] = dataGridView1.Rows[e.RowIndex].Cells["product_id"].Value.ToString();
                CellID[1] = dataGridView1.Rows[e.RowIndex].Cells["product_barcode"].Value.ToString();
                CellID[2] = dataGridView1.Rows[e.RowIndex].Cells["product_name"].Value.ToString();

                Form EditProduct = new EditProduct();
                EditProduct.ShowDialog(this);

                while (EditProduct.DialogResult != DialogResult.Cancel)
                {
                    if (EditProduct.DialogResult == DialogResult.OK)
                    {
                        DataGridFill();
                        break;
                    }
                    else
                    {
                        EditProduct.ShowDialog(this);
                    }
                }

            }
        }
    }
}
