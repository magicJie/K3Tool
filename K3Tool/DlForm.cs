using System;
using System.Windows.Forms;
using K3Tool.Extend;

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
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var number = Dl.DlPurchasedWarehouse.Work(kstime.Value.ToString("yyyy-MM-dd"), jstime.Value.ToString("yyyy-MM-dd"));
            MessageBox.Show(@"成功新增" + number);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var number = Dl.DlRefund.Work(kstime.Value.ToString("yyyy-MM-dd"), jstime.Value.ToString("yyyy-MM-dd"));
            MessageBox.Show(@"成功新增" + number);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var number = Dl.DlPicking.Work(kstime.Value.ToString("yyyy-MM-dd"), jstime.Value.ToString("yyyy-MM-dd"));
            MessageBox.Show(@"成功新增" + number);
        }
        /// <summary>
        /// 产品入库单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            var number = Dl.DlProductInventory.Work(kstime.Value.ToString("yyyy-MM-dd"), jstime.Value.ToString("yyyy-MM-dd"));
            MessageBox.Show(@"成功新增" + number);
        }
        /// <summary>
        /// 销售出库单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            var number = Dl.DlSalesOutLet.Work(kstime.Value.ToString("yyyy-MM-dd"), jstime.Value.ToString("yyyy-MM-dd"));
            MessageBox.Show(@"成功新增" + number);
        }
        /// <summary>
        /// 其他出库单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            var number = Dl.DlOtherOutboundBill.Work(kstime.Value.ToString("yyyy-MM-dd"), jstime.Value.ToString("yyyy-MM-dd"));
            MessageBox.Show(@"成功新增" + number);
        }
    }
}
