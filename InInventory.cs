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
                Enter();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form ManageProducts = new ManageProducts();
            ManageProducts.MdiParent = ParentForm;
            ManageProducts.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Enter();
        }

        private void Enter()
        {
            Barcode = Int32.Parse(textBox1.Text);
            Form InInvLocation = new InInvOptions();
            InInvLocation.ShowDialog();
        }
    }
}
