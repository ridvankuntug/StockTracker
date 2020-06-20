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
    public partial class InInvLocation : Form
    {
        public InInvLocation()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string LocationA = comboBox1.Text;
            string LocationB = comboBox2.Text;
            string LocationC = comboBox3.Text;
            Form InInvNumber = new InInvNumber();
            InInvNumber.Owner = this;//Alt formun parent(bu)'ı kapatabilmesini sağlayacak 
            InInvNumber.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
