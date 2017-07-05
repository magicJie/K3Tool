using System;
using System.Windows.Forms;
using K3Tool.Extend;

namespace K3Tool
{
    public partial class DbForm : Form
    {
        public DbForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var number = Db.DbPurchasedWarehouse.Work(kstime.Value.ToString("yyyy-MM-dd"), jstime.Value.ToString("yyyy-MM-dd"));            
            MessageBox.Show(@"成功新增" + number);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var number = Db.DbSalesOutLet.Work(kstime.Value.ToString("yyyy-MM-dd"), jstime.Value.ToString("yyyy-MM-dd"));            
            MessageBox.Show(@"成功新增" + number);
        }
       
    }


}
