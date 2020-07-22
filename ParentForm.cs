using StockTracker.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;


namespace StockTracker
{
    public partial class ParentForm : Form
    {
        public ParentForm()
        {
            InitializeComponent();            
        }

        private void ParentForm_Load(object sender, EventArgs e)
        {
            this.Text = this.Text + " v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();



            DatabaseClass.CreateDB();
            DatabaseClass.CreateTable();
            string[] LicenseKey = new string[2];
            LicenseKey = Regedit.Read();

            DatabaseClass.DeleteOlderThen(Settings.Default.DeletePeriod.ToString());

            if (LicenseKey == null)
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
                menuStrip1.Enabled = false;
                if (!DatabaseClass.ProductsTableCheck())
                {
                    if (MessageBox.Show(
                        "Do you have a database created and backed up with this program that you want to add to the system? \nIf you choose a wrong file, you may need to uninstall and reinstall the program.", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk
                        ) == DialogResult.Yes)
                    {
                        Form ImportDatabaseFile = new ImportDatabaseFile();
                        ImportDatabaseFile.ShowDialog();
                    }
                    else
                    {
                        Form AddLocation = new AddLocation();
                        AddLocation.ShowDialog();
                    }
                }
                else
                {
                    Form AddLocation = new AddLocation();
                    AddLocation.ShowDialog();
                }
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

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form SettingsMenu = new SettingsMenu();
            SettingsMenu.ShowDialog(this);
        }

        private void backUpDatabaseToDesktopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void databaseManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                    "Are you sure you want to enter this menu? \nIn this menu, you can perform irreversible actions or delete all data.?", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk
                ) == DialogResult.Yes)
            {
                Form DatabaseManagement = new DatabaseManagement();
                DatabaseManagement.MdiParent = this;
                DatabaseManagement.Show();
            }
        }
    }
}
