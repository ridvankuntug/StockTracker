using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using StockTracker.Properties;

namespace StockTracker
{
    public partial class DatabaseManagement : Form
    {
        public DatabaseManagement()
        {
            InitializeComponent();
            label1.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            if (MethodsClass.ExportDatabase())
            {
                label1.ForeColor = Color.Green;
                label1.Text = "Backed up to the desktop.";
                label1.Visible = true;
            }
            else
            {
                label1.ForeColor = Color.Red;
                label1.Text = "Not backed up.";
                label1.Visible = true;                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                       "Are you sure delete this datas?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk
                   ) == DialogResult.Yes)
            {
                if (MethodsClass.ExportDatabase())
                {
                    if (DatabaseClass.DropHistoryStock())
                    {
                        label2.ForeColor = Color.Green;
                        label2.Text = "Backed up and delete.";
                        label2.Visible = true;
                    }
                    else
                    {

                        label2.ForeColor = Color.Red;
                        label2.Text = "Cant delete datas.";
                        label2.Visible = true;
                    }
                }
                else
                {
                    label2.ForeColor = Color.Red;
                    label2.Text = "Not backed up.";
                    label2.Visible = true;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                       "Are you sure delete this datas?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk
                   ) == DialogResult.Yes)
            {
                if (MethodsClass.ExportDatabase())
                {
                    if (DatabaseClass.DropHistoryStockLocation())
                    {
                        label3.ForeColor = Color.Green;
                        label3.Text = "Backed up and delete.";
                        label3.Visible = true;
                    }
                    else
                    {
                        label3.ForeColor = Color.Red;
                        label3.Text = "Cant delete datas.";
                        label3.Visible = true;
                    }
                }
                else
                {
                    label3.ForeColor = Color.Red;
                    label3.Text = "Not backed up.";
                    label3.Visible = true;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                          "Are you sure you want to delete all the data? \nThe program will be restarted.", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk
                      ) == DialogResult.Yes)
            {
                if (MethodsClass.ExportDatabase())
                {
                    if (File.Exists(@"Databases/Stock.db"))
                    {
                        File.Delete(@"Databases/Stock.db");
                        Settings.Default.DeletePeriod = 2;
                        Settings.Default.Save();
                        Application.Restart();
                    }
                    else
                    {
                        label4.ForeColor = Color.Red;
                        label4.Text = "Database not deleted.";
                        label4.Visible = true;
                    }
                }
                else
                {
                    label4.ForeColor = Color.Red;
                    label4.Text = "Database not deleted.";
                    label4.Visible = true;
                }
            }
            else
            {
                label4.ForeColor = Color.Red;
                label4.Text = "Database not deleted.";
                label4.Visible = true;
            }
        }
    }
}
