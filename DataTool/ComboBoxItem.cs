using DevComponents.DotNetBar;
using System;
namespace DataTool
{
    public class ComboBoxItem: DevComponents.DotNetBar.ComboBoxItem
    {
        public object Value { set; get; }
        public ComboBoxItem() : base()
        {
        }
        public ComboBoxItem(string name) : base(name)
        {
        }
        public ComboBoxItem(string name, string text) : base(name, text)
        {
        }

        public ComboBoxItem(string name, string text, object value):base(name,text)
        {
            Value = value;
        }
    }
}
