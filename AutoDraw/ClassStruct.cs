using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDraw
{
    public class ClassStruct
    {
        public class ProjectInfo
        {
            //工程名称
            //阶段
            //图号规则
            //缩写+图册+张

            private string _projetName;
            private string _projetPhase;
            private ProjectPrintPattern _printPattern;
            public string ProjectName
            {
                get
                {
                    return this._projetName;
                }
                set
                {
                    this._projetName = value;
                }
            }
            public string ProjectPhase
            {
                get
                {
                    return this._projetPhase;
                }
                set
                {
                    this._projetPhase = value;
                }
            }
            public ProjectPrintPattern PrintNamePattern
            {
                get
                {
                    return this._printPattern;
                }
                set
                {
                    this._printPattern = value;
                }
            }

            public ProjectInfo(string Project_Name, string Project_Phase, ProjectPrintPattern Print_Name_Pattern)
            {
                this._projetName = Project_Name;
                this._projetPhase = Project_Phase;
                this._printPattern = Print_Name_Pattern;

            }
        }
        public class ProjectPrintPattern
        {
            //图号规则
            //缩写+图册+张

            private string _printName;
            private string _printChapter;
            public string PrintName
            {
                get
                {
                    return this._printName;
                }
                set
                {
                    this._printName = value;
                }
            }
            public string PrintChapter
            {
                get
                {
                    return this._printChapter;
                }
                set
                {
                    this._printChapter = value;
                }
            }

            public ProjectPrintPattern(string projetShortName, string projectChapter)
            {
                this._printName = projetShortName;
                this._printChapter = projectChapter;
            }
        }

        //自定义站点类型
        public class KeyPoint:IComparable
        {
            private string _location = string.Empty;
            private string _name = string.Empty;
            private string _type = string.Empty;
            private int _distance;

            public string location
            {
                get
                {
                    return this._location;
                }
                set
                {
                    this._location = value;
                }
            }

            public string name
            {
                get
                {
                    return this._name;
                }
                set
                {
                    this._name = value;
                }
            }
            public string type
            {
                get
                {
                    return this._type;
                }
                set
                {
                    this._type = value;
                }
            }
            public int distance
            {
                get
                {
                    return this._distance;
                }
                set
                {
                    this._distance = value;
                }
            }
            public KeyPoint(string StationLocation, string StationName, string StationType, int Distance)
            {
                _location = StationLocation;
                _name = StationName;
                _type = StationType;
                _distance = Distance;
            }

            int IComparable.CompareTo(object obj)
            {
                KeyPoint temp = (KeyPoint)obj;
                return this.distance.CompareTo(temp.distance);
            }

        }

        //重写为站点
        public class StationPoint : KeyPoint
        {
            public StationPoint(string StationLocation, string StationName, string StationType,int Distance) : base(StationLocation, StationName, StationType, Distance)
            {

            }
        }

        //重写为设备
        public class EquipePoint : KeyPoint
        {
            public EquipePoint(string EquipeLocation, string EquipeName, string EquipeType, int Distance) : base(EquipeLocation, EquipeName, EquipeType, Distance)
            {

            }
        }

        public class LineType
        {
            private string _short;//缩写
            private string _name; //全称
            private string _fournisseurName;//设备商名称
            private double _lineType;//线型（是否铠装）
            private string _lineXin;//芯数
            private double _minDis;//最短距离
            private double _maxDis;//最长距离


            public string name
            {
                get
                {
                    return this._name;
                }
                set
                {
                    this._name = value;
                }
            }
            public string shortfor
            {
                get
                {
                    return this._short;
                }
                set
                {
                    this._short = value;
                }
            }
            public string fournisseurName
            {
                get
                {
                    return this._fournisseurName;
                }
                set
                {
                    this._fournisseurName = value;
                }
            }
            public double lineType
            {
                get
                {
                    return this._lineType;
                }
                set
                {
                    this._lineType = value;
                }
            }
            public string lineXin
            {
                get
                {
                    return this._lineXin;
                }
                set
                {
                    this._lineXin = value;
                }
            }
            public double minLength
            {
                get
                {
                    return this._minDis;
                }
                set
                {
                    this._minDis = value;
                }
            }
            public double maxLength
            {
                get
                {
                    return this._maxDis;
                }
                set
                {
                    this._maxDis = value;
                }
            }

            public LineType() { }
            public LineType(string ShortName, string LineName, string FournisseurName, double LineType, string LineXin, double MinDistance, double MaxDistance)
            {
                _short = ShortName;
                _name = LineName;
                _fournisseurName = FournisseurName;
                _lineType = LineType;
                _lineXin = LineXin;
                _minDis = MinDistance;
                _maxDis = MaxDistance;
            }
        }

        public class LineInfor : LineType
        {
            private double _lineLength;

            public double lineLength
            {
                get
                {
                    return this._lineLength;
                }
                set
                {
                    this._lineLength = value;
                }
            }

            public LineInfor() : base() { }

            public LineInfor(string ShortName, string LineName, string FournisseurName, double LineType, string LineXin, double MinDistance, double MaxDistance, double LengthLine) : base(ShortName, LineName, FournisseurName, LineType, LineXin, MinDistance, MaxDistance)
            {
                _lineLength = LengthLine;
            }
        }

        public class ConnectionAndLine
        {
            private StationPoint _stationPoint;
            private EquipePoint _equipePoint;
            private LineInfor _lineInfor;
            
            public StationPoint station
            {
                get
                {
                    return this._stationPoint;

                }
                set
                {
                    this._stationPoint = value;
                }
            }

            public EquipePoint equipe
            {
                get
                {
                    return this._equipePoint;
                }
                set
                {
                    this._equipePoint = value;
                }
            }
            public LineInfor line
            {
                get
                {
                    return this._lineInfor;
                }
                set
                {
                    this._lineInfor = value;
                }
            }

            public ConnectionAndLine(StationPoint StationPointInfor, EquipePoint EquipePointInfor, LineInfor ConnectionLineinfor)
            {
                this._equipePoint = EquipePointInfor;
                this._stationPoint = StationPointInfor;
                this._lineInfor = ConnectionLineinfor;
            }
        }
    }
}
