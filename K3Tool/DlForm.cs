using System;
using System.Collections.Generic;
using System.Windows.Forms;
using K3Tool.Extend;
using Tool.K3;

namespace K3Tool
{
    /// <summary>
    /// 大连窗口--大鹏
    /// </summary>
    public partial class DlForm : Form
    {
        public DlForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var number = Dl.DpPurchasedWarehouse.Work(kstime.Value.ToString("yyyy-MM-dd"), jstime.Value.ToString("yyyy-MM-dd"));
            MessageBox.Show("成功新增" + number);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var number = Dl.DpSalesOutLet.Work(kstime.Value.ToString("yyyy-MM-dd"), jstime.Value.ToString("yyyy-MM-dd"));
            MessageBox.Show("成功新增" + number);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var number = Dl.DpPicking.Work(kstime.Value.ToString("yyyy-MM-dd"), jstime.Value.ToString("yyyy-MM-dd"));
            MessageBox.Show("成功新增" + number);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var number = Dp.DpProductInventory.Work(kstime.Value.ToString("yyyy-MM-dd"), jstime.Value.ToString("yyyy-MM-dd"));
            MessageBox.Show("成功新增" + number);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var number = Dl.DpRequisitionSlip.Work(kstime.Value.ToString("yyyy-MM-dd"), jstime.Value.ToString("yyyy-MM-dd"));
            MessageBox.Show("成功新增" + number);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var number = Dl.DpNewReceiveBill.Work(kstime.Value.ToString("yyyy-MM-dd"), jstime.Value.ToString("yyyy-MM-dd"));
            MessageBox.Show("成功导入" + number+"条数据");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var number = Dl.OtherOutboundBill.Work(kstime.Value.ToString("yyyy-MM-dd"), jstime.Value.ToString("yyyy-MM-dd"));
            MessageBox.Show("成功导入" + number + "条数据");
        }
    }
}
