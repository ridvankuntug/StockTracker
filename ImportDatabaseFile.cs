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
    public partial class ImportDatabaseFile : Form
    {
        public ImportDatabaseFile()
        {
            InitializeComponent();
            label3.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MethodsClass.ImportDatabase())
            {
                label3.ForeColor = Color.Green;
                label3.Text = "Imported.";
                label3.Visible = true;
                Application.Restart();
            }
            else
            {
                label3.ForeColor = Color.Red;
                label3.Text = "Not imported.";
                label3.Visible = true;
            }
        }
    }
}
