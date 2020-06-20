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
    public partial class Home : Form
    {
        public Home()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Form InInventoryForm = new InInventory();
            InInventoryForm.MdiParent = ParentForm;
            InInventoryForm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form OutInventoryForm = new OutInventory();
            OutInventoryForm.MdiParent = ParentForm;
            OutInventoryForm.Show();
        }
    }
}
