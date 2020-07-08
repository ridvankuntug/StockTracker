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
    public partial class EULA : Form
    {
        public EULA()
        {
            InitializeComponent();
        }

        private void EULA_Load(object sender, EventArgs e)
        {
            string curDir = System.IO.Directory.GetCurrentDirectory(); 
            this.webBrowser1.Url = new Uri(String.Format("file:///{0}/EULA.html", curDir));
        }
    }
}
