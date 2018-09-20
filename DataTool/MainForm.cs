using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataTool.Model;
using DevComponents.DotNetBar;
using DevComponents.AdvTree;
using DevComponents.DotNetBar.Controls;

namespace DataTool
{
    public partial class MainForm : Office2007RibbonForm
    {
        private BackgroundWorker _worker;
        private dynamic _scheme= new EntityNotifier<Scheme>();
        private ConfigSettings _configSettings;
        private bool _initIng = true;
        private Dictionary<string, TabItem> _tabItemDic = new Dictionary<string, TabItem>();

        public MainForm()
        {
            InitializeComponent();
            Reload();
            _worker = new BackgroundWorker();
            _worker.DoWork += _worker_DoWork;
            _worker.RunWorkerCompleted += _worker_RunWorkerCompleted;
            _worker.ProgressChanged += _worker_ProgressChanged;
            _initIng = false;//构造完后就记录初始化完毕
        }

        private void Reload()
        {
            //读配置文件
            _configSettings = ConfigSettings.Load();
            //查看配置是否最近一次打开的文件,打开最近文件失败或者配置不加载
            if (_configSettings.OpenRecent && _configSettings.RecentDocs.Count > 0)
            {
                try
                {
                    _scheme = Scheme.Load(_configSettings.RecentDocs[0].FullNanme);
                }
                catch
                {
                    _scheme = null;
                }
            }
            if (_scheme == null && _configSettings.DefaultNew)
            {
                _scheme = new Scheme();
                _scheme.Name = "方案1";
                new DumpTask(_scheme) { Name = "任务1" };
            }
            if (_scheme != null)
            {
                ReloadTreeScheme();
                //将TabItem存入dic中便于管理
                _tabItemDic.Clear();
                _tabItemDic.Add(tabScheme.Name, tabScheme);
                _tabItemDic.Add(tabTaskTemplate.Name, tabTaskTemplate);
                _tabItemDic.Add(tabMappingTemplate.Name, tabMappingTemplate);
                //激活，初始化方案tab
                ReloadSchemeTabItem();
            }
        }

        public void ReloadTreeScheme()
        {
            //加载方案树
            treeScheme.Nodes.Clear();
            var rootNode = new Node
            {
                Name = "nodeScheme",
                Text = _scheme.Name
            };
            treeScheme.Nodes.Add(rootNode);

            for (int i = 0, j = _scheme.TaskList.Count; i < j; i++)
            {
                var task = _scheme.TaskList[i];
                var taskNode = new Node
                {
                    Name = "nodeTask" + i,
                    Text = task.Name
                };
                rootNode.Nodes.Add(taskNode);
                for (int m = 0, n = task.MappingList.Count; m < n; m++)
                {
                    var mapping = task.MappingList[m];
                    var mappingNode = new Node
                    {
                        Name = "nodeMapping" + m,
                        Text = $"映射{m + 1}"
                    };
                    taskNode.Nodes.Add(mappingNode);
                }
            }
            treeScheme.ExpandAll();
        }

        private void ReloadSchemeTabItem()
        {
            tabControlMain.SelectedTab = _tabItemDic[tabScheme.Name];
            if (!_initIng)
                return;
            //绑定下拉选项
            SetEnumBinding(cbUpdateMode, typeof(UpdateMode));
            SetEnumBinding(cbDumpMode, typeof(DumpMode));
            SetEnumBinding(cbDeleteMode, typeof(DeleteMode));
            SetEnumBinding(cbSourceDbType, typeof(DatabaseType));
            SetEnumBinding(cbTargetDbType, typeof(DatabaseType));

            tbSchemeName.DataBindings.Add("Text", _scheme, "Name");
            tbSchemeAuthor.DataBindings.Add("Text", _scheme, "Author");
            dtLastModifiedTime.DataBindings.Add("Value", _scheme, "LastModifiedTime");
            cbUpdateMode.DataBindings.Add("SelectedValue", _scheme, "UpdateMode");
            cbDumpMode.DataBindings.Add("SelectedValue", _scheme, "DumpMode");
            cbDeleteMode.DataBindings.Add("SelectedValue", _scheme, "DeleteMode");
            intInBatchNum.DataBindings.Add("ValueObject", _scheme, "BatchNum");
            cbSourceDbType.DataBindings.Add("SelectedValue", _scheme, "SourceDbType");
            tbSourceConnStr.DataBindings.Add("Text", _scheme, "SourceConnStr");
            cbTargetDbType.DataBindings.Add("SelectedValue", _scheme, "TargetDbType");
            tbTargetConnStr.DataBindings.Add("Text", _scheme, "TargetConnStr");
        }

        private void SetEnumBinding(ComboBoxEx comboBox,Type enumType)
        {
            comboBox.DisplayMember = "Description";
            comboBox.ValueMember = "value";
            comboBox.DataSource = Enum.GetValues(enumType)
                        .Cast<Enum>()
                        .Select(value => new
                        {
                            (Attribute.GetCustomAttribute(value.GetType().GetField(value.ToString()), typeof(DescriptionAttribute)) as DescriptionAttribute)?.Description,
                            value
                        })
                        .OrderBy(item => item.value)
                        .ToList();
        }
        private void _worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //radProgressBar1.Value1 = e.ProgressPercentage * radProgressBar1.Maximum / 100;
        }

        private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                //radProgressBar1.Value1 = 0;
                //todo 取消执行的操作
            }
            else if (e.Error != null)
            {
                //radProgressBar1.Value1 = 0;
                //todo 发生错误时执行的操作
            }
            else
            {
                //todo
            }
        }

        private void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            worker.WorkerReportsProgress = true;
            for (int i = 1; i <= 10; i++)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    Thread.Sleep(500);
                    worker.ReportProgress(i * 10);//报告工作进度
                }
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            _scheme = new Scheme();
            Reload();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = false,
                CheckFileExists = true,
                Filter = "方案文件(*.scheme)|*.scheme"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _scheme = Scheme.Load(dialog.FileName);
                }
                catch
                {
                    MessageBox.Show("提示", "加载文件出错！");
                }
            }
            Reload();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            treeScheme.Focus();//规避输入焦点不移开输入控件导致数据绑定不执行问题
            var dialog = new SaveFileDialog()
            {
                AddExtension = true,
                DefaultExt = ".scheme",
                Filter = "方案文件(*.scheme)|*.scheme",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _scheme.Save(dialog.FileName);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            //提示还有未保存的数据
            _scheme = null;
            Reload();
        }

        private void btnOption_Click(object sender, EventArgs e)
        {

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            //提示还有未保存的数据
            Close();
        }

        private void treeScheme_AfterNodeSelect(object sender, AdvTreeNodeEventArgs e)
        {
            if (e.Node.Name.Contains("scheme"))
            {
                tabControlMain.SelectedTab = _tabItemDic[tabScheme.Name];
            }
            else
            {
                tabControlMain.SelectedTab = _tabItemDic["tab_" + e.Node.Name];
            }
        }
        //停止线程的操作
        //private void button3_Click(object sender, EventArgs e)
        //{
        //    if (backgroundWorker1.WorkerSupportsCancellation)
        //    {
        //        backgroundWorker1.CancelAsync();
        //    }
        //}
    }
}
