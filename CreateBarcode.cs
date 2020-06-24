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

        private void button1_Click(object sender, EventArgs e)
        {
            if (DatabaseClass.IsBarcodeExist(textBox1.Text))
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

        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox1.Image.Save(@"Barcodes/"+ textBox1.Text + ".png", ImageFormat.Png);
        }

        void BarcodeCreater()//Kodu iki kere yazmamız gerekeceği için barkod oluşturan kodu yazdık
        {
            BarcodeWriter writer = new BarcodeWriter() { Format = BarcodeFormat.CODE_128 };
            pictureBox1.Image = writer.Write(textBox1.Text);

        }
    }
}
