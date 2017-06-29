using System;
using System.Collections.Generic;
using System.Windows.Forms;
using K3Tool.Extend;
using Tool.K3;

namespace K3Tool
{
    public partial class GbForm : Form
    {
        public GbForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var number = Gb.GbPurchasedWarehouse.Work(kstime.Value.ToString("yyyy-MM-dd"), jstime.Value.ToString("yyyy-MM-dd"));            
            MessageBox.Show("成功新增" + number);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var number = Gb.GbSalesOutLet.Work(kstime.Value.ToString("yyyy-MM-dd"), jstime.Value.ToString("yyyy-MM-dd"));            
            MessageBox.Show("成功新增" + number);
        }
       
    }


}
