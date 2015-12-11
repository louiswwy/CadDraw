﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AutoDraw
{
    class XmlFunction
    {
        Dictionary<string, string> loadedDic;

        /// <summary>
        /// xml文件中写入wayPoint
        /// </summary>
        /// <param name="xmlFile">文件地址</param>
        /// <param name="infoStation">字典</param>
        public void addWayPointNode(string xmlFile, Dictionary<string, string> infoStation)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFile);

            if (true)
            {                                           
                XmlNode root = xmlDoc.SelectSingleNode("Projet");//查找<WayPoints> 
                XmlNode subroot = root.SelectSingleNode("WayPoints");//查找<WayPoints> 
                foreach (var item in infoStation)
                {
                    if (!hasElement(subroot, item.Key.ToUpper()))//loadedDic.ContainsKey(item.Key))
                    {
                        XmlElement xe1 = xmlDoc.CreateElement("WayPoint");//创建一个<WayPoint>节点 
                        xe1.SetAttribute("location", item.Key.ToUpper());//设置该节点location属性 

                        XmlElement subxe1 = xmlDoc.CreateElement("PName");//创建一个<PName>节点 
                        subxe1.InnerText = item.Value.Split(new char[] { ',' })[0];

                        XmlElement subxe2 = xmlDoc.CreateElement("PType");//创建一个<PName>节点 
                        subxe2.InnerText = item.Value.Split(new char[] { ',' })[1];

                        xe1.AppendChild(subxe1);
                        xe1.AppendChild(subxe2);
                        subroot.AppendChild(xe1);
                    }

                }
                xmlDoc.Save(xmlFile);   
                //loadedDic = infoStation;
            }
        }

        /// <summary>
        /// 读取xml文件
        /// </summary>
        /// <param name="xmlFile">文件地址</param>
        public Dictionary<String,string> loadWayPoint(string xmlFile)
        {
            Dictionary<string, string> inforStation = new Dictionary<string, string>();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFile);
            XmlNode root = xmlDoc.SelectSingleNode("Projet");//查找<WayPoints> 
            XmlNode subroot = root.SelectSingleNode("WayPoints");//查找<WayPoints> 

            XmlNodeList nodeList = subroot.ChildNodes;//获取FatherNode节点的所有子节点   
            foreach (var item in nodeList)
            {
                XmlElement xe = (XmlElement)item;
                string key = xe.GetAttribute("location");

                XmlNode nameNode = xe.SelectSingleNode("PName");//查找<PName> 
                XmlNode typeNode = xe.SelectSingleNode("PType");//查找<PType> 

                string name = nameNode.InnerText;
                string type = typeNode.InnerText;

                inforStation.Add(key.ToUpper(), name + "," + type);
            }
            return inforStation;

        }

        /// <summary>
        /// 在xml文件，location子节点下查找是否已经有dictionary中的项，如果有返回true。
        /// </summary>
        /// <param name="FatherNode">WayPoints节点</param>
        /// <param name="location">dictionary中的location属性</param>
        /// <returns>是否包含项</returns>
        public bool hasElement(XmlNode FatherNode, string location)
        {
            bool elementFound = false;
            XmlNodeList nodeList = FatherNode.ChildNodes;//获取FatherNode节点的所有子节点   
            foreach (var node in nodeList)
            {
                XmlElement xe = (XmlElement)node;//将子节点类型转换为XmlElement类型   
                if (xe.GetAttribute("location").ToUpper() == location)//如果genre属性值为“李赞红”   
                {
                    elementFound = true;
                    break;
                }
            }
            return elementFound;
        }
    }
}
