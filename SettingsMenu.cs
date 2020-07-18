using StockTracker.Properties;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace StockTracker
{
    public partial class SettingsMenu : Form
    {
        public SettingsMenu()
        {
            InitializeComponent();
        }

        public string[] location;

        private void SettingsMenu_Load(object sender, EventArgs e)
        {
            location = DatabaseClass.MinLocation();
            string location0 = location[0];
            for (char i = Convert.ToChar(location[0]); i <= 'Z'; i++)
            {
                location0 = location0 + i;
            }

            comboBox1.Items.AddRange(location0.Distinct().ToArray().ToList().Select(i => (object)i).ToArray());
            comboBox2.Items.AddRange(Enumerable.Range(Int32.Parse(location[1]), 10 - Int32.Parse(location[1])).Select(i => (object)i).ToArray());
            comboBox3.Items.AddRange(Enumerable.Range(Int32.Parse(location[2]), 100 - Int32.Parse(location[2])).Select(i => (object)i).ToArray());

            numericUpDown1.Value = Settings.Default.DeletePeriod;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Settings.Default.DeletePeriod = Convert.ToInt32(numericUpDown1.Value);
            Settings.Default.Save();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Settings.Default.DeletePeriod = Convert.ToInt32(numericUpDown1.Value);

            if (comboBox1.SelectedItem != null && comboBox2.SelectedItem != null & comboBox3.SelectedItem != null)
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
