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
    public partial class AddLocation : Form
    {
        public AddLocation()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                                "Having trouble?", "Support", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk
                ) == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start("mailto:ridvankuntug@gmail.com");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int[] comboLocation = new int[3];
            comboLocation[0] = Int32.Parse(comboBox1.SelectedItem.ToString());
            comboLocation[1] = Int32.Parse(comboBox1.SelectedItem.ToString());
            comboLocation[2] = Int32.Parse(comboBox1.SelectedItem.ToString());
            DatabaseClass.LocationTableAddItem(comboLocation);

            Application.Restart();
        }

        private void AddLocation_Load(object sender, EventArgs e)
        {
            comboBox1.Items.AddRange(Enumerable.Range(0, 100).Select(i => (object)i).ToArray());
            comboBox2.Items.AddRange(Enumerable.Range(0, 100).Select(i => (object)i).ToArray());
            comboBox3.Items.AddRange(Enumerable.Range(0, 100).Select(i => (object)i).ToArray());
        }
    }
}
