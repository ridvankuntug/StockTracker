using StockTracker.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StockTracker
{
    public partial class AddLocation : Form
    {
        public AddLocation()
        {
            InitializeComponent();
        }

        private void AddLocation_Load(object sender, EventArgs e)
        {
            comboBox1.Items.AddRange("ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToList().Select(i => (object)i).ToArray());
            comboBox2.Items.AddRange(Enumerable.Range(1, 9).Select(i => (object)i).ToArray());
            comboBox3.Items.AddRange(Enumerable.Range(1, 99).Select(i => (object)i).ToArray());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                    "Having trouble?", "Support", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk
                ) == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start("mailto:ridvankuntug@gmail.com?subject=I Have a Problem With Stock Tracker");

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Settings.Default.DeletePeriod = Convert.ToInt32(numericUpDown1.Value);
            Settings.Default.Save();

            if(comboBox1.SelectedItem != null && comboBox2.SelectedItem != null & comboBox3.SelectedItem != null)
            {
                char comboLocation1 = Char.Parse(comboBox1.SelectedItem.ToString());
                int comboLocation2 = Int32.Parse(comboBox2.SelectedItem.ToString());
                int comboLocation3 = Int32.Parse(comboBox3.SelectedItem.ToString());

                progressBar1.Style = ProgressBarStyle.Marquee;
                button1.Enabled = false;
                DatabaseClass.LocationTableAddItem(comboLocation1, comboLocation2, comboLocation3);
            }
            else
            {
                MessageBox.Show("You can't leave empty slot.");
            }
        }
    }
}
