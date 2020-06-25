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
    public partial class InInvOptions : Form
    {
        public InInvOptions()
        {
            InitializeComponent();
        }

        private void InInvOptions_Load(object sender, EventArgs e)
        {
            label7.Text = DatabaseClass.WhereIsProduct(InInventory.Barcode);
            string[] locations = new string[3];
            locations = DatabaseClass.GetLocationList();

            comboBox1.Items.AddRange(locations[0].Distinct().ToArray().ToList().Select(i => (object)i).ToArray());
            comboBox2.Items.AddRange(Enumerable.Range(1, Int32.Parse(locations[1])).Select(i => (object)i).ToArray());
            comboBox3.Items.AddRange(Enumerable.Range(1, Int32.Parse(locations[2])).Select(i => (object)i).ToArray());
            comboBox1.SelectedItem = null;
            comboBox1.SelectedText = "--select--";
            comboBox2.SelectedItem = null;
            comboBox2.SelectedText = "--select--";
            comboBox3.SelectedItem = null;
            comboBox3.SelectedText = "--select--";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] Location = new string[3];
            Location[0] = comboBox1.SelectedItem.ToString();
            Location[1] = comboBox2.SelectedItem.ToString();
            Location[2] = comboBox3.SelectedItem.ToString();

            DatabaseClass.InInventory(InInventory.Barcode, Decimal.ToInt32(numericUpDown1.Value), Location);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
