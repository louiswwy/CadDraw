using System;
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
        /// xml文件中写入站点信息
        /// </summary>
        /// <param name="xmlFile">文件地址</param>
        /// <param name="infoStation">字典</param>
        public void addWayPointNode(string xmlFile,string parentNodeName, string nodeName, Dictionary<string, string> infoStation)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFile);

            if (true)
            {
                XmlNode root = xmlDoc.SelectSingleNode("Projet");//查找<Projet> 
                XmlNode subroot = root.SelectSingleNode("WayPoints");//查找<WayPoints> 
                XmlNode stationRoot = subroot.SelectSingleNode(parentNodeName);//查找<StationPointLists> 
                foreach (var item in infoStation)
                {
                    if (!hasElement(stationRoot, item.Key.ToUpper()))//loadedDic.ContainsKey(item.Key))
                    {
                        XmlElement xe1 = xmlDoc.CreateElement(nodeName);//创建一个<WayPoint>节点 
                        xe1.SetAttribute("location", item.Key.ToUpper());//设置该节点location属性 

                        XmlElement subxe1 = xmlDoc.CreateElement("PName");//创建一个<PName>节点 
                        subxe1.InnerText = item.Value.Split(new char[] { ',' })[0];

                        XmlElement subxe2 = xmlDoc.CreateElement("PType");//创建一个<PName>节点 
                        subxe2.InnerText = item.Value.Split(new char[] { ',' })[1];

                        xe1.AppendChild(subxe1);
                        xe1.AppendChild(subxe2);
                        stationRoot.AppendChild(xe1);
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
        public Dictionary<string, string> loadWayPoint(string xmlFile, string nodeName)
        {
            Dictionary<string, string> inforStation = new Dictionary<string, string>();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFile);
            XmlNode root = xmlDoc.SelectSingleNode("Projet");//查找<Projet> 
            XmlNode subroot = root.SelectSingleNode("WayPoints");//查找<WayPoints> 

            XmlNode subStationRoot = subroot.SelectSingleNode(nodeName);//查找<StationPoints>  
            if (subStationRoot.HasChildNodes && subStationRoot != null)
            {
                XmlNodeList nodeList = subStationRoot.ChildNodes;//获取FatherNode节点的所有子节点   
                PFunction pf = new PFunction();

                foreach (var item in nodeList)
                {
                    XmlElement xe = (XmlElement)item;
                    string key = xe.GetAttribute("location");
                    List<string> outList = new List<string>();

                    if(key.Split(new char[] { ',' }).Length == 0)
                    {

                        pf.isExMatch(key, @"^([A-Z]+)(\d+)\+(\d{0,4})$", out outList);
                    }
                    else
                    {

                        pf.isExMatch(key.Split(new char[] { '-' })[0], @"^([A-Z]+)(\d+)\+(\d{0,4})$", out outList);
                    }
                    
                    Int32 DistNum = System.Math.Abs(Int32.Parse(outList[1]) * 1000 + Int32.Parse(outList[2]));

                    XmlNode nameNode = xe.SelectSingleNode("PName");//查找<PName> 
                    XmlNode typeNode = xe.SelectSingleNode("PType");//查找<PType> 

                    string name = nameNode.InnerText;
                    string type = typeNode.InnerText;

                    inforStation.Add(key.ToUpper(), name + "," + type + "," + DistNum);
                }
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
                if (xe.GetAttribute("location").ToUpper() == location)//如果location属性值为location的值   
                {
                    elementFound = true;
                    break;
                }
            }
            return elementFound;
        }


        public void modifWayPoint(string xmlFile,List<string> oldInfor, List<string> newInfor)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFile);
            XmlNode root = xmlDoc.SelectSingleNode("Projet");//查找<Projet> 
            XmlNode subroot = root.SelectSingleNode("WayPoints");//查找<WayPoints> 
            XmlNode wypointRoot = subroot.SelectSingleNode("StationPointLists");//查找<StationPoint> 

            
            foreach (XmlNode xn in wypointRoot.ChildNodes)
            {
                XmlElement xe = (XmlElement)xn;
                //string a = xe.GetAttribute("location").ToString();
                //string b = oldInfor.Keys.ToString().Replace(" ", "");
                if (xe.GetAttribute("location").ToString().Replace(" ", "") == oldInfor[0].Split(new char[]{','})[0])
                {
                    xe.SetAttribute("location", newInfor[0].Split(new char[] { ',' })[0].ToUpper());

                    XmlNodeList nls = xe.ChildNodes;//继续获取xe子节点的所有子节点
                    foreach (XmlNode xn1 in nls)//遍历
                    {
                        XmlElement xe2 = (XmlElement)xn1;//转换类型
                        if (xe2.Name == "PName")//如果找到
                        {
                            xe2.InnerText = newInfor[0].Split(new char[]{','})[1];//则修改
                            
                        }
                        else if (xe2.Name == "PType")//如果找到
                        {
                            xe2.InnerText = newInfor[0].Split(new char[] { ',' })[2];//则修改
                            
                        }
                    }
                    
                    break;//找到退出来就可以了
                }
            }
            xmlDoc.Save(xmlFile);

        }

        /// <summary>
        /// 删除项
        /// </summary>
        /// <param name="xmlFile"></param>
        /// <param name="NodeName"></param>
        public void supprimWayPoint(string xmlFile, string NodeName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFile);
            XmlNode root = xmlDoc.SelectSingleNode("Projet");//查找<Projet> 
            XmlNode subroot = root.SelectSingleNode("WayPoints");//查找<WayPoints> 
            XmlNode wypointRoot = subroot.SelectSingleNode("StationPointLists");//查找<StationPoint> 

            foreach (XmlNode xn in wypointRoot.ChildNodes)
            {
                XmlElement xe = (XmlElement)xn;
                string a = xe.GetAttribute("location").ToString();
                if (xe.GetAttribute("location").ToString() == NodeName)
                {
                    xn.ParentNode.RemoveChild(xn);
                    break;
                }
            }


            xmlDoc.Save(xmlFile);  
        }

        /// <summary>
        /// 删除制定项
        /// </summary>
        /// <param name="xmlFile"></param>
        /// <param name="parentNode"></param>
        /// <param name="NodeName"></param>
        public void supprimWayPoint(string xmlFile, string parentNode, string NodeName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFile);
            XmlNode root = xmlDoc.SelectSingleNode("Projet");//查找<Projet> 
            XmlNode subroot = root.SelectSingleNode("WayPoints");//查找<WayPoints> 
            XmlNode wypointRoot = subroot.SelectSingleNode(parentNode);//查找<StationPoint> 

            if (wypointRoot != null)
            {
                wypointRoot.RemoveAll();
            }


            xmlDoc.Save(xmlFile);
        }

        #region 连接情况信息
        /// <summary>
        /// 向xml文件写入设备连接信息
        /// </summary>
        /// <param name="xmlFile">文件地址</param>
        /// <param name="selectGrid">选中所亭项</param>
        /// <param name="selectBlock">选中的块图案</param>
        public void createConnectionXml(string xmlFile, string selectGrid, string selectBlocks)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFile);

            XmlNode root = xmlDoc.SelectSingleNode("Projet");//查找<Projet> 
            XmlNode subroot = root.SelectSingleNode("Connections");//查找<Connection> 

            if (subroot == null)
            {
                subroot = xmlDoc.CreateElement("Connections"); //添加
                root.AppendChild(subroot);
            }
            XmlElement xe1 = xmlDoc.CreateElement("Pair");//创建一个<Pair>节点 

            xe1.SetAttribute("location", selectGrid.Split(new char[] { ',' })[0]);//设置该节点location属性 
            xe1.SetAttribute("name", selectGrid.Split(new char[] { ',' })[1]);//设置该节点name属性 


            XmlElement subXml1 = xmlDoc.CreateElement("equipement");//创建一个<equipement>节点 
            subXml1.SetAttribute("location", selectBlocks.Split(new char[] { ',' })[0]);


            subXml1.SetAttribute("line", "SPTYWPL,23,28芯");//selectBlocks.Split(new char[] { '-' })[2]);

            subXml1.SetAttribute("Length", selectBlocks.Split(new char[] { ',' })[3]);
            subXml1.InnerText = selectBlocks.Split(new char[] { ',' })[1]; 

            xe1.AppendChild(subXml1);
           

            subroot.AppendChild(xe1);
            xmlDoc.Save(xmlFile);

        }

        public void removeConnection(string xmlFile)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFile);

            XmlNode root = xmlDoc.SelectSingleNode("Projet");//查找<Projet> 
            XmlNode subroot = root.SelectSingleNode("Connection");//查找<Connection> 

            if (subroot == null)
            {
                subroot = xmlDoc.CreateElement("Connection"); //添加
                root.AppendChild(subroot);
            }
            else
            {
                subroot.RemoveAll();
            }

            xmlDoc.Save(xmlFile);

        }

        public void loadConnectionXml(string xmlFile)
        {

        }

        #endregion

        #region 线缆部分
        /// <summary>
        /// 写入线缆类型
        /// </summary>
        /// <param name="xmlFile"></param>
        public void createLineType(string xmlFile,string nodeName, Dictionary<string, string> NameAndType)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFile);


            XmlNode root = xmlDoc.SelectSingleNode("Projet");//查找<Projet> 
            XmlNode xe1 = root.SelectSingleNode(nodeName);
            if (xe1 == null)
            {
                xe1 = xmlDoc.CreateElement(nodeName);//创建一个<LineName>节点 
                root.AppendChild(xe1);
            }

            foreach (var item in NameAndType)
            {
                if (!duplicateXML(xe1.ChildNodes, item.Key.ToString()))
                {
                    string text = item.Value.ToString();
                    string name = text.Split(new char[] { ',' })[0]; //type
                    string sx = text.Split(new char[] { ',' })[1];  //short
                    string num = text.Split(new char[] { ',' })[2]; //num1
                    string numX = text.Split(new char[] { ',' })[3]; //numX
                    string fournisseur = text.Split(new char[] { ',' })[4]; //厂家
                    string minDis = text.Split(new char[] { ',' })[5]; //最近距离
                    string maxDis = text.Split(new char[] { ',' })[6]; //最远距离

                    XmlElement childNode = xmlDoc.CreateElement("Line");//创建一个<Line>节点  
                    childNode.SetAttribute("Type", name);//设置该节点 线缆类型 名字属性 
                    childNode.SetAttribute("Short", sx);//设置该节点 缩写 属性 
                    childNode.SetAttribute("num1", num);//设置该节点 第一个数量 属性 
                    childNode.SetAttribute("numXin", numX);//设置该节点 芯数 属性 
                    childNode.SetAttribute("fournisseur", fournisseur);//设置该节点 芯数 属性 
                    childNode.SetAttribute("minDistance", minDis);//设置该节点 芯数 属性 
                    childNode.SetAttribute("maxDistance", maxDis);//设置该节点 芯数 属性 
                    childNode.InnerText = item.Key.ToString();
                    xe1.AppendChild(childNode);
                }
            }
            xmlDoc.Save(xmlFile);
        }

        /// <summary>
        /// 查找重复项
        /// </summary>
        /// <param name="xmlList"></param>
        /// <param name="toCheck"></param>
        /// <returns></returns>
        public bool duplicateXML(XmlNodeList xmlList, string toCheck)
        {
            bool isDupli = false;
            foreach (XmlElement node in xmlList)
            {
                if (node.InnerText == toCheck)
                {
                    isDupli = true;
                    break;
                }
            }
            return isDupli;
        }

        /// <summary>
        /// 更改线缆型号
        /// </summary>
        /// <param name="xmlFile"></param>
        /// <param name="NodeName"></param>
        /// <param name="NameAndType"></param>
        public void updataLineType(string xmlFile, string NodeName, Dictionary<string, string> NameAndType)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFile);


            XmlNode root = xmlDoc.SelectSingleNode("Projet");//查找<Projet> 


            XmlNode xe1 = root.SelectSingleNode(NodeName);//选择一个<lineName>节点

            if (xe1 != null)
            {
                xe1.RemoveAll(); //删除现有项

                xmlDoc.Save(xmlFile);
                createLineType(xmlFile,NodeName, NameAndType);

            }

        }

        /// <summary>
        /// 读取线缆型号
        /// </summary>
        /// <param name="xmlFile"></param>
        /// <returns></returns>
        public Dictionary<string, string> loadLineType(string xmlFile)
        {
            Dictionary<string, string> retuneValue = new Dictionary<string, string>();
            if (xmlFile != "")
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFile);


                XmlNode root = xmlDoc.SelectSingleNode("Projet");//查找<Projet> 

                //foreach (string item in NameAndType)

                XmlNode xe1 = root.SelectSingleNode("lineName");//创建一个<LineName>节点

                if (xe1 != null)
                {
                    XmlNodeList nodeList = xe1.ChildNodes;//获取FatherNode节点的所有子节点   

                    if (nodeList != null)
                    {
                        foreach (var childNode in nodeList)
                        {
                            XmlElement childElement = (XmlElement)childNode;
                            string type = childElement.GetAttribute("Type");
                            string Short = childElement.GetAttribute("Short");
                            string num1 = childElement.GetAttribute("num1");
                            string num2 = childElement.GetAttribute("numXin");
                            string fourni = childElement.GetAttribute("fournisseur");
                            string minD = childElement.GetAttribute("minDistance");
                            string maxD = childElement.GetAttribute("maxDistance");
                            string fullName = childElement.InnerText;

                            retuneValue.Add(fullName, type + "," + Short + "," + num1 + "," + num2 + "," + fourni + "," + minD + "," + maxD);
                        }
                    }
                }
                else
                {
                    return null;
                }
            }
            return retuneValue;

        }

        #endregion

        #region 连接情况

        /*public void createXmlConnection(string xmlFile)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFile);


            XmlNode root = xmlDoc.SelectSingleNode("Projet");//查找<Projet> 
            XmlNode xe1 = root.SelectSingleNode("Connection");//查找<Connection> 
        }*/
        #endregion

        #region 规则部分
        public void writeRule(string xmlPath, string Nequipe)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);
            XmlNode root = xmlDoc.SelectSingleNode("Rule");//查找<Rule> 
            if (!hasElement(root, xmlPath))
            {
                XmlElement xe1 = xmlDoc.CreateElement("equipement");//创建一个<equipement>节点
                xe1.SetAttribute("Name", Nequipe);

                XmlElement subXE = xmlDoc.CreateElement("Interval");//创建一个<Interval>节点
                xe1.AppendChild(subXE);

                XmlElement subXE1 = xmlDoc.CreateElement("terrain");//创建一个<MaxInterval>节点
                subXE1.SetAttribute("MaxInterval", "");
                subXE1.SetAttribute("MinInterval", "");
                subXE1.InnerText = "路基";

                XmlElement subXE2 = xmlDoc.CreateElement("terrain");//创建一个<MaxInterval>节点
                subXE2.SetAttribute("MaxInterval", "");
                subXE2.SetAttribute("MinInterval", "");
                subXE2.InnerText = "桥梁";

                XmlElement subXE3 = xmlDoc.CreateElement("terrain");//创建一个<MaxInterval>节点
                subXE3.SetAttribute("MaxInterval", "");
                subXE3.SetAttribute("MinInterval", "");
                subXE3.InnerText = "山区垭口";

                root.AppendChild(xe1);
                subXE.AppendChild(subXE1);
                subXE.AppendChild(subXE2);
                subXE.AppendChild(subXE3);

                xmlDoc.Save(xmlPath);
            }


        }

        public void updateRule(string xmlPath, string NameEquipe, List<string> rules)
        {
            string maxDis = rules[0];
            List<string> spWayPoint = new List<string>();
            if (rules.Count > 0)
            {
                rules.RemoveAt(0);
            }
            spWayPoint = rules;

            //
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);

            XmlNode root = xmlDoc.SelectSingleNode("Rule");//查找<Projet> 
            XmlNode subroot = root.SelectSingleNode("equipement");//查找<equipement> 
            subroot.Attributes["MaxInterval"].Value = maxDis; //变更属性值


            //检查值是否重复
            bool isDupli = false;
            foreach (XmlElement subEle in subroot.ChildNodes)
            {
                subroot.RemoveChild(subEle);
            }
            foreach (string rule in spWayPoint)
            {
                XmlElement specPoint = xmlDoc.CreateElement("SpWayPoint");//特殊的所亭

                specPoint.InnerText = rule;

                subroot.AppendChild(specPoint);
            }
        }

        public List<string> loadRule(string xmlPath, string equName)
        {
            List<string> returnList = new List<string>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);

            XmlNode root = xmlDoc.SelectSingleNode("Rule");//查找<Projet> 

            return returnList;
        }

        #endregion


        #region 项目信息
        public void writeProjrtInfo(string xmlPath, string proName, string PictureName,string NumChapter)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);

            XmlNode root = xmlDoc.SelectSingleNode("Projet");//查找<Projet> 
            XmlNode subroot = root.SelectSingleNode("ProjetInfor");//查找<ProjetInfor> 

            XmlNode subrootName = subroot.SelectSingleNode("ProjetName");//查找<ProjetName> 
            subrootName.InnerText = proName;

            XmlNode subrootPName = subroot.SelectSingleNode("PictureName");//查找<PictureName> 
            subrootPName.InnerText = PictureName;

            XmlNode subrootCName = subroot.SelectSingleNode("ChapterName");//查找<ChapterName> 
            subrootCName.InnerText = NumChapter;

            xmlDoc.Save(xmlPath);
            
        }

        public List<string> readProjrtInfo(string xmlPath)//, string proName, string PictureName,string NumChapter)
        {
            try
            {
                List<string> projectInfo = new List<string>();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlPath);

                XmlNode root = xmlDoc.SelectSingleNode("Projet");//查找<Projet> 
                XmlNode subroot = root.SelectSingleNode("ProjetInfor");//查找<ProjetInfor> 

                XmlNode subrootName = subroot.SelectSingleNode("ProjetName");//查找<ProjetName> 
                string proName = subrootName.InnerText;

                XmlNode subrootPName = subroot.SelectSingleNode("PictureName");//查找<PictureName> 
                string PictureName = subrootPName.InnerText;

                XmlNode subrootCName = subroot.SelectSingleNode("ChapterName");//查找<ChapterName> 
                string NumChapter = subrootCName.InnerText;

                projectInfo.Add(proName);
                projectInfo.Add(PictureName);
                projectInfo.Add(NumChapter);
                return projectInfo;
            }
            catch (Exception ee)
            {
                List<string> errorList = new List<string>();
                errorList.Add(ee.ToString());
                return errorList;
                
            }
        }
        #endregion
    }
}


