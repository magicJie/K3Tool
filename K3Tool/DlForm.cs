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
    }
}
