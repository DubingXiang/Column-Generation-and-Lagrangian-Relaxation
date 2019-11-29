using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Threading.Tasks;


namespace Crew_Planning
{
    /// <summary>
    /// 运行线
    /// </summary>
    public class Line
    {
        private int iD;
        public int ID
        {
            get { return iD; }
            set { iD = value; }
        }
        private int lineID;
        public int LineID
        {
            get { return lineID; }
            set { lineID = value; }
        }
        private string trainCode;
        public string TrainCode
        {
            get { return trainCode; }
            set { trainCode = value; }
        }
        private int rountingID;
        public int RountingID
        {
            get { return rountingID; }
            set { rountingID = value; }
             
        }
        private string depStaCode;
        public string DepStaCode
        {
            get { return depStaCode; }
            set { depStaCode = value; }
        }
        private string arrStaCode;
        public string ArrStaCode
        {
            get { return arrStaCode; }
            set { arrStaCode = value; }
        }
        private int depTimeCode;
        public int DepTimeCode
        {
            get { return depTimeCode; }
            set { depTimeCode = value; }
        }
        private int arrTimeCode;
        public int ArrTimeCode
        {
            get { return arrTimeCode; }
            set { arrTimeCode = value; }
        }
        private double lagMultiplier;//拉格朗日乘子
        public double LagMultiplier
        {
            get { return lagMultiplier; }
            set { lagMultiplier = value; }
        }
        private int numSelected;//此弧是被选中的次数
        public int NumSelected
        {
            get { return numSelected; }
            set { numSelected = value; }
        }
        public bool TSSContain;
    }
    /// <summary>
    /// 车站
    /// </summary>
    public class Station
    {
        private int iD;
        public int ID
        {
            get { return iD; }
            set { iD = value; }
        }
        private string staCode;
        public string StaCode
        {
            get { return staCode; }
            set { staCode = value; }
        }
        private int reMainCrew;//车站是否具有夜间留宿乘务组能力
        public int ReMainCrew
        {
            get { return reMainCrew; }
            set { reMainCrew = value; }
        }
    }
    /// <summary>
    /// 点
    /// </summary>
    public class T_S_Node
    {
        private int iD;
        public int ID
        {
            get { return iD; }
            set { iD = value; }
        }
        private int lineID;
        public int LineID
        {
            get { return lineID; }
            set { lineID = value; }
        }
        private string trainCode;
        public string TrainCode
        {
            get { return trainCode; }
            set { trainCode = value; }
        }
        private int rountingID;
        public int RountingID
        {
            get { return rountingID; }
            set { rountingID = value; }

        }
        private string staCode;
        public string StaCode
        {
            get { return staCode; }
            set { staCode = value; }
        }
        private int timeCode;
        public int TimeCode
        {
            get { return timeCode; }
            set { timeCode = value; }
        }
        private int pointType;
        /// <summary>
        /// 1-始发点; 2-终到点
        /// </summary>
        public int PointType
        {
            get { return pointType; }
            set { pointType = value; }
        }

        public List<T_S_Arc> Out_T_S_ArcList = new List<T_S_Arc>();

        public List<T_S_S_Node> StateNodeSet = new List<T_S_S_Node>();

    }
    public class T_S_Node_AscByTime : IComparer<T_S_Node> {
        public static T_S_Node_AscByTime pointAsc = new T_S_Node_AscByTime();
        public int Compare(T_S_Node a, T_S_Node b) {
            return a.TimeCode.CompareTo(b.TimeCode);
        }
    }


    public class T_S_S_Node
    {
        private int iD;
        public int ID
        {
            get { return iD; }
            set { iD = value; }
        }
        private int lineID;
        public int LineID
        {
            get { return lineID; }
            set { lineID = value; }
        }
        private string trainCode;
        public string TrainCode
        {
            get { return trainCode; }
            set { trainCode = value; }
        }
        private int rountingID;
        public int RountingID
        {
            get { return rountingID; }
            set { rountingID = value; }

        }
        private string staCode;
        public string StaCode
        {
            get { return staCode; }
            set { staCode = value; }
        }
        private int timeCode;
        public int TimeCode
        {
            get { return timeCode; }
            set { timeCode = value; }
        }
        private int pointType;

        /// <summary>
        ///与时空网络中不同，代表着此点与前继节点组成的弧的类型
        /// 1——运行弧；
        /// 21-日间连接弧，
        /// 22-换乘连接弧，
        /// 23-间休连接弧，
        /// 24-外段驻班休连接弧；
        /// 31-虚拟出乘弧，
        /// 32-虚拟退乘弧，
        /// 33-虚拟停驻弧
        /// </summary>
        public int PointType
        {
            get { return pointType; }
            set { pointType = value; }
        }
        private int superPointID;//对应的物理超节点，时空状态网中的一个点即为时空网中物理点的一个状态
        public int SuperPointID
        {
            get { return superPointID; }
            set { superPointID = value; }
        }
        private int prevSuperID;//前继点的物理超节点
        public int PrevSuperID
        {
            get { return prevSuperID; }
            set { prevSuperID = value; }
        }
        public string OStation;//此点所在交路的始发车站
        public List<int> PassLine = new List<int>();//此点所读竞得运行线
        //点的状态由累计接续时间、累计驾驶时间、累计值乘时间三项组成。并时刻记录前继节点代号
        private int cumuConnTime;
        public int CumuConnTime
        {
            get { return cumuConnTime; }
            set { cumuConnTime = value; }
        }
        private int cumuDriveTime;
        public int CumuDriveTime
        {
            get { return cumuDriveTime; }
            set { cumuDriveTime = value; }
        }
        private int cumuDayCrewTime;
        public int CumuDayCrewTime
        {
            get { return cumuDayCrewTime; }
            set { cumuDayCrewTime = value; }
        }
        private int cumuLunch;//0-没吃午饭；1-正在吃过午饭；2-已经吃完午饭
        public int CumuLunch
        {
            get { return cumuLunch; }
            set { cumuLunch = value; }
        }
        private int cumuDinner;//0-没吃晚饭；1-正在吃过晚饭；2-已经吃完晚饭
        public int CumuDinner
        {
            get { return cumuDinner; }
            set { cumuDinner = value; }
        }
        private int prevID;
        public int PrevID
        {
            get { return prevID; }
            set { prevID = value; }
        }
        
        private int relax;        
        /// <summary>
        /// 根据当前点的状态，判定其的休息状态
        /// 0-不需要休息；
        /// 1-需要休息；
        /// 2-可休可不休（在最小休息时间和最大休息时间之间） 
        /// </summary>
        public int Relax
        {
            get { return relax; }
            set { relax = value; }
        }
        
        public int PervSuperPointID;
        public int PervCumuConnTime;
        public int PervCumuDriveTime;
        public int PervCumuDayCrewTime;
        public int PervCumuLunch;
        public int PervCumuDinner;
        public int PervRelax;
        public int PerPointType;
        public int Isfeasible;//此点是否可行

        /*********db**********/
       
        public Resource Resources = new Resource(1);

        public T_S_S_Node PrevNode = null;
        public T_S_Node SuperPoint = null;
        public T_S_Node PrevSuperPoint = null;

        public void Init_db(int id, int prevID,
            int lineID, int routingID, string trainCode, 
            string station, int timeCode,
            int pointType, string OStation,
            int superID, int prevSuperID,
            T_S_S_Node prevNode,
            T_S_Node superPoint, T_S_Node prevSuperPoint) {

            this.iD = id;
            this.prevID = prevID;

            this.lineID = lineID;
            this.rountingID = routingID;
            this.trainCode = trainCode;

            this.staCode = station;
            this.timeCode = timeCode;

            this.pointType = pointType;
            this.OStation = OStation;

            this.superPointID = superID;
            this.prevSuperID = prevSuperID;

            this.PrevNode = prevNode;
            this.SuperPoint = superPoint;
            this.PrevSuperPoint = prevSuperPoint;
        }
        
		public List<T_S_S_Arc> Out_T_S_S_ArcList = new List<T_S_S_Arc>();
        public List<T_S_S_Node> Out_T_S_S_NodeList = new List<T_S_S_Node>();

		private double price;//代表当前点与其前继节点组成弧的时间费用
        public double Price
        {
            get { return price; }
            set { price = value; }
        }
		public double PenaltyMealViolate = 0;
		
		
      
    }
    public class T_S_S_Node_AscByTime : IComparer<T_S_S_Node>
    {
        public static T_S_S_Node_AscByTime nodeAsc = new T_S_S_Node_AscByTime();
        public int Compare(T_S_S_Node a, T_S_S_Node b) {
            return a.TimeCode.CompareTo(b.TimeCode);
        }
    }
    public class Resource
    {
        private int driveTime_accumu;
        public int DriveTime_accumu
        {
            get { return driveTime_accumu; }
        }        
        public void SetDriveTime(int driveTime) {
            driveTime_accumu = driveTime;
        }
        //private int connTime_accumu;
        //public int ConnTime_accumu
        //{
        //    get { return connTime_accumu; }
        //}
        private int dayCrewTime_accumu;
        public int DayCrewTime_accumu
        {
            get { return dayCrewTime_accumu; }
        }
        public void SetDayCrewTime(int dayCrewTime) {
            dayCrewTime_accumu = dayCrewTime;
        }

        private int lunchStatus;
        public int LunchStatus
        {
            get { return lunchStatus; }
        }
        public void SetLunchStatus(int lunch) {
            lunchStatus = lunch;
        }
        private int dinnerStatus;
        public int DinnerStatus
        {
            get { return dinnerStatus; }
        }
        public void SetDinnerStatus(int dinner) {
            dinnerStatus = dinner;
        }
        //public void SetTimeResource(int driveTime,int dayCrewTime) {
        //    driveTime_accumu = driveTime;            
        //    dayCrewTime_accumu = dayCrewTime;
        //}
        //public void SetMealStatus(int lunch, int dinner) {
        //    lunchStatus = lunch;
        //    dinnerStatus = dinner;
        //}
        public int[] LinesVisitedArray;

        public Resource(int lineNum, int driveTime=0, int dayCrewTime=0, int lunch=0, int dinner=0) {
            if (lineNum <= 0) {
                throw new ArgumentOutOfRangeException("lineNum <= 0,检查是否未读入了Timetable文件，导致LineList未初始化");
            }

            driveTime_accumu = driveTime;
            dayCrewTime_accumu = dayCrewTime;
            lunchStatus = lunch;
            dinnerStatus = dinner;
            LinesVisitedArray = new int[lineNum];
        }
        

    }

    /// <summary>
    /// 弧
    /// </summary>
    public class T_S_Arc
    {
        private int iD;
        public int ID
        {
            get { return iD; }
            set { iD = value; }
        }
        private int lineID;
        public int LineID
        {
            get { return lineID; }
            set { lineID = value; }
        }
        private int rountingID;
        public int RountingID
        {
            get { return rountingID; }
            set { rountingID = value; }
        }
        private string trainCode;
        public string TrainCode
        {
            get { return trainCode; }
            set { trainCode = value; }
        }
        private int startPointID;
        public int StartPointID
        {
            get { return startPointID; }
            set { startPointID = value; }
        }
        private int endPointID;
        public int EndPointID
        {
            get { return endPointID; }
            set { endPointID = value; }
        }
        private string startStaCode;
        public string StartStaCode
        {
            get { return startStaCode; }
            set { startStaCode = value; }
        }
        private string endStaCode;
        public string EndStaCode
        {
            get { return endStaCode; }
            set { endStaCode = value; }
        }
        private int startTimeCode;
        public int StartTimeCode
        {
            get { return startTimeCode; }
            set { startTimeCode = value; }
        }
        private int endTimeCode;
        public int EndTimeCode
        {
            get { return endTimeCode; }
            set { endTimeCode = value; }
        }
        private int arcType;
        public int ArcType
        {
            get { return arcType; }
            set { arcType = value; }
        }
        public int BestNs;
        public int CurNs;
        
        /*******db********/
        public T_S_Node StartPoint = null;
        public T_S_Node EndPoint = null;

        private int len = 0;
        public int Len {
            get { return len; }
        }

        public void Init_db(T_S_Node startPoint_, T_S_Node endPoint_,
            int id_, int lineID_, int routingID_, string trainCode_,
            int startPointID_, int endPointID_,
            string startStation_, string endStation_,
            int startTime_, int endTime_,
            int arcType_) {

            StartPoint = startPoint_;
            EndPoint = endPoint_;

            iD = id_;
            lineID = lineID_;
            rountingID = routingID_;
            trainCode = trainCode_;

            startPointID = startPointID_;
            endPointID = endPointID_;

            startStaCode = startStation_;
            endStaCode = endStation_;

            startTimeCode = startTime_;
            endTimeCode = endTime_;

            arcType = arcType_;

            len = endTimeCode - startTimeCode;
        }
        
    }
    public class T_S_S_Arc
    {
        private int iD;
        public int ID
        {
            get { return iD; }
            set { iD = value; }
        }
        private int lineID;
        public int LineID
        {
            get { return lineID; }
            set { lineID = value; }
        } 
        private string trainCode;
        public string TrainCode
        {
            get { return trainCode; }
            set { trainCode = value; }
        }
        private int rountingID;
        public int RountingID
        {
            get { return rountingID; }
            set { rountingID = value; }

        }
        private int startPointID;
        public int StartPointID
        {
            get { return startPointID; }
            set { startPointID = value; }
        }
        private int endPointID;
        public int EndPointID
        {
            get { return endPointID; }
            set { endPointID = value; }
        }
        private int startSuperPointID;
        public int StartSuperPointID
        {
            get { return startSuperPointID; }
            set { startSuperPointID = value; }
        }
        private int endSuperPointID;
        public int EndSuperPointID
        {
            get { return endSuperPointID; }
            set { endSuperPointID = value; }
        }
      
        private string startStaCode;
        public string StartStaCode
        {
            get { return startStaCode; }
            set { startStaCode = value; }
        }
        private string endStaCode;
        public string EndStaCode
        {
            get { return endStaCode; }
            set { endStaCode = value; }
        }
        private int startTimeCode;
        public int StartTimeCode
        {
            get { return startTimeCode; }
            set { startTimeCode = value; }
        }
        private int endTimeCode;
        public int EndTimeCode
        {
            get { return endTimeCode; }
            set { endTimeCode = value; }
        }
        private int startCumuConnTime;
        public int StartCumuConnTime
        {
            get { return startCumuConnTime; }
            set { startCumuConnTime = value; }
        }
        private int endCumuConnTime;
        public int EndCumuConnTime
        {
            get { return endCumuConnTime; }
            set { endCumuConnTime = value; }
        }
        private int startCumuDriveTime;
        public int StartCumuDriveTime
        {
            get { return startCumuDriveTime; }
            set { startCumuDriveTime = value; }
        }
        private int endCumuDriveTime;
        public int EndCumuDriveTime
        {
            get { return endCumuDriveTime; }
            set { endCumuDriveTime = value; }
        }
        private int startCumuCrewTime;
        public int StartCumuCrewTime
        {
            get { return startCumuCrewTime; }
            set { startCumuCrewTime = value; }
        }
        private int endCumuCrewTime;
        public int EndCumuCrewTime
        {
            get { return endCumuCrewTime; }
            set { endCumuCrewTime = value; }
        }
        private int startCumuLunch;
        /// <summary>
        /// 0-没吃午饭；
        /// 1-正在吃过午饭；
        /// 2-已经吃完午饭
        /// </summary>
        public int StartCumuLunch
        {
            get { return startCumuLunch; }
            set { startCumuLunch = value; }
        }
        private int endCumuLunch;//0-没吃午饭；1-正在吃过午饭；2-已经吃完午饭
        /// <summary>
        /// 0-没吃午饭；
        /// 1-正在吃过午饭；
        /// 2-已经吃完午饭
        /// </summary>
        public int EndCumuLunch
        {
            get { return endCumuLunch; }
            set { endCumuLunch = value; }
        }
        private int startCumuDinner;
        /// <summary>
        /// 0-没吃晚饭；
        /// 1-正在吃过晚饭；
        /// 2-已经吃完晚饭
        /// </summary>
        public int CumuDinner
        {
            get { return startCumuDinner; }
            set { startCumuDinner = value; }
        }
        private int endCumuDinner;
        /// <summary>
        /// 0-没吃晚饭；
        /// 1-正在吃过晚饭；
        /// 2-已经吃完晚饭
        /// </summary>
        public int EndCumuDinner
        {
            get { return endCumuDinner; }
            set { endCumuDinner = value; }
        }
        private int arcType;
        /// <summary>
        /// 弧的类型
        /// 1——运行弧；
        /// 21-日间连接弧，
        /// 22-换乘连接弧，
        /// 23-外段驻班连接弧，
        /// 24-间休（间休用餐）弧
        /// 31-虚拟出乘弧，
        /// 32-虚拟退乘弧，
        /// 33-虚拟停驻弧
        /// </summary>
        public int ArcType
        {
            get { return arcType; }
            set { arcType = value; }
        }
        private int numSelected;//此弧是被选中的次数
        public int NumSelected
        {
            get { return numSelected; }
            set { numSelected = value; }
        }
        private double lagMultiplier;//拉格朗日乘子
        public double LagMultiplier
        {
            get { return lagMultiplier; }
            set { lagMultiplier = value; }
        }
        private double price;//实际弧长，不再是了 since 20191123
        public void SetArcPrice(double len, double penalty) {
            price = len + penalty;
        }
        public double Price
        {
            get { return price; }
            //set { price = value; }
        }

        private double penaltyMealViolate = 0; //只有是trip之间的弧的该值才可能 > 0
        public void SetPenaltyMealViolate(double penalty) {
            penaltyMealViolate = penalty;
        }
        public double PenaltyMealViolate {
            get { return penaltyMealViolate; }
        }

        private double lagPrice;//弧长+拉格朗日乘子
        public double LagPrice
        {
            get { return lagPrice; }
            set { lagPrice = value; }
        }
        private double heurPrice;//启发式算法中定义的弧长
        public double HeurPrice
        {
            get { return heurPrice; }
            set { heurPrice = value; }
        }
        public int BestNs;


        /********db******/
        public T_S_S_Node StartNode = null;
        public T_S_S_Node EndNode = null;

        //public void Init_db(int id, int startPointID, int endPointID)

    }
    /// <summary>
    /// 乘务基地类
    /// </summary>
    public class CrewBase
    {
        private int iD;
        public int ID
        {
            get { return iD; }
            set { iD = value; }
        }
        private string staCode;
        public string StaCode
        {
            get { return staCode; }
            set { staCode = value; }
        }
        private int phyCrewCapcity;//实设乘务组数量
        public int PhyCrewCapacity
        {
            get { return phyCrewCapcity; }
            set { phyCrewCapcity = value; }
        }
        private int virCrewCapcity;//虚设乘务组数量
        public int VirCrewCapacity
        {
            get { return virCrewCapcity; }
            set { virCrewCapcity = value; }
        }
        private int virStartPointID;//代表乘务基地的在时空网络中虚拟点代号
        public int VirStartPointID
        {
            get { return virStartPointID; }
            set { virStartPointID = value; }
        }
        private int virEndPointID;//代表乘务基地的时空网络中虚拟点代号
        public int VirEndPointID
        {
            get { return virEndPointID; }
            set { virEndPointID = value; }
        }
        private int oIDinTSS;
        public int OIDinTSS
        {
            get { return oIDinTSS; }
            set { oIDinTSS = value; }
        }
        private int dIDinTSS;
        public int DIDinTSS
        {
            get { return dIDinTSS; }
            set { dIDinTSS = value; }
        }
    } 
    public class Crew
    {
        public int ID;
        public double OutCost;
        public double RemainCost;
    }

    public enum TSS_NodeType {
        task = 1,
        day_conn = 21,
        night_conn = 22,
        external = 24,
        vir_on = 31,
        vir_off = 32,
        vir_remain = 33
    }
    public enum TSS_ArcType
    {
        task_arc = 1,
        connect_arc = 2,
        on_duty_arc = 3,
        off_duty_arc = 4
    }
}