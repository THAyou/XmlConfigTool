using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace XmlConfigTool
{
    /// <summary>
    /// Xml配置文件工具类
    /// 作者：Tyou
    /// 版本:v1.5
    /// *增加快捷添加配置功能 v1.1
    /// *遍历XML时,可读取注释 v1.2
    /// *增加自由选择Xml关键内容 v1.3
    /// *文件路径参数改善，可以选择传入类型 v1.4
    /// *增加实时载入XML文件功能,构造函数优化,性能优化 v1.5
    /// *修改增加配置时,去掉多余的注释项 v1.5.1
    /// </summary>
    public class XmlConfigTool
    {
        public XmlConfigTool(string Path, string FileName)
        {
            ConfigFinder(Path + FileName);
            ConfigList = _ConfigList;
        }

        public XmlConfigTool(string Path, PathType Type)
        {
            switch (Type) {
                case PathType.FileName:
                    ConfigFinder(Directory.GetCurrentDirectory() + Path);
                    ConfigList = _ConfigList;
                    break;
                case PathType.PathFileName:
                    ConfigFinder(Path);
                    ConfigList = _ConfigList;
                    break;
            }
        }

        public XmlConfigTool(string FileName)
        {
            ConfigFinder(Directory.GetCurrentDirectory() + FileName);
            ConfigList = _ConfigList;
        }

        public XmlConfigTool()
        {
        }
        /// <summary>
        /// 配置内容
        /// </summary>
        public List<ConfigContent> ConfigList { get { return _ConfigList; } set { } }

        private static List<ConfigContent> _ConfigList { get; set; }

        /// <summary>
        /// XML文件操作选项
        /// </summary>
        public static XmlDocument _XmlDocument = null;

        /// <summary>
        /// 默认路径
        /// </summary>
        private string _defaultPath = Directory.GetCurrentDirectory() + "//App.config";

        /// <summary>
        /// 最终配置文件路径
        /// </summary>
        private string _configPath = null;

        /// <summary>
        /// 配置节点关键字
        /// </summary>
        private string _appSettings = "appSettings";

        /// <summary>
        /// 配置节点关键内容
        /// </summary>
        private string _configuration = "configuration";

        /// <summary>
        /// 配置节点关键节点集合
        /// </summary>
        private List<string> _configurationList { get; set; } = new List<string>();

        /// <summary>
        /// 修改文件路径
        /// </summary>
        private void ConfigFinder(string Path)
        {
            _configPath = Path;
            LoadConfigurationXml();
        }

        /// <summary>
        /// 修改文件路径
        /// </summary>
        /// <param name="Path"></param>
        public void FinderPath(string Path)
        {
            _configPath = Path;
            LoadConfigurationXml();
        }

        /// <summary>
        /// 修改配置节点关键字
        /// </summary>
        public void FinderAppSettings(string AppSettings)
        {
            _appSettings = AppSettings;
        }

        public void FinderConfiguration(string Configuration)
        {
            _configuration = Configuration;
        }

        /// <summary>
        /// 增加关键配置节点
        /// </summary>
        /// <param name="Setting"></param>
        public void Adduration(string Setting)
        {
            _configurationList.Add(Setting);
            LoadConfigurationXml();
        }

        /// <summary>
        /// 增加关键配置节点(顺序依照List顺序)
        /// </summary>
        /// <param name="Setting"></param>
        public void Adduration(List<string> Setting)
        {
            _configurationList.AddRange(Setting);
            LoadConfigurationXml();
        }

        /// <summary>
        /// 获取关键内容Node
        /// </summary>
        /// <returns></returns>
        private XmlNode GetappSettings()
        {
            var ConfigNode = _XmlDocument as XmlNode;
            foreach (var ation in _configurationList)
            {
                ConfigNode = ConfigNode.SelectSingleNode(ation);
            }
            return ConfigNode;
        }

        /// <summary>
        /// 加载(刷新)Xml文件
        /// </summary>
        public void Load()
        {
            ConfigFinder(_defaultPath);
            ConfigList = _ConfigList;
        }

        /// <summary>
        /// 重新加载Xml文件
        /// </summary>
        /// <param name="Path">文件路径或文件名</param>
        /// <param name="Type">参数类型</param>
        public void Load(string Path, PathType Type)
        {
            switch (Type)
            {
                case PathType.FileName:
                    ConfigFinder(Directory.GetCurrentDirectory() + Path);
                    ConfigList = _ConfigList;
                    break;
                case PathType.PathFileName:
                    ConfigFinder(Path);
                    ConfigList = _ConfigList;
                    break;
            }
        }

        /// <summary>
        /// 读取配置文件内容(Xml)
        /// </summary>
        private void LoadConfigurationXml()
        {
            try
            {
                var configColltion = new List<ConfigContent>();
                _XmlDocument = LoadXmlFile(_configPath);
                if (_XmlDocument == null || !(_XmlDocument is XmlDocument)) return;
                if (_configurationList.Count == 0)
                {
                    _configurationList.Add(_configuration);
                    _configurationList.Add(_appSettings);
                }

                foreach (XmlNode node in GetappSettings())
                {
                    if (node.NodeType == XmlNodeType.Element)
                    {
                        configColltion.Add(new ConfigContent
                        {
                            key = node.Attributes["key"] == null ? "" : node.Attributes["key"].Value,
                            value = node.Attributes["value"] == null ? "" : node.Attributes["value"].Value,
                            ramark = node.Attributes["remarks"] == null ? "" : node.Attributes["remarks"].Value,
                            XType = XmlNodeType.Element
                        });
                    }
                    else if (node.NodeType == XmlNodeType.Comment)
                    {
                        configColltion.Add(new ConfigContent
                        {
                            value = node.Value,
                            XType = XmlNodeType.Comment
                        });
                    }
                }
                _ConfigList = configColltion;
            }
            catch (Exception ex)
            {
                _ConfigList = new List<ConfigContent>();
            }
        }

        /// <summary>
        /// 解析XML文件
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        /// <returns></returns>
        private static XmlDocument LoadXmlFile(string FilePath)
        {
            var Xmld = new XmlDocument();
            try
            {
                Xmld.Load(FilePath);
            }
            catch { }
            return Xmld;
        }

        /// <summary>
        /// 加载输入的Xml
        /// </summary>
        /// <param name="XmlStr"></param>
        /// <returns></returns>
        private static XmlDocument LoadXmlString(string XmlStr)
        {
            var Xmld = new XmlDocument();
            try
            {
                Xmld.LoadXml(XmlStr);
            }
            catch { }
            return Xmld;
        }

        /// <summary>
        /// 追加Node
        /// </summary>
        /// <param name="node"></param>
        private void AppendNode(XmlNode node)
        {
            var ConfigNode = _XmlDocument as XmlNode;
            foreach (var ation in _configurationList)
            {
                ConfigNode = ConfigNode.SelectSingleNode(ation);
            }
            ConfigNode.AppendChild(node);
        }

        /// <summary>
        /// 输入一个XML，添加到Config(快捷添加配置)
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public string AddXmlStr(string xml)
        {
            try
            {
                var XmlD = LoadXmlString(xml);
                foreach (XmlNode node in XmlD)
                {
                    switch (node.NodeType)
                    {
                        case XmlNodeType.Element:
                            XmlElement Xnode = _XmlDocument.CreateElement("add");
                            Xnode.SetAttribute("key", node.Attributes["key"] == null ? "" : node.Attributes["key"].Value);
                            Xnode.SetAttribute("value", node.Attributes["value"] == null ? "" : node.Attributes["value"].Value);
                            Xnode.SetAttribute("remarks", node.Attributes["remarks"] == null ? "" : node.Attributes["remarks"].Value);
                            AppendNode(Xnode);
                            break;
                        case XmlNodeType.Comment:
                            var CommentNode = node as XmlComment;
                            XmlComment remakeNode = _XmlDocument.CreateComment(CommentNode.Value);
                            AppendNode(CommentNode);
                            break;
                    }
                }
                //要想使对xml文件所做的修改生效，必须执行以下Save方法
                _XmlDocument.Save(_configPath);
                LoadConfigurationXml();
                return "success";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// 根据Key获取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string key)
        {
            LoadConfigurationXml();
            return ConfigList.Where(m=>m.key==key).FirstOrDefault().value;
        }

        /// <summary>
        /// 根据key修改对应的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string SetValue(string key, string value)
        {
            try
            {
                foreach (XmlNode node in GetappSettings())
                {
                    if (node.Name == "add")
                    {
                        if (node.Attributes["key"] != null)
                        {
                            if (node.Attributes["key"].Value == key)
                            {
                                node.Attributes["value"].Value = value;
                            }
                        }
                    }
                }
                //要想使对xml文件所做的修改生效，必须执行以下Save方法
                _XmlDocument.Save(_configPath);
                LoadConfigurationXml();
                return "success";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// 根据key修改对应的值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">valus</param>
        /// <param name="ramark">注释</param>
        /// <returns></returns>
        public string SetValue(string key, string value, string ramark)
        {
            try
            {
                foreach (XmlNode node in GetappSettings())
                {
                    if (node.Name == "add")
                    {
                        if (node.Attributes["key"].Value != null)
                        {
                            if (node.Attributes["key"].Value == key)
                            {
                                node.Attributes["value"].Value = value;
                                if (node.Attributes["remarks"] == null)
                                {
                                    var no=node as XmlElement;
                                    no.SetAttribute("remarks", ramark);
                                }
                                else
                                {
                                    node.Attributes["remarks"].Value = ramark;
                                }
                                
                            }
                        }
                    }
                }
                //要想使对xml文件所做的修改生效，必须执行以下Save方法
                _XmlDocument.Save(_configPath);
                LoadConfigurationXml();
                return "success";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// 添加配置
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="ramark">注释</param>
        /// <returns></returns>
        public string AddConfig(string key, string value, string ramark)
        {
            try
            {
                var Xml = GetappSettings();
                XmlComment remakeNode = _XmlDocument.CreateComment(ramark);
                XmlElement node = _XmlDocument.CreateElement("add");
                node.SetAttribute("key",key);   
                node.SetAttribute("value", value);
                if (ramark != null && ramark != "")
                {
                    node.SetAttribute("remarks", ramark);
                    AppendNode(remakeNode);
                }
                AppendNode(node);
                //要想使对xml文件所做的修改生效，必须执行以下Save方法
                _XmlDocument.Save(_configPath);
                LoadConfigurationXml();
                return "success";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// 添加配置
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns></returns>
        public string AddConfig(string key, string value)
        {
            try
            {
                var Xml = GetappSettings();
                XmlElement node = _XmlDocument.CreateElement("add");
                node.SetAttribute("key", key);
                node.SetAttribute("value", value);
                AppendNode(node);
                //要想使对xml文件所做的修改生效，必须执行以下Save方法
                _XmlDocument.Save(_configPath);
                LoadConfigurationXml();
                return "success";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// 根据Key删除配置
        /// </summary>
        /// <param name="key">key</param>
        /// <returns></returns>
        public string DelConfig(string key)
        {
            try
            { 
                foreach (XmlNode node in GetappSettings())
                {
                    if (node.Name == "add")
                    {
                        if (node.Attributes["key"] != null)
                        {
                            if (node.Attributes["key"].Value == key)
                            {
                                GetappSettings().RemoveChild(node);
                            }
                        }
                    }
                }
                _XmlDocument.Save(_configPath);
                LoadConfigurationXml();
                return "success";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

    }

    /// <summary>
    /// ConfigContent
    /// </summary>
    public class ConfigContent
    {
        /// <summary>
        /// key
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// 配置值
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// 注释
        /// </summary>
        public string ramark { get; set; }

        /// <summary>
        /// Xml类型
        /// </summary>
        public XmlNodeType XType { get; set; }
    }

    /// <summary>
    /// 输入的文件地址类型
    /// </summary>
    public enum PathType
    {
        FileName = 0,
        PathFileName = 1,
    }
}
