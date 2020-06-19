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
    public partial class ParentForm : Form
    {
        public ParentForm()
        {
            InitializeComponent();

            DatabaseClass.CreateDB();
            DatabaseClass.CreateTable();

            Form HomeForm = new HomeForm();
            HomeForm.MdiParent = this;
            HomeForm.Show();
        }
    }
}
