using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ZXing;
using System.Drawing.Imaging;
using System.IO;

namespace StockTracker
{
    public partial class CreateBarcode : Form
    {
        public CreateBarcode()
        {
            InitializeComponent();
        }

        private void CreateBarcode_Load(object sender, EventArgs e)
        {
            try
            {
                    if (!Directory.Exists("Barcodes"))
                    {
                        Directory.CreateDirectory("Barcodes");
                        Console.WriteLine("Director created.");
                    }
                    else
                    {
                        Console.WriteLine("Director already exist.");
                    }

            }
            catch (IOException IOThrow)
            {
                MessageBox.Show("Message: " + IOThrow.Message + "\n");
            }
        }

        public void createBarcode()
        {
            if (textBox1.Text.Length< 9)
            {
                MessageBox.Show("Barcode must be at 9 characters long.");
            }
            else
            {

                if (DatabaseClass.IsBarcodeExist(textBox1.Text, ""))
                {
                    if (MessageBox.Show("This barcode already exist in inventory list. Clear field for new barcode?", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk
                    ) == DialogResult.Yes)
                    {
                        textBox1.Text = "";
                    }
                    else
                    {
                        BarcodeCreater();
                    }
                }
                else
                {
                    BarcodeCreater();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            createBarcode();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Save(@"Barcodes/" + textBox1.Text + ".png", ImageFormat.Png);
                MessageBox.Show("Saved.");
            }
        }

        void BarcodeCreater()//Kodu iki kere yazmamız gerekeceği için barkod oluşturan kodu yazdık
        {
            BarcodeWriter writer = new BarcodeWriter() { Format = BarcodeFormat.CODE_128 };
            pictureBox1.Image = writer.Write(textBox1.Text);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form ManageProducts = new ManageProducts();
            ManageProducts.MdiParent = ParentForm;
            ManageProducts.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            string rand = rnd.Next(100000000, 999999999).ToString();

            
            do {
                textBox1.Text = rand;
                rand = rnd.Next(100000000, 999999999).ToString();
            } while(DatabaseClass.IsBarcodeExist(rand, ""));

            createBarcode();

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
    }
}
