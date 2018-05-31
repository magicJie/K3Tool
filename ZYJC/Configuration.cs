using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ZYJC
{
    public class Configuration
    {
        public string Cycle { get => _cycle; }
        public string BackCycle { get => _backCycle; }
        public string MainData { get => _mainData; }
        private static Configuration _configuration;
        private List<Source> _sources;
        private string _cycle;
        private string _backCycle;
        private string _mainData;

        public List<Source> Sources
        {
            get
            {
                return _sources;
            }
        }

        public static Configuration Current
        {
            get
            {
                if (_configuration == null)
                    Load();
                if (_configuration == null)
                    throw new Exception("还未加载配置文件！");
                return _configuration;
            }
        }

        protected Configuration()
        {
            _sources = new List<Source>();
        }

        public static void Load()
        {
            _configuration = new Configuration();
            var doc = new XmlDocument();
            doc.Load("Config.xml");
            var root = doc.SelectSingleNode("sources");
            _configuration._cycle = root.Attributes["cycle"].Value;
            _configuration._backCycle = root.Attributes["backCycle"].Value;
            _configuration._mainData = root.Attributes["mainData"].Value;
            var nodes = doc.SelectNodes("sources/source");
            foreach (XmlNode node in nodes)
            {
                _configuration._sources.Add(new Source
                {
                    Name = node.Attributes["name"].Value,
                    ConnectionString = node.SelectSingleNode("connectionString").Attributes["value"].Value,
                    PlanCodePrefix = node.SelectSingleNode("planCodePrefix").Attributes["value"].Value,
                    Line = node.SelectSingleNode("line").Attributes["value"].Value,
                });
            }
        }
    }

    public class Source
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public string PlanCodePrefix { get; set; }
        public string Line { get; set; }
    }
}
