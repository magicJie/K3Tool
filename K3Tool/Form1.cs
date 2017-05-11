using System;
using System.Collections.Generic;
using System.Windows.Forms;
using K3Tool.Extend;
using Tool.K3;

namespace K3Tool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var number = Dp.DpPurchasedWarehouse.Work();
            MessageBox.Show("成功新增" + number);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var number = Dp.DpSalesOutLet.Work();
            MessageBox.Show("成功新增" + number);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var number = Dp.DpPicking.Work();
            MessageBox.Show("成功新增" + number);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var number = Dp.DpProductInventory.Work();
            MessageBox.Show("成功新增" + number);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var number = Dp.DpRequisitionSlip.Work();
            MessageBox.Show("成功新增" + number);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var number = Dp.DpNewReceiveBill.Work();
            MessageBox.Show("成功新增" + number);
        }
    }
}
