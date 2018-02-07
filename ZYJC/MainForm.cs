using System;
using System.Windows.Forms;
using ZYJC.Importer;


namespace ZYJC
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"成功新增" + new MaterielImporter().InitImport(kstime.Value, jstime.Value));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"成功新增" + new MaterielBOMImporter().InitImport(kstime.Value, jstime.Value));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"成功新增" + new ProductionPlanImporter().InitImport(kstime.Value, jstime.Value));
        }
    }
}
