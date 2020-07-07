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
    public partial class ManageProducts : Form
    {
        public ManageProducts()
        {
            InitializeComponent();
        }

        private void ManageProducts_Load(object sender, EventArgs e)
        {
            DataGridFill();
        }

        private void DataGridFill()
        {
            DataSet ds = DatabaseClass.GridFill("*", "products", textBox3.Text, textBox4.Text, "", "");
            dataGridView1.DataSource = ds.Tables["*"];
            dataGridView1.Columns[1].HeaderText = "Barcodes";
            dataGridView1.Columns[2].HeaderText = "Name";
            dataGridView1.Columns[3].HeaderText = "Add Date";
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
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
            if (textBox1.Text.Length < 9)
            {
                MessageBox.Show("Barcode must be at 9 characters long.");
            }
            else if (DatabaseClass.IsBarcodeExist(textBox1.Text))
            {
                MessageBox.Show("Barcode already exist.");
            }
            else
            {
                DatabaseClass.AddNewProduct(Int32.Parse(textBox1.Text), textBox2.Text);
                dataGridView1.DataSource = null;
                DataGridFill();

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
            dataGridView1.DataSource = null;
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
