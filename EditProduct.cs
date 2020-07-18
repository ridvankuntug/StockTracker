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
    public partial class EditProduct : Form
    {
        public EditProduct()
        {
            InitializeComponent();
        }

        private void EditProduct_Load(object sender, EventArgs e)
        {
            textBox1.Text = label4.Text = ManageProducts.CellID[1];
            textBox2.Text = label5.Text = ManageProducts.CellID[2];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Save();
        }

        public void Save()
        {
            if (textBox1.Text.Length < 9)
            {
                MessageBox.Show("Barcode must be at 9 characters long.");
                this.DialogResult = DialogResult.No;
                this.Close();
            }
            else if (textBox2.Text == "")
            {
                MessageBox.Show("Product name must not be empty.");
                this.DialogResult = DialogResult.No;
                this.Close();
            }
            else if (DatabaseClass.IsBarcodeExist(textBox1.Text, ManageProducts.CellID[0]))
            {
                MessageBox.Show("Barcode already exist another product.");
                this.DialogResult = DialogResult.No;
                this.Close();
            }
            else if (DatabaseClass.IsProductNameExist(textBox2.Text, ManageProducts.CellID[0]))
            {
                MessageBox.Show("Product name already exist another product.");
                this.DialogResult = DialogResult.No;
                this.Close();
            }
            else
            {
                DatabaseClass.EditProduct(Int32.Parse(ManageProducts.CellID[0]), Int32.Parse(textBox1.Text), textBox2.Text);
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

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            Save();
        }
    }
}
