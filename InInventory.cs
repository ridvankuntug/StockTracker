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
    public partial class InInventory : Form
    {
        public InInventory()
        {
            InitializeComponent();
        }

        public static int Barcode;

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Barcode = Int32.Parse(textBox1.Text);
                Form InInvLocation = new InInvOptions();
                InInvLocation.ShowDialog();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InInventory_Load(object sender, EventArgs e)
        {
            Form NewProduct = new NewProduct();
            NewProduct.MdiParent = ParentForm;
            NewProduct.Show();
        }
    }
}
