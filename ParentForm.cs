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
    public partial class ParentForm : Form
    {
        public ParentForm()
        {
            InitializeComponent();
            //this.FormBorderStyle = FormBorderStyle.FixedSingle;

            DatabaseClass.CreateDB();
            DatabaseClass.CreateTable();


            if (!DatabaseClass.LocationTableCheck())
            {
                Form AddLocation = new AddLocation();
                AddLocation.ShowDialog();
            }
            else
            {
                Form HomeForm = new Home();
                HomeForm.MdiParent = this;
                HomeForm.Show();
            }
        }

        private void newInventoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form NewProduct = new ManageProducts();
            NewProduct.MdiParent = this;
            NewProduct.Show();
        }
    }
}
