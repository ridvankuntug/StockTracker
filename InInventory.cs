﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace StockTracker
{
    public partial class InInventory : Form
    {
        public InInventory()
        {
            InitializeComponent();
        }

        private void InInventory_Load(object sender, EventArgs e)
        {
            //DataGridFill();
            label3.Visible = false;
            label4.Visible = false;
        }

        private void DataGridFill()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.DataSource = null;

            string date = "AND history_adding_time  BETWEEN '" + DateTime.Today.AddDays(-2).ToString("yyyy-MM-dd") + " 00:00:00' and '" + DateTime.Today.ToString("yyyy-MM-dd") + " 23:59:59'";

            DataSet ds = DatabaseClass.GridFill("*", "in_history_view", "", "", date, "");
            dataGridView1.DataSource = ds.Tables["*"];
            dataGridView1.Columns[0].HeaderText = "Name";
            dataGridView1.Columns[1].HeaderText = "Barcode";
            dataGridView1.Columns[2].Visible = false;
            dataGridView1.Columns[3].HeaderText = "Number";
            dataGridView1.Columns[4].HeaderText = "Location";
            dataGridView1.Columns[5].HeaderText = "Add Date";
            dataGridView1.Columns["product_name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView1.Sort(this.dataGridView1.Columns[4], ListSortDirection.Descending);
        }

        public static int Barcode;

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Enter();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form ManageProducts = new ManageProducts();
            ManageProducts.MdiParent = ParentForm;
            ManageProducts.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Enter();
        }

        private void Enter()
        {
            if (textBox1.Text.Length < 9)
            {
                MessageBox.Show("Barcode must be at 9 characters long.");
            }
            else if (!DatabaseClass.IsBarcodeExist(textBox1.Text, ""))
            {
                MessageBox.Show("Barcode is undefined on products.");
            }
            else
            {
                Barcode = Int32.Parse(textBox1.Text);

                Form InInvOptions = new InInvOptions();
                InInvOptions.ShowDialog(this);
                if (InInvOptions.DialogResult == DialogResult.OK)
                {
                    DataGridFill();
                }
            }
        }

        public static void labelChange(string status, string label)
        {
            System.Threading.Thread threadlabel = new System.Threading.Thread(new System.Threading.ThreadStart(change));
            Control.CheckForIllegalCrossThreadCalls = false;    // THREAD ÇAKIŞMASINI ENGELLER
            threadlabel.Priority = System.Threading.ThreadPriority.Highest;
            threadlabel.Start();
            
            void change(){
                label3.Text = status;
                label3.ForeColor = Color.Green;
                label3.Visible = true;

                label4.Text = label;
                label4.ForeColor = Color.Green;
                label4.Visible = true;

                Thread.Sleep(10000);

                label3.Visible = false;
                label4.Visible = false;

                label3.Text = "";
                label4.Text = "";

            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(textBox1.Text, "  ^ [0-9]"))
            {
                textBox1.Text = "";
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void InInventory_Activated(object sender, EventArgs e)
        {
            DataGridFill();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (MethodsClass.ExportToExcel(dataGridView1))
            {
                MessageBox.Show("Saved");
            }
            else
            {
                MessageBox.Show("Can't saved!");
            }
        }
    }
}
