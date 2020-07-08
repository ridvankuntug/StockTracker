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
            string[] LicenseKey = new string[2];
            LicenseKey = Regedit.Read();
            
            if(LicenseKey == null)
            {
                menuStrip1.Enabled = false;
                Form License = new License();
                License.MdiParent = this;
                License.Show();
            }
            else if (!MethodsClass.LicenseCheck(MethodsClass.CreateKey(LicenseKey[0]), LicenseKey[1]))
            {
                menuStrip1.Enabled = false;
                Form License = new License();
                License.MdiParent = this;
                License.Show();
            }
            else if (!DatabaseClass.LocationTableCheck())
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

        private void homeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Form> formsList = new List<Form>();

            for (int i = formsList.Count - 1; i > 0; i--)
            {
                if (formsList[i].Name != "Menu")
                {
                    formsList[i].Close();
                }
            }

            Form HomeForm = new Home();
            HomeForm.MdiParent = this;
            HomeForm.Show();
        }
    }
}
