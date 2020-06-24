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

        private void AddLocation_Load(object sender, EventArgs e)
        {
            comboBox1.Items.AddRange("ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToList().Select(i => (object)i).ToArray());
            comboBox2.Items.AddRange(Enumerable.Range(1, 100).Select(i => (object)i).ToArray());
            comboBox3.Items.AddRange(Enumerable.Range(1, 100).Select(i => (object)i).ToArray());
            comboBox1.SelectedItem = null;
            comboBox1.SelectedText = "--select--";
            comboBox2.SelectedItem = null;
            comboBox2.SelectedText = "--select--";
            comboBox3.SelectedItem = null;
            comboBox3.SelectedText = "--select--";
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
            if(comboBox1.SelectedItem != null && comboBox2.SelectedItem != null & comboBox3.SelectedItem != null)
            {
                char comboLocation1 = Char.Parse(comboBox1.SelectedItem.ToString());
                int comboLocation2 = Int32.Parse(comboBox2.SelectedItem.ToString());
                int comboLocation3 = Int32.Parse(comboBox3.SelectedItem.ToString());

                progressBar1.Maximum = (comboLocation1 - 64) * comboLocation2 * comboLocation3;//Max değeri belirlemek için döngü sayısı hesaplanıyor

                char i = 'A';
                int j = 1, k = 1;
                int x = 1;

                for (i = 'A'; i <= comboLocation1; i++)
                {
                    for (j = 1; j <= comboLocation2; j++)
                    {
                        for (k = 1; k <= comboLocation3; k++)
                        {
                            DatabaseClass.LocationTableAddItem(i, j, k);
                            progressBar1.PerformStep();
                        }
                    }
                }
                Application.Restart();
            }
            else
            {
                MessageBox.Show("You can't leave empty slot.");
            }
        }
    }
}
