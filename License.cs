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
    public partial class License : Form
    {
        public License()
        {
            InitializeComponent();
        }

        private void License_Load(object sender, EventArgs e)
        {
            button1.Enabled = false;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:ridvankuntug@gmail.com?subject=I Want to Buy a License&body=I want to buy a license for the Stock Tracker application. Can you send me price information and purchase instructions.");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:ridvankuntug@gmail.com?subject=I I Have a License But Not Working&body=I Have a license for the Stock Tracker application but not working. Can you contact me about this.");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form EULA = new EULA();
            EULA.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                if (textBox1.Text != null && textBox2.Text != null)
                {
                    if (MethodsClass.LicenseCheck(MethodsClass.CreateKey(textBox1.Text), textBox2.Text))
                    {
                        Regedit.Write(textBox1.Text, textBox2.Text);
                        MessageBox.Show("Thank you for your purchase.");
                        Application.Restart();
                    }
                    else
                    {
                        MessageBox.Show("The license is not valid, check it and if it still doesn't work, contact the developer.");
                    }
                }
                else
                {
                    MessageBox.Show("Be sure to fill in all the fields.");
                }
            }
            else
            {
                MessageBox.Show("You must accept EULA first.");
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                button1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
            }
        }

    }
}
