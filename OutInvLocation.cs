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
    public partial class OutInvLocation : Form
    {
        public OutInvLocation()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form OutInvNumber = new OutInvNumber();
            OutInvNumber.Owner = this;//Alt formun parent(bu)'ı kapatabilmesini sağlayacak 
            OutInvNumber.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
