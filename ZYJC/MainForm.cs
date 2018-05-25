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
            Configuration.Load();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"成功新增" + new MaterielImporter(Configuration.Current.Sources[0]).UpdateImport(new DateTime(1970, 1, 1), DateTime.Now));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"成功新增" + new BOMImporter(Configuration.Current.Sources[0]).UpdateImport(new DateTime(1970, 1, 1), DateTime.Now));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"成功新增" + new ProductionPlanImporter(Configuration.Current.Sources[0]).UpdateImport(new DateTime(1970, 1, 1), DateTime.Now));
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
