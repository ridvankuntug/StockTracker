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
    public partial class OutInvOptions : Form
    {
        public OutInvOptions()
        {
            InitializeComponent();
        }

        private void OutInvOptions_Load(object sender, EventArgs e)
        {
            label8.Text = DatabaseClass.ProductName(OutInventory.Barcode);
            label7.Text = "";
            List<List<string>> locations = new List<List<string>>();
            locations = DatabaseClass.GetStockLocations(OutInventory.Barcode);

            comboBox1.Items.Clear();
            for(int i = 0; i < locations[0].Count; i++)
            {
                comboBox1.Items.Add(locations[0][i]);
                
                label7.Text = label7.Text + locations[0][i] + ": " + locations[1][i] + ";  ";
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string Location;
            try
            {
                if (comboBox1.SelectedItem == null)
                {
                    MessageBox.Show("Location is not selected correctly.");
                }
                else if (numericUpDown1.Value < 1)
                {
                    MessageBox.Show("The number cannot be zero or negative.");
                }
                else
                {
                    Location = comboBox1.SelectedItem.ToString();

                    if (!DatabaseClass.OutInvIsExist(OutInventory.Barcode, Location))
                    {
                        MessageBox.Show("Stock not found");
                        this.Close();
                    }
                    else if(!DatabaseClass.EnoughInStock(OutInventory.Barcode, Location, Decimal.ToInt32(numericUpDown1.Value)))
                    {
                        if (DatabaseClass.IsStockZero(OutInventory.Barcode, Location, Decimal.ToInt32(numericUpDown1.Value)))
                        {
                            DatabaseClass.OutInventoryDelete(OutInventory.Barcode, Decimal.ToInt32(numericUpDown1.Value), Location);
                            this.Close();
                        }
                        else {
                            MessageBox.Show("There is not enough product in the division to perform the operation. \n Check the number or location.");
                            this.DialogResult = DialogResult.No;
                            this.Close();
                        }
                    }
                    else
                    {
                        DatabaseClass.OutInventoryUpdate(OutInventory.Barcode, Decimal.ToInt32(numericUpDown1.Value), Location);
                        this.Close();
                    }

                }
            }
            catch (NullReferenceException InvalidThrow)
            {
                MessageBox.Show("Location is not selected correctly. Message: " + InvalidThrow.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
