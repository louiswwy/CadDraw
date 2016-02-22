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
            if (subStationRoot != null)
            {
                if (subStationRoot.HasChildNodes)
                {
                    XmlNodeList nodeList = subStationRoot.ChildNodes;//获取FatherNode节点的所有子节点   
                    PFunction pf = new PFunction();

                    string notdrawList = loadNotDrawBlockList(xmlFile);
                    foreach (var item in nodeList)
                    {
                        XmlElement xe = (XmlElement)item;
                        string key = xe.GetAttribute("location");
                        List<string> outList = new List<string>();

                        if (key.Split(new char[] { ',' }).Length == 0)
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

                        if (!notdrawList.Contains(name)&& !notdrawList.Contains(type)) //如果不在‘不绘制列表中’则输出
                        {
                            inforStation.Add(key.ToUpper(), name + "," + type + "," + DistNum);
                        }
                        else //否则，忽视
                        {

                        }
                    }
                }
            }
            else
            {

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
        /// 删除waypoint项
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
        /// 删除指定waypoint项
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
        /// <param name="selectStation">选中所亭项</param>
        /// <param name="selectEquipement">选中的块图案</param>
        public List<ClassStruct.ConnectionAndLine> createConnectionXml(string xmlFile, List<ClassStruct.LineType> lineTypeList, List<string> StationEquipePairList)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFile);

            XmlNode root = xmlDoc.SelectSingleNode("Projet");//查找<Projet> 
            XmlNode subroot = root.SelectSingleNode("Connections");//查找<Connection> 

            if (subroot == null)
            {
                subroot = xmlDoc.CreateElement("Connections"); //添加Connections节点
                root.AppendChild(subroot);
            }

            List<ClassStruct.ConnectionAndLine> Station_Equipe_Line_List = new List<ClassStruct.ConnectionAndLine>();

            foreach (string StationEquipePair in StationEquipePairList) //循环
            {

                XmlElement xe1 = xmlDoc.CreateElement("Pair");//创建一个<Pair>节点 


                string station = StationEquipePair.Split(new char[] { '_' })[0];
                string equipe = StationEquipePair.Split(new char[] { '_' })[1];
                 
                ClassStruct.StationPoint stationInfor = new ClassStruct.StationPoint(station.Split(new char[] { ',' })[0], station.Split(new char[] { ',' })[1], station.Split(new char[] { ',' })[2], int.Parse(station.Split(new char[] { ',' })[3]));
                //生成自定义 设备信息类
                ClassStruct.EquipePoint equipeInfor = new ClassStruct.EquipePoint(equipe.Split(new char[] { ',' })[0], equipe.Split(new char[] { ',' })[1], equipe.Split(new char[] { ',' })[2], int.Parse(equipe.Split(new char[] { ',' })[3]));

                //站点的里程
                int stationLocation = stationInfor.distance;

                //设备的里程
                int equipementLocation = equipeInfor.distance;

                //设备与基站、所亭间的距离
                int distance = Math.Abs(stationLocation - equipementLocation);

                if (stationInfor.type != "")
                {
                    if (distance == 0)
                    {
                        distance = 100;
                    }
                    else
                    {
                        distance = Convert.ToInt32(Math.Round(distance * 1.2));
                    }
                }

                
                //站点节点
                XmlElement subXmlStation = xmlDoc.CreateElement("Station");//创建一个<Station>节点 
                subXmlStation.SetAttribute("Stationlocation", stationInfor.location);
                subXmlStation.SetAttribute("StationType", stationInfor.type);
                subXmlStation.InnerText = stationInfor.name;

                //设备节点
                XmlElement subXmlEquipe = xmlDoc.CreateElement("Equipement");//创建一个<Equipement>节点 
                subXmlEquipe.SetAttribute("Equipelocation", equipeInfor.location);
                subXmlEquipe.SetAttribute("EquipeType", equipeInfor.type);
                subXmlEquipe.InnerText = equipeInfor.name;

                //线型子节点
                XmlElement subXmlLine = xmlDoc.CreateElement("Line");//创建一个<Line>节点 

                ClassStruct.LineInfor LineInformation;
                //按规则选择线型  distance
                string selectLineType = ""; //选中的线型
                foreach (ClassStruct.LineType lineInfo in lineTypeList)
                {
                    int minDis = Convert.ToInt32(lineInfo.minLength);
                    int maxDis = Convert.ToInt32(lineInfo.maxLength);
                    if (minDis <= distance && distance < maxDis)
                    {
                        selectLineType = lineInfo.name;   //如果设备离站点距离在线型适用范围内

                        LineInformation = new ClassStruct.LineInfor(lineInfo.shortfor, lineInfo.name, lineInfo.fournisseurName, lineInfo.lineType, lineInfo.lineXin, lineInfo.maxLength, lineInfo.minLength, distance);

                        ClassStruct.ConnectionAndLine StationEquipeLine = new ClassStruct.ConnectionAndLine(stationInfor, equipeInfor, LineInformation);
                        Station_Equipe_Line_List.Add(StationEquipeLine);
                        break;  //则跳出
                    }
                }
                
                subXmlLine.SetAttribute("distance", distance.ToString());//设置该节点distance属性，
                                                                                                       //subXmlLine.SetAttribute("LineName",)
                subXmlLine.InnerText = selectLineType;//

                xe1.AppendChild(subXmlStation);
                xe1.AppendChild(subXmlEquipe);
                xe1.AppendChild(subXmlLine);

                subroot.AppendChild(xe1);
            }


            
            xmlDoc.Save(xmlFile);
            return Station_Equipe_Line_List;
        }

        public void removeConnection(string xmlFile)
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
            else
            {
                subroot.RemoveAll();
            }

            xmlDoc.Save(xmlFile);

        }

        public void removeConnection(string xmlFile, string removeItem)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFile);

            XmlNode root = xmlDoc.SelectSingleNode("Projet");//查找<Projet> 
            XmlNode subroot = root.SelectSingleNode("Connections");//查找<Connections> 

            if (subroot != null)
            {
                foreach (XmlNode childNode in subroot.ChildNodes) //遍历<pair>子节点
                {
                    //<pair>子节点
                    XmlNode StationRoot = childNode.SelectSingleNode("Station");//查找<Station>节点
                    if (StationRoot != null)
                    {
                        if (StationRoot.InnerText == removeItem)
                        {
                            subroot.RemoveChild(childNode); //删除对应<pair>节点
                        }
                    }
                }

            }
            else
            {
                //subroot.RemoveAll();
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
                    childNode.SetAttribute("LineType", num);//设置该节点 类型 属性（铠装） 
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
                            string num1 = childElement.GetAttribute("LineType");
                            string num2 = childElement.GetAttribute("numXin");
                            string fourni = childElement.GetAttribute("fournisseur");
                            string minD = childElement.GetAttribute("minDistance");
                            string maxD = childElement.GetAttribute("maxDistance");
                            string fullName = childElement.InnerText;

                            retuneValue.Add(fullName, type + "," + Short + "," + num1 + "," + num2 + "," + fourni + "," + minD + "," + maxD);//
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


        #region 规则部分 没有使用 
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
        public void writeProjrtInfo(string xmlPath, string proName, string PictureName, string NumChapter, string ProjetShortName)
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

            XmlNode subrootCShort = subroot.SelectSingleNode("ProjectShortName");//查找<ChapterName> 
            subrootCShort.InnerText = ProjetShortName;

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

                XmlNode subrootPShort = subroot.SelectSingleNode("ProjectShortName");//查找<ChapterName> 
                string NumPShort = subrootPShort.InnerText;

                projectInfo.Add(proName);
                projectInfo.Add(PictureName);
                projectInfo.Add(NumChapter);
                projectInfo.Add(NumPShort);
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

        #region 不绘制的站点
        public bool addNotDrawBlock(string xmlFile,ClassStruct.StationPoint blockInfo)
        {
            bool tryInsert = false;
            try
            {


                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFile);

                XmlNode root = xmlDoc.SelectSingleNode("Projet");//查找<Projet> 
                XmlNode wpRoot = root.SelectSingleNode("WayPoints");//查找<WayPoints> 
                XmlNode ndRoot = wpRoot.SelectSingleNode("NotDrawStationPointLists");//查找<NotDrawStationPointLists> 

                if (ndRoot == null) //如果不存在则添加‘NotDrawStationPointLists’节点
                {
                    wpRoot = xmlDoc.CreateElement("NotDrawStationPointLists"); //添加Connections节点
                    wpRoot.AppendChild(ndRoot);
                }
                else
                {
                    if (!hasElement(ndRoot, blockInfo.location)) //如果不含有该项则新增
                    {
                        XmlElement xe1 = xmlDoc.CreateElement("StationPoints");//创建一个<WayPoint>节点 
                        xe1.SetAttribute("location", blockInfo.location);//设置该节点location属性 

                        XmlElement subxe1 = xmlDoc.CreateElement("PName");//创建一个<PName>节点 
                        subxe1.InnerText = blockInfo.name;

                        XmlElement subxe2 = xmlDoc.CreateElement("PType");//创建一个<PName>节点 
                        subxe2.InnerText = blockInfo.type;

                        xe1.AppendChild(subxe1);

                        xe1.AppendChild(subxe2);

                        ndRoot.AppendChild(xe1);
                    }
                }
                xmlDoc.Save(xmlFile);
                tryInsert = true;
            }
            catch(System.Exception ex)
            {
                string errorMessage = ex.Message.ToString();
            }

            return tryInsert;
        }

        public bool supNotDrawBlock(string xmlFile, ClassStruct.StationPoint blockInfo)
        {
            bool tryInsert = false;
            try
            {


                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlFile);

                XmlNode root = xmlDoc.SelectSingleNode("Projet");//查找<Projet> 
                XmlNode wpRoot = root.SelectSingleNode("WayPoints");//查找<WayPoints> 
                XmlNode ndRoot = wpRoot.SelectSingleNode("NotDrawStationPointLists");//查找<NotDrawStationPointLists> 

                if (ndRoot != null) //如果不存在则添加‘NotDrawStationPointLists’节点
                {
                    foreach(XmlNode xN in ndRoot.ChildNodes)
                    {
                        if(xN.Attributes["location"].Value == blockInfo.location)
                        {
                            if (xN.SelectSingleNode("PName").InnerText == blockInfo.name)
                            {
                                if(xN.SelectSingleNode("PType").InnerText == blockInfo.type)
                                {
                                    ndRoot.RemoveChild(xN); //如果有则删除
                                }
                            }
                        }
                    }
                    xmlDoc.Save(xmlFile);
                }


                tryInsert = true;
            }
            catch (System.Exception ex)
            {
                string errorMessage = ex.Message.ToString();
            }

            return tryInsert;
        }

        public string loadNotDrawBlockList(string xmlFile)
        {
            string List_NotDraw = "";

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFile);

            XmlNode root = xmlDoc.SelectSingleNode("Projet");//查找<Projet> 
            XmlNode wpRoot = root.SelectSingleNode("WayPoints");//查找<WayPoints> 
            XmlNode ndRoot = wpRoot.SelectSingleNode("NotDrawStationPointLists");//查找<NotDrawStationPointLists> 

            if (ndRoot != null)
            {
                XmlNodeList ndSRoot = ndRoot.SelectNodes("StationPoints");
                foreach (XmlNode sigNode in ndSRoot)
                {
                    List_NotDraw += sigNode.SelectSingleNode("PName").InnerText + sigNode.SelectSingleNode("PType").InnerText + ",";
                }
            }
            return List_NotDraw;
            
        }

        public string loadNotDrawBlockList(string xmlFile, out List<ClassStruct.StationPoint> list_not_draw_station)
        {
            string List_NotDraw = "";
            ClassStruct.StationPoint not_draw;
            list_not_draw_station = new List<ClassStruct.StationPoint>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFile);

            XmlNode root = xmlDoc.SelectSingleNode("Projet");//查找<Projet> 
            XmlNode wpRoot = root.SelectSingleNode("WayPoints");//查找<WayPoints> 
            XmlNode ndRoot = wpRoot.SelectSingleNode("NotDrawStationPointLists");//查找<NotDrawStationPointLists> 

            if (ndRoot != null)
            {
                XmlNodeList ndSRoot = ndRoot.SelectNodes("StationPoints");
                foreach (XmlNode sigNode in ndSRoot)
                {
                    string name = sigNode.SelectSingleNode("PName").InnerText;
                    string type = sigNode.SelectSingleNode("PType").InnerText;
                    string location = sigNode.Attributes["location"].InnerText;

                    List_NotDraw += name + type + ",";

                    not_draw = new ClassStruct.StationPoint(location, name, type, 0);

                    list_not_draw_station.Add(not_draw);
                }
            }
            return List_NotDraw;

        }
        #endregion
    }
}


