using System;
using System.Collections.Generic;
using System.Windows.Forms;
using K3Tool.Extend;
using Tool.K3;

namespace K3Tool
{
    public partial class gb : Form
    {
        public gb()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var time = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            var number = Gb.GbPurchasedWarehouse.Work(time);            
            MessageBox.Show("成功新增" + number);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var time = dateTimePicker2.Value.ToString("yyyy-MM-dd");
            var number =Gb.GbSalesOutLet.Work(time);            
            MessageBox.Show("成功新增" + number);
        }
       
    }


}
