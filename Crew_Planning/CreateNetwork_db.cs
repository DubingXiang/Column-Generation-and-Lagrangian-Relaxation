using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Collections;
using System.IO;
using System.Diagnostics;

namespace Crew_Planning
{
    public class CrewRule
    {
        int minDriveTime;
        int maxDriveTime;
        public int MinDriveTime
        {
            get { return minDriveTime; }
        }
        public int MaxDriveTime
        {
            get { return maxDriveTime; }
        }
        public void SetDriveTimeWindow(int minDriveTime_, int maxDriveTime_) {
            minDriveTime = minDriveTime_;
            maxDriveTime = maxDriveTime_;
        }

        //int minTransferTime;
        int minConnTime;
        int maxConnTime;
        public int MinConnTime
        {
            get { return minConnTime; }
        }
        public int MaxConnTime
        {
            get { return maxConnTime; }
        }
        public void SetConnTimeWindow(int minConnTime_, int maxConnTime_) {
            minConnTime = minConnTime_;
            maxConnTime = maxConnTime_;
        }

        //int minRelaxTime;
        //int maxRelaxTime;

        int minDayCrewTime;
        int maxDayCrewTime;
        public int MinDayCrewTime
        {
            get { return minDayCrewTime; }
        }
        public int MaxDayCrewTime
        {
            get { return maxDayCrewTime; }
        }
        public void SetDayCrewTimeWindow(int minDayCrewTime_, int maxDayCrewTime_) {
            minDayCrewTime = minDayCrewTime_;
            maxDayCrewTime = maxDayCrewTime_;
        }


        public CrewRule() {
            minConnTime = -1;
            maxConnTime = -1;
            minDriveTime = -1;
            maxDriveTime = -1;
            minDayCrewTime = -1;
            maxDayCrewTime = -1;
        }
    }

    public class CreateNetWork_db
    {
        private List<Line> lineList;
        public List<Line> LineList
        {
            get { return lineList; }
            set { lineList = value; }
        }
        private List<T_S_Node> t_S_NodeList;
        public List<T_S_Node> T_S_NodeList
        {
            get { return t_S_NodeList; }
            set { t_S_NodeList = value; }
        }

        private List<T_S_Arc> t_S_ArcList;
        public List<T_S_Arc> T_S_ArcList
        {
            get { return t_S_ArcList; }
            set { t_S_ArcList = value; }
        }
        private List<CrewBase> crewBaseList;
        public List<CrewBase> CrewBaseList
        {
            get { return crewBaseList; }
            set { crewBaseList = value; }
        }
        private List<Crew> crewList;
        public List<Crew> CrewList
        {
            get { return crewList; }
            set { crewList = value; }
        }
        private List<Station> stationList;
        public List<Station> StationList
        {
            get { return stationList; }
            set { stationList = value; }
        }
        public List<T_S_S_Node> T_S_S_NodeList;
        private List<T_S_S_Arc> t_S_S_ArcList;
        public List<T_S_S_Arc> T_S_S_ArcList
        {
            get { return t_S_S_ArcList; }
            set { t_S_S_ArcList = value; }
        }

        private List<T_S_Node> base_start_pointList = new List<T_S_Node>();
        private List<T_S_Node> base_end_pointList = new List<T_S_Node>();
        private Dictionary<string, int[]> BaseStationToODIDDict = new Dictionary<string, int[]>();

        public void LoadData_csv(int Days,
            string timetableFile,
            string crewbaseFile,
            string crewFile,
            string stationFile) {

            string f1 = timetableFile;
            string f2 = crewbaseFile;
            string f3 = crewFile;
            string f4 = stationFile;

            int j;
            string[] str = new string[7];
            StreamReader csv = new StreamReader(f1, Encoding.Default);
            string row = csv.ReadLine();
            row = csv.ReadLine();
            //Dictionary<string, bool> StationMap = new Dictionary<string, bool>();
            LineList = new List<Line>();
            T_S_NodeList = new List<T_S_Node>();

            while (row != null) {
                str = row.Split(',');
                Line line = new Line();
                line.ID = Convert.ToInt32(str[0]);
                line.LineID = Convert.ToInt32(str[0]);
                line.TrainCode = Convert.ToString(str[1]);
                line.RountingID = Convert.ToInt32(str[6]);
                line.DepStaCode = Convert.ToString(str[2]);
                line.DepTimeCode = Convert.ToInt32(str[4]);
                line.ArrStaCode = Convert.ToString(str[3]);
                line.ArrTimeCode = Convert.ToInt32(str[5]);
                line.LagMultiplier = 0.0;
                line.NumSelected = 0;
                LineList.Add(line);

                for (j = 0; j < Days; j++) {
                    T_S_Node t_s_node1 = new T_S_Node();
                    t_s_node1.ID = (Convert.ToInt32(str[0]) * 2 - 1);
                    t_s_node1.LineID = Convert.ToInt32(str[0]);
                    t_s_node1.RountingID = Convert.ToInt32(str[6]);
                    t_s_node1.TrainCode = Convert.ToString(str[1]);
                    t_s_node1.StaCode = Convert.ToString(str[2]);
                    t_s_node1.TimeCode = Convert.ToInt32(str[4]) + 1440 * j;
                    t_s_node1.PointType = 1;//1——始发点，2——终到点
                    T_S_NodeList.Add(t_s_node1);

                    T_S_Node t_s_node2 = new T_S_Node();
                    t_s_node2.ID = (Convert.ToInt32(str[0]) * 2);
                    t_s_node2.LineID = Convert.ToInt32(str[0]);
                    t_s_node2.RountingID = Convert.ToInt32(str[6]);
                    t_s_node2.TrainCode = Convert.ToString(str[1]);
                    t_s_node2.StaCode = Convert.ToString(str[3]);
                    t_s_node2.TimeCode = Convert.ToInt32(str[5]) + 1440 * j;
                    t_s_node2.PointType = 2;//1——始发点，2——终到点
                    T_S_NodeList.Add(t_s_node2);

                    row = csv.ReadLine();

                }

            }
            csv.Close();

            //CrewBase            
            CrewBaseList = new List<CrewBase>();

            csv = new StreamReader(f2, Encoding.Default);
            row = csv.ReadLine();
            row = csv.ReadLine();
            while (row != null) {
                str = row.Split(',');

                #region 录入虚拟起终点
                T_S_Node t_s_node1 = new T_S_Node();
                t_s_node1.ID = T_S_NodeList.Count + 1;
                t_s_node1.LineID = 0;
                t_s_node1.RountingID = 0;
                t_s_node1.TrainCode = "";
                t_s_node1.StaCode = Convert.ToString(str[1]);
                t_s_node1.TimeCode = 0;
                t_s_node1.PointType = 3;//1——始发点，2——终到点，3——虚拟起点

                T_S_NodeList.Add(t_s_node1);
                base_start_pointList.Add(t_s_node1);

                T_S_Node t_s_node2 = new T_S_Node();
                t_s_node2.ID = T_S_NodeList.Count + 1;
                t_s_node2.LineID = 0;
                t_s_node2.RountingID = 0;
                t_s_node2.TrainCode = "";
                t_s_node2.StaCode = Convert.ToString(str[1]);
                t_s_node2.TimeCode = 0;
                t_s_node2.PointType = 4;//1——始发点，2——终到点，4——虚拟终点

                T_S_NodeList.Add(t_s_node2);
                base_end_pointList.Add(t_s_node2);
                #endregion

                CrewBase crewbase = new CrewBase();
                crewbase.ID = Convert.ToInt32(str[0]);
                crewbase.StaCode = Convert.ToString(str[1]);
                crewbase.PhyCrewCapacity = Convert.ToInt32(str[2]);
                crewbase.VirCrewCapacity = Convert.ToInt32(str[3]);
                crewbase.VirStartPointID = T_S_NodeList.Count - 1;
                crewbase.VirEndPointID = T_S_NodeList.Count;
                //crewbase.VirStartPointID = Convert.ToInt32(Dt.Rows[i]["时空网起点标号"]);
                //crewbase.VirEndPointID = Convert.ToInt32(Dt.Rows[i]["时空网终点标号"]);
                crewbase.OIDinTSS = Convert.ToInt32(str[4]);
                crewbase.DIDinTSS = Convert.ToInt32(str[5]);
                CrewBaseList.Add(crewbase);

                if (!BaseStationToODIDDict.ContainsKey(crewbase.StaCode)) {
                    BaseStationToODIDDict.Add(crewbase.StaCode, new int[2] { crewbase.OIDinTSS, crewbase.DIDinTSS });
                }
                else {
                    throw new Exception("读取基地数据错误！！");
                }

                row = csv.ReadLine();
            }
            csv.Close();

            //Crew
            csv = new StreamReader(f3, Encoding.Default);
            row = csv.ReadLine();

            CrewList = new List<Crew>();
            while (!csv.EndOfStream) {
                row = csv.ReadLine();
                str = row.Split(',');

                Crew crew = new Crew();
                crew.ID = Convert.ToInt32(str[0]);
                crew.OutCost = Convert.ToDouble(str[1]);
                crew.RemainCost = Convert.ToDouble(str[2]);
                CrewList.Add(crew);
            }


            //station            
            StationList = new List<Station>();
            csv = new StreamReader(f4, Encoding.Default);
            row = csv.ReadLine();
            row = csv.ReadLine();
            while (row != null) {
                str = row.Split(',');
                Station station = new Station();

                station.ID = Convert.ToInt32(str[0]);
                station.StaCode = str[1];
                station.ReMainCrew = Convert.ToInt32(str[2]);
                StationList.Add(station);
                row = csv.ReadLine();
            }
        }
        

        /// <summary>
        /// 时空网络的创建,，无需检查，已调试完成
        /// </summary>
        public void CreateT_S_NetWork_db() {
            int arcCount = 1;
            T_S_ArcList = new List<T_S_Arc>();

            int num_cur_arc = T_S_ArcList.Count;
            create_Line_T_S_Arcs(ref arcCount);
            Console.WriteLine("建立T_S_Network, 运行弧数量:{0}, 当前弧总数量:{1}", T_S_ArcList.Count, T_S_ArcList.Count);

            num_cur_arc = T_S_ArcList.Count;
            create_Connect_T_S_Arcs(ref arcCount, BasicTimeRules.MinConnTime, BasicTimeRules.MaxConnTime);
            Console.WriteLine("建立T_S_Network, 连接弧数量:{0}, 当前弧总数量:{1}", T_S_ArcList.Count - num_cur_arc, T_S_ArcList.Count);

            num_cur_arc = T_S_ArcList.Count;
            create_OnAndOff_T_S_Arcs(ref arcCount);
            Console.WriteLine("建立T_S_Network, 出退乘弧（包括虚拟乘务组出乘弧）数量:{0}, 当前弧总数量:{1}",
                T_S_ArcList.Count - num_cur_arc, T_S_ArcList.Count);

        }
        /// <summary>
        /// 构建列车弧 
        /// </summary>
        /// <param name="arcCount"></param>
        private void create_Line_T_S_Arcs(ref int arcCount) {
            for (int i = 0; i < T_S_NodeList.Count - 1; i++) {
                for (int j = i + 1; j < i + 2/*T_S_NodeList.Count*/; j++) {
                    // 因为在创建并添加点的时候，同一Line的出发点和到达点是先后创建添加的，所以内层不用循环
                    T_S_Node startPoint = T_S_NodeList[i];
                    T_S_Node endPoint = T_S_NodeList[j];

                    if (startPoint.LineID == endPoint.LineID
                        && startPoint.PointType == 1 && endPoint.PointType == 2
                        && ((startPoint.TimeCode < 1440 && endPoint.TimeCode < 1440)
                            || (startPoint.TimeCode >= 1440 && endPoint.TimeCode >= 1440))) {
                        T_S_Arc t_s_arc = new T_S_Arc();
                        t_s_arc.Init_db(startPoint, endPoint,
                            arcCount, startPoint.LineID, startPoint.RountingID, startPoint.TrainCode,
                            startPoint.ID, endPoint.ID,
                            startPoint.StaCode, endPoint.StaCode,
                            startPoint.TimeCode, endPoint.TimeCode,
                            1);//1——运行弧

                        T_S_ArcList.Add(t_s_arc);
                        startPoint.Out_T_S_ArcList.Add(t_s_arc);

                        arcCount++;
                    }
                }
            }
        }
        /// <summary>
        /// 创建日间连接弧和夜间连接弧
        /// </summary>
        /// <param name="arcCount"></param>
        /// <param name="minConnTime"></param>
        /// <param name="maxConnTime"></param>
        private void create_Connect_T_S_Arcs(ref int arcCount, int minConnTime, int maxConnTime) {
            Dictionary<string, List<T_S_Node>> StationToPointDict = new Dictionary<string, List<T_S_Node>>();
            foreach (var point in T_S_NodeList) {
                if (point.PointType == 3 || point.PointType == 4) {
                    continue;
                }

                if (!StationToPointDict.ContainsKey(point.StaCode)) {
                    StationToPointDict.Add(point.StaCode, new List<T_S_Node>());
                }
                StationToPointDict[point.StaCode].Add(point);
            }

            foreach (var pointset in StationToPointDict.Values) {
                pointset.Sort(T_S_Node_AscByTime.pointAsc);

                for (int i = 0; i < pointset.Count; i++) {
                    T_S_Node startPoint = pointset[i];
                    if (startPoint.PointType == 1) {
                        continue; // 连接弧的起点必须是到达点
                    }

                    for (int j = i + 1; j < pointset.Count; j++) {
                        // 因为排序了，所以后面的node j必然不早于前面的node i，所以可以直接 i + 1
                        T_S_Node endPoint = pointset[j];
                        if (endPoint.PointType == 2) {
                            continue; // 连接弧的终点必须是出发点
                        }

                        int len = endPoint.TimeCode - startPoint.TimeCode;
                        if (len < minConnTime) {
                            continue;
                        }
                        else if (len > maxConnTime) {
                            break;
                        }
                        else {
                            // 不考虑多天的情况，暂且仅考虑1天
                            // 故在此可以直接创建弧
                            T_S_Arc t_s_arc = new T_S_Arc();
                            t_s_arc.Init_db(startPoint, endPoint,
                                arcCount, startPoint.LineID, startPoint.RountingID, startPoint.TrainCode,
                                startPoint.ID, endPoint.ID,
                                startPoint.StaCode, endPoint.StaCode,
                                startPoint.TimeCode, endPoint.TimeCode,
                                21);//21——日间连接弧

                            T_S_ArcList.Add(t_s_arc);
                            startPoint.Out_T_S_ArcList.Add(t_s_arc);

                            arcCount++;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 构建出退乘弧，包括虚拟乘务组出退乘弧，即停驻弧
        /// </summary>
        /// <param name="arcCount"></param>
        private void create_OnAndOff_T_S_Arcs(ref int arcCount) {
            #region 创建出退乘弧
            // 只需要一次遍历，因为目的就是把位于基地所在站的stationNode与基地起终点相连            
            for (int i = 0; i < T_S_NodeList.Count; i++) {
                T_S_Node stationPoint = T_S_NodeList[i];

                T_S_Node baseStartPoint=null;
                T_S_Node baseEndPoint=null;
                bool isBaseStation = false;
                foreach (var crewbase in CrewBaseList) {
                    if (stationPoint.StaCode == crewbase.StaCode) {
                        isBaseStation = true;
                        baseStartPoint = T_S_NodeList[crewbase.VirStartPointID - 1];
                        baseEndPoint = T_S_NodeList[crewbase.VirEndPointID - 1];
                        break;
                    }
                }
                if (!isBaseStation) {
                    continue;
                }
                if (stationPoint.PointType == 1                    
                    && stationPoint.TimeCode < 1440) {
                    T_S_Arc t_s_arc = new T_S_Arc();
                    t_s_arc.Init_db(baseStartPoint, stationPoint,
                       arcCount, 0, stationPoint.RountingID, stationPoint.TrainCode,
                       baseStartPoint.ID, stationPoint.ID,
                       stationPoint.StaCode, stationPoint.StaCode,
                       0, stationPoint.TimeCode,
                       31);//31——出乘弧

                    T_S_ArcList.Add(t_s_arc);
                    baseStartPoint.Out_T_S_ArcList.Add(t_s_arc);

                    arcCount++;
                }
                if (stationPoint.PointType == 2) {
                    T_S_Arc t_s_arc = new T_S_Arc();
                    t_s_arc.ArcType = 32;//32——退乘弧
                    t_s_arc.Init_db(stationPoint, baseEndPoint,
                       arcCount, 0, 0, stationPoint.TrainCode,
                       stationPoint.ID, baseEndPoint.ID,
                       stationPoint.StaCode, stationPoint.StaCode,
                       stationPoint.TimeCode, 1440,
                       32);//32——退乘弧
                    if (t_s_arc.StartTimeCode > 1440) {
                        t_s_arc.EndTimeCode = 2880;
                    }

                    T_S_ArcList.Add(t_s_arc);
                    stationPoint.Out_T_S_ArcList.Add(t_s_arc);

                    arcCount++;
                }
            }

            // 创建虚拟停驻弧，即虚拟乘务组出乘弧
            foreach (CrewBase crewbase in CrewBaseList) {
                T_S_Node startPoint = T_S_NodeList[crewbase.VirStartPointID-1];
                T_S_Node endPoint = T_S_NodeList[crewbase.VirEndPointID-1];

                T_S_Arc t_s_arc = new T_S_Arc();
                t_s_arc.Init_db(startPoint, endPoint,
                    arcCount, 0, 0, "",
                    crewbase.VirStartPointID, crewbase.VirEndPointID,
                    crewbase.StaCode, crewbase.StaCode,
                    0, 1440, //若是两天会有些变化
                    33);//33——虚拟乘务组出退乘弧

                T_S_ArcList.Add(t_s_arc);
                startPoint.Out_T_S_ArcList.Add(t_s_arc);
                arcCount++;
            }
            #endregion
        }

        public int STLunch;
        public int ETLunch;
        public int STDinner;
        public int ETDinner;
        public void SetMealWindows(int stLunch, int etLunch, int stDinner, int etDinner) {
            this.STLunch = stLunch;
            this.ETLunch = etLunch;
            this.STDinner = stDinner;
            this.ETDinner = etDinner;
        }
        public int minMealTime;
        public int maxMealTime;
        public void SetMealTime(int minMealTime_, int maxMealTime_) {
            minMealTime = minMealTime_;
            maxMealTime = maxMealTime_;
        }

        public int virtualRoutingCost = 0;
        public void SetVirRoutingCost(int virRoutingCost) {
            virtualRoutingCost = virRoutingCost;
        }

        private double arcPenaltyMealViolate = 0;
        public void SetArcPenaltyMealViolate(double penalty) {
            arcPenaltyMealViolate = penalty;
        }
        public double ArcPenaltyMealViolate
        {
            get { return arcPenaltyMealViolate; }
        }

        CrewRule BasicTimeRules = new CrewRule();
        public void SetBasicTimeRules(int mindrivetime, int maxdrivetime,
            int minconntime, int maxconntime,
            int mindaycrewtime, int maxdaycrewtime) {
            BasicTimeRules.SetDriveTimeWindow(mindrivetime, maxdrivetime);
            BasicTimeRules.SetConnTimeWindow(minconntime, maxconntime);
            BasicTimeRules.SetDayCrewTimeWindow(mindaycrewtime, maxdaycrewtime);
        }
        public void ShowBasicTimeRules() {
            string ruleStr = string.Format("DriveTimeWindow[{0},{1}]," +
                "ConnTimeWindow[{2},{3}],DayCrewTimeWindow[{4},{5}]",
                BasicTimeRules.MinDriveTime, BasicTimeRules.MaxDriveTime,
                BasicTimeRules.MinConnTime, BasicTimeRules.MaxConnTime,
                BasicTimeRules.MinDayCrewTime, BasicTimeRules.MaxDayCrewTime);
            Console.WriteLine(ruleStr);
        }

        
        public void CreateT_S_S_Network_db(/*CrewRule crewRule*/) {

            Debug.Assert(BasicTimeRules.MinConnTime != -1);

            List<T_S_S_Node> allStateNodes = new List<T_S_S_Node>();
            Queue<T_S_S_Node> queue = new Queue<T_S_S_Node>();

            initVirStartNode(ref allStateNodes, ref queue);

            //List<T_S_S_Node> tempDNodeList = new List<T_S_S_Node>();
            //Dictionary<int, T_S_S_Node> IDToNodeDict = new Dictionary<int, T_S_S_Node>();
            //foreach (var startNode in allStateNodes) {
            //    IDToNodeDict.Add(startNode.ID, startNode);
            //}

            int debug_count = 100000;
            int debug_iter = 0;

            int nodeCount = CrewBaseList.Count * 2; //其余时空状态点的编号从乘务基地虚拟起终点之后开始
            T_S_S_Node top_node;

            while (queue.Count > 0) {
                //if (--debug_count < 0) {
                //    debug_count = 100000;
                //    debug_iter++;
                    
                //    Console.WriteLine("current Queue.Count is [{0}]", queue.Count);
                //    Console.WriteLine("debug iter is [{0}]", debug_iter);
                //}
                
                top_node = queue.Dequeue();
                //Console.WriteLine(Logger.stateNodeInfoToStr(top_node, OutputMode.console));
                //if (debug_iter >= 10) {
                //    if (debug_count  < 3000) {
                //        Console.WriteLine("cur debug count is [{0}]", debug_count);
                //    }
                //}
                T_S_Node cur_super_point = top_node.SuperPoint;
                List<T_S_Arc> out_T_S_ArcList = cur_super_point.Out_T_S_ArcList;
                for (int i = 0; i < out_T_S_ArcList.Count; i++) {
                    T_S_Arc cur_t_s_arc = out_T_S_ArcList[i];


                    // debug
                    //if (top_node.PassLine.Contains(30)
                    //    && top_node.PassLine.Contains(143)
                    //    //&& top_node.PassLine.Contains(64) 
                    //    //&& top_node.PassLine.Contains(157)
                    //    && cur_t_s_arc.EndTimeCode == 1029
                    //    && cur_t_s_arc.ArcType == 21) {
                    //    Console.WriteLine("top_node.PenaltyMealViolate:" + top_node.PenaltyMealViolate);
                    //}

                    //end debug

                    T_S_S_Node extendStateNode = new T_S_S_Node();
                    extendStateNode.Resources = new Resource(lineList.Count);

                    if (!FeasibleDriveTime(top_node, cur_t_s_arc, ref extendStateNode.Resources)) {
                        continue;
                    }
                    if (!FeasibleDayCrewTime(top_node, cur_t_s_arc, ref extendStateNode.Resources)) {
                        continue;
                    }
                    // 交路退乘点必须与出乘点相同 （必须回基地的约束）
                    if (cur_t_s_arc.ArcType == 32
                        && cur_t_s_arc.EndStaCode != top_node.OStation) {
                        continue;
                    }
                    // 用餐软约束
                    // 必须是非运行弧，才可能可以用餐
                    if (cur_t_s_arc.ArcType != 1) {
                        handleMealConstraint(top_node, cur_t_s_arc, ref extendStateNode);
                        // NOTE：可以处理一下，若当前弧用餐成功，则标记为用餐弧
                    }
                    else {
                        extendStateNode.Resources.SetLunchStatus(top_node.Resources.LunchStatus);
                        extendStateNode.Resources.SetDinnerStatus(top_node.Resources.DinnerStatus);
                    }

                    // extendStartNode基本属性赋值
                    extendStateNode.Init_db(nodeCount, top_node.ID,
                        cur_t_s_arc.LineID, cur_t_s_arc.RountingID, cur_t_s_arc.TrainCode,
                        cur_t_s_arc.EndStaCode, cur_t_s_arc.EndTimeCode,
                        cur_t_s_arc.ArcType, top_node.OStation,
                        cur_t_s_arc.EndPointID, cur_super_point.ID,
                        top_node, cur_t_s_arc.EndPoint, cur_super_point);

                    // 还有其他属性
                    // 1 途径运行线
                    extendStateNode.PassLine.AddRange(top_node.PassLine);
                    if (cur_t_s_arc.ArcType == 1) {
                        extendStateNode.PassLine.Add(cur_t_s_arc.LineID);
                        extendStateNode.Resources.LinesVisitedArray[cur_t_s_arc.LineID-1] += 1;
                    }
                    // 2 起终点的编号
                    if (cur_t_s_arc.ArcType == 31) {
                        extendStateNode.Price = 1440;

                        Debug.Assert(extendStateNode.PenaltyMealViolate == 0);
                    }
                    else if (cur_t_s_arc.ArcType == 32) {
                        extendStateNode.ID = BaseStationToODIDDict[extendStateNode.StaCode][1];
                        extendStateNode.Price = 0;
                        --nodeCount; // 终点有自己的编号，所以此次不用增加nodeCount;
                        Debug.Assert(extendStateNode.PenaltyMealViolate == 0);                        
                    }
                    else if (cur_t_s_arc.ArcType == 33) {
                        extendStateNode.ID = BaseStationToODIDDict[extendStateNode.StaCode][1];
                        extendStateNode.Price = virtualRoutingCost;
                        --nodeCount; // 终点有自己的编号，所以此次不用增加nodeCount;
                        Debug.Assert(extendStateNode.PenaltyMealViolate == 0);                        
                    }
                    else {
                        extendStateNode.Price = cur_t_s_arc.Len;// + extendStateNode.PenaltyMealViolate; 
                        //后续对弧再赋值Penalty
                    }


                    // 比较标号优劣
                    //for (int stateIdx = cur_t_s_arc.EndPoint.StateNodeSet.Count; stateIdx >= 0; stateIdx--) {
                    //}

                    top_node.Out_T_S_S_NodeList.Add(extendStateNode);
                    allStateNodes.Add(extendStateNode);
                    queue.Enqueue(extendStateNode);
                    ++nodeCount;

                    //if (cur_t_s_arc.ArcType != 32 && cur_t_s_arc.ArcType != 33) {
                    //    IDToNodeDict.Add(extendStateNode.ID, extendStateNode);
                    //}
                    //else {
                    //    tempDNodeList.Add(extendStateNode);
                    //}

                    
                } // end of loop "out_T_S_ArcList" 
            } // end of loop "while"

            // 找出所有可行的状态点（即可以到达虚拟终点的状态点）

            #region //基于回溯    
            //T_S_S_NodeList = new List<T_S_S_Node>();
            //foreach (T_S_S_Node note in tempDNodeList) {
            //    int d = note.PrevID;
            //    do {

            //        if (IDToNodeDict.ContainsKey(d)) {
            //            IDToNodeDict[d].Isfeasible = 1;
            //            T_S_S_NodeList.Add(IDToNodeDict[d]);

            //            d = IDToNodeDict[d].PrevID;                        
            //        }
            //    }
            //    while (d != -1);
            //}
            //T_S_S_NodeList.AddRange(tempDNodeList);
            //T_S_S_NodeList.Sort(T_S_S_Node_AscByTime.nodeAsc);
            //int virODNum = CrewBaseList.Count * 2;
            //int newIDCount = virODNum;
            //foreach (var node in T_S_S_NodeList) {
            //    if (node.ID >= virODNum) {
            //        node.ID = newIDCount;
            //        ++newIDCount;
            //    }
            //}
            //foreach (var node in T_S_S_NodeList) {
            //    if (node.PrevID >= 0) {
            //        node.PrevID = node.PrevNode.ID;
            //    }
            //}
            #endregion

            keepAllFeasibleStateNode(allStateNodes);

            // 根据可行状态点建立时空状态弧
            createT_S_S_Arcs();

        } // end of function CreateT_S_S_Network_db

        void initVirStartNode(ref List<T_S_S_Node> stateNodeList, ref Queue<T_S_S_Node> queue_T_S_S_Node) {
            foreach (T_S_Node base_start_point in base_start_pointList) {
                T_S_S_Node t_s_s_node = new T_S_S_Node();
                // 将起点放在编号前列，便于连续编号
                // 这段丑陋，先这样
                //foreach (CrewBase crewbase in CrewBaseList) {
                //    if (base_start_point.StaCode == crewbase.StaCode) {
                //        t_s_s_node.ID = crewbase.OIDinTSS;
                //        break;
                //    }
                //}
                t_s_s_node.ID = BaseStationToODIDDict[base_start_point.StaCode][0];

                t_s_s_node.Init_db(t_s_s_node.ID, -1,
                    -1, 0, base_start_point.TrainCode,
                    base_start_point.StaCode, base_start_point.TimeCode,
                    0, base_start_point.StaCode,
                    base_start_point.ID, -1,
                    null, base_start_point, null);

                t_s_s_node.Resources = new Resource(LineList.Count);

                t_s_s_node.Price = 0;

                stateNodeList.Add(t_s_s_node);
                queue_T_S_S_Node.Enqueue(t_s_s_node);
            }
        }

        // 处理用餐约束
        void handleMealConstraint(T_S_S_Node curStateNode, T_S_Arc arc, ref T_S_S_Node extendStateNode) {

            if (withinMealWindow(arc, STLunch, ETLunch, minMealTime)) {
                if (/*curStateNode.Resources.LunchStatus < 2 &&*/
                    getRealMealSlot(arc, STLunch, ETLunch) < minMealTime) {
                    // 还未用过午餐，且当前弧应该用餐但无法用餐
                    // 则施加惩罚
                    extendStateNode.PenaltyMealViolate = arcPenaltyMealViolate;

                    extendStateNode.Resources.SetLunchStatus(curStateNode.Resources.LunchStatus);
                }
                else {
                    extendStateNode.Resources.SetLunchStatus(2);
                }
            }
            else if (withinMealWindow(arc, STDinner, ETDinner, minMealTime)) {
                if (/*curStateNode.Resources.DinnerStatus < 2 &&*/
                    getRealMealSlot(arc, STDinner, ETDinner) < minMealTime) {
                    // 还未用过晚餐，且当前弧应该用餐但无法用餐
                    // 则施加惩罚
                    extendStateNode.PenaltyMealViolate = arcPenaltyMealViolate;

                    extendStateNode.Resources.SetDinnerStatus(curStateNode.Resources.DinnerStatus);
                }
                else {
                    extendStateNode.Resources.SetDinnerStatus(2);
                }
            }
            else {
                extendStateNode.Resources.SetLunchStatus(curStateNode.Resources.LunchStatus);
                extendStateNode.Resources.SetDinnerStatus(curStateNode.Resources.DinnerStatus);
            }
        }
        bool withinMealWindow(T_S_Arc arc, int mealwindow_begin, int mealwindow_end, int minMealSpan) {            
            return arc.EndTimeCode - mealwindow_begin >= minMealSpan
                && mealwindow_end - arc.StartTimeCode >= minMealSpan;
        }
        int getRealMealSlot(T_S_Arc arc, int mealwindow_begin, int mealwindow_end) {
            int slot_end = Math.Min(arc.EndTimeCode, mealwindow_end);
            int slot_begin = Math.Max(arc.StartTimeCode, mealwindow_begin);
            return slot_end - slot_begin;
        }
        // end 处理用餐约束

        // 更新时间属性值，不涉及用餐约束
        bool FeasibleDriveTime(T_S_S_Node curStateNode, T_S_Arc arc, ref Resource extendResource) {
            if (arc.ArcType == 1) {
                //运行弧
                if (!visitedCurLine(curStateNode, arc)) {
                    int driveTime_accumu = curStateNode.Resources.DriveTime_accumu + arc.Len;

                    if (driveTime_accumu <= BasicTimeRules.MaxDriveTime
                        /*&& dayCrewTime_accumu <= BasicTimeRules.MaxDayCrewTime*/) {
                        // NOTE：只检查驾驶时间，不检查dayCrewTime

                        // 若执行该arc代表的Line后，驾驶时间 <= max，则可行
                        extendResource.SetDriveTime(driveTime_accumu);

                    }
                    else { // 若驾驶时间大于最大允许值，则不可extend，cut掉
                        return false;
                    }
                }
            }
            else if (arc.ArcType == 21) {
                //日间连接弧
                int driveTime_accumu = curStateNode.Resources.DriveTime_accumu;

                if (BasicTimeRules.MinDriveTime <= driveTime_accumu
                    && driveTime_accumu < BasicTimeRules.MaxDriveTime) {
                    // 1.若驾驶时间 in [min. max)，则可行，但要判断是否可以间休

                    // NOTE：这里将最小间休时间等于最小接续时间
                    //     那么下面这个判断始终是true，即只要是接续弧，就视为间休，驾驶时间清零
                    if (arc.Len >= BasicTimeRules.MinConnTime) {
                        // 若弧长 >= 最小间休时长，视为在此间休
                        extendResource.SetDriveTime(0);
                    }
                }
                else if (driveTime_accumu < BasicTimeRules.MinDriveTime) {
                    // 2.若驾驶时间 < 最小驾驶时间，则不间休，视为接续
                    extendResource.SetDriveTime(driveTime_accumu + arc.Len);
                }
                else if (driveTime_accumu == BasicTimeRules.MaxDriveTime) {
                    // 3.若驾驶时间 == 必须间休
                    if (arc.Len < BasicTimeRules.MinConnTime) {
                        // 若弧长小于最小间休时长，则不可行
                        return false;
                    }
                    // 否则，间休
                    extendResource.SetDriveTime(0);
                }

            }
            else if (arc.ArcType == 31 || arc.ArcType == 32 || arc.ArcType == 33) {
                //虚拟出乘弧, //虚拟退乘弧, //虚拟停驻弧
                if (arc.ArcType != 32) { //虚拟出乘，虚拟停驻，当前的状态点的驾驶时间都是0
                    Debug.Assert(curStateNode.Resources.DriveTime_accumu == 0);
                    Debug.Assert(curStateNode.Resources.DayCrewTime_accumu == 0);
                }

                extendResource.SetDriveTime(curStateNode.Resources.DriveTime_accumu);

            }
            else {
                Debug.Print("检查驾驶时间约束，出现未定义的T_S_Arc.ArcType:{0}\n", arc.ArcType);
            }

            return true;
        }

        bool FeasibleDayCrewTime(T_S_S_Node curStateNode, T_S_Arc arc, ref Resource extendResource) {
            if (arc.ArcType == 31 || arc.ArcType == 33) {
                return true;
            }

            int dayCrewTime_accumu = curStateNode.Resources.DayCrewTime_accumu;
            if (arc.ArcType != 32) {
                dayCrewTime_accumu += arc.Len;
            }
            
            if (dayCrewTime_accumu > BasicTimeRules.MaxDayCrewTime) {
                // 任何时候，dayCrewTime都不允许超过最大值
                return false;
            }
            else if (arc.ArcType == 32
                && (dayCrewTime_accumu < BasicTimeRules.MinDayCrewTime || dayCrewTime_accumu > BasicTimeRules.MaxDayCrewTime)) {
                // 如果退乘时，dayCrewTime not in dayCrewTimeWindow [min, max]，不可行
                return false;
            }

            extendResource.SetDayCrewTime(dayCrewTime_accumu);

            return true;
        }
        /// <summary>
        /// arc为运行弧时使用
        /// 判断是否当前状态点（表示的部分路径）已经访问过该Line
        /// 避免同一Line被一条路径多次访问
        /// </summary>
        /// <param name="curStateNode"></param>
        /// <param name="arc"></param>
        /// <returns></returns>
        bool visitedCurLine(T_S_S_Node curStateNode, T_S_Arc arc) {
            return curStateNode.Resources.LinesVisitedArray[arc.LineID - 1] > 0;
        }

        // end 更新时间属性值

        void keepAllFeasibleStateNode(List<T_S_S_Node> allStateNodes) {
            T_S_S_NodeList = new List<T_S_S_Node>(allStateNodes);            

            bool existIsolatedNode = false;
            do {
                existIsolatedNode = false;
                for (int i = T_S_S_NodeList.Count - 1; i >= 0; i--) {
                    T_S_S_Node node = T_S_S_NodeList[i];
                    if ((node.PointType == 32 || node.PointType == 33)) {                    
                        continue;
                    }
                    else if (node.Out_T_S_S_NodeList.Count == 0) {
                        //Console.WriteLine(Logger.stateNodeInfoToStr(node, OutputMode.console));

                        node.PrevNode.Out_T_S_S_NodeList.Remove(node);
                        T_S_S_NodeList.RemoveAt(i);
                        existIsolatedNode = true;                        
                        continue;
                    }                    
                }
            } while (existIsolatedNode);

            // 重新编号
            // 按时间排序

            List<int> temp_debug = new List<int>() {2030, 2155, 2278, 2415, 2479, 2657, 2736, 2916 };

            T_S_S_NodeList.Sort(T_S_S_Node_AscByTime.nodeAsc);
            int virODNum = CrewBaseList.Count * 2;
            int newIDCount = virODNum;
            foreach (var node in T_S_S_NodeList) {
                if (node.ID >= virODNum) {
                    node.ID = newIDCount;
                    ++newIDCount;

                    //debug
                    //if (temp_debug.Contains(node.ID)) {
                    //    Console.WriteLine("cur node Price:" + node.Price);
                    //    Console.WriteLine("cur node PenaltyMealViolate:" + node.PenaltyMealViolate);
                    //}

                }
            }
            foreach (var node in T_S_S_NodeList) {
                if (node.PrevID >= 0) {
                    node.PrevID = node.PrevNode.ID;
                }
            }

        }
        
        // 创建时空状态弧
        void createT_S_S_Arcs() {
            T_S_S_ArcList = new List<T_S_S_Arc>();
            for (int i = 0; i < T_S_S_NodeList.Count; i++) {
                // NOTE：i应该从虚拟起点后开始                
                T_S_S_Node curNode = T_S_S_NodeList[i];
                if (curNode.PointType == 0) {
                    continue;
                }
                T_S_S_Node prevNode = curNode.PrevNode;

                T_S_S_Arc t_s_s_arc = new T_S_S_Arc();
                t_s_s_arc.ID = T_S_S_ArcList.Count;
                t_s_s_arc.StartPointID = prevNode.ID;
                t_s_s_arc.EndPointID = curNode.ID;

                t_s_s_arc.StartNode = prevNode;
                t_s_s_arc.EndNode = curNode;


                if (curNode.PointType == 31) {
                    Debug.Assert(prevNode.TimeCode == 0);
                }
                else if (curNode.PointType == 32) {
                    Debug.Assert(curNode.TimeCode == 1440);
                }
                else if (curNode.PointType == 33) {
                    Debug.Assert(prevNode.TimeCode == 0 && curNode.TimeCode == 1440);
                }
                t_s_s_arc.StartTimeCode = prevNode.TimeCode;
                t_s_s_arc.EndTimeCode = curNode.TimeCode;

                t_s_s_arc.NumSelected = 0;
                t_s_s_arc.LineID = curNode.LineID;
                t_s_s_arc.TrainCode = curNode.PointType == 1 ? curNode.TrainCode : "";
                t_s_s_arc.ArcType = curNode.PointType;

                // Price
                t_s_s_arc.SetPenaltyMealViolate(curNode.PenaltyMealViolate);
                t_s_s_arc.SetArcPrice(curNode.Price, curNode.PenaltyMealViolate);
                t_s_s_arc.LagPrice = t_s_s_arc.Price;
                t_s_s_arc.HeurPrice = t_s_s_arc.Price;

                if (t_s_s_arc.ArcType == 1 || t_s_s_arc.ArcType == 33) {
                    t_s_s_arc.LagMultiplier = 0;
                }
                else {
                    t_s_s_arc.LagMultiplier = -1;
                }
                // meal status
                t_s_s_arc.EndCumuLunch = curNode.Resources.LunchStatus;
                t_s_s_arc.EndCumuDinner = curNode.Resources.DinnerStatus;

                T_S_S_ArcList.Add(t_s_s_arc);
            }

            
        }


        public bool AllLineContain() {
            int i = 0, j = 0;
            bool isnotcontain = false;
            for (i = 0; i < LineList.Count; i++) {
                LineList[i].TSSContain = false;
            }
            for (i = 0; i < LineList.Count; i++) {
                for (j = 0; j < T_S_S_ArcList.Count; j++) {
                    if (LineList[i].ID == T_S_S_ArcList[j].LineID) {
                        LineList[i].TSSContain = true;
                    }
                }
            }

            string str = "";
            string savepath = ".\\looseSeg.csv";
            string str1 = "";
            string savepath1 = ".\\looseSeg1.txt";
            string str2 = "";
            string savepath2 = ".\\chosenSeg.txt";
            int numLoose = 0;
            string str3 = "";
            string savepath3 = ".\\dontChosenSeg.txt";
            for (i = 0; i < LineList.Count; i++) {
                if (LineList[i].TSSContain == false) {
                    isnotcontain = true;
                    str += LineList[i].TrainCode + ",";
                    numLoose++;
                    string strTemp1 = "";
                    strTemp1 += LineList[i].ID + ",";
                    strTemp1 += LineList[i].TrainCode + ",";
                    strTemp1 += LineList[i].DepStaCode + ",";
                    strTemp1 += LineList[i].ArrStaCode + ",";
                    strTemp1 += LineList[i].DepTimeCode + ",";
                    strTemp1 += LineList[i].ArrTimeCode + ",";
                    strTemp1 += LineList[i].RountingID + ",";
                    str3 += strTemp1 + '\n';
                    //break;
                }
                else if (LineList[i].TSSContain) {
                    string strTemp = "";
                    strTemp += LineList[i].ID + ",";
                    strTemp += LineList[i].TrainCode + ",";
                    strTemp += LineList[i].DepStaCode + ",";
                    strTemp += LineList[i].ArrStaCode + ",";
                    strTemp += LineList[i].DepTimeCode + ",";
                    strTemp += LineList[i].ArrTimeCode + ",";
                    strTemp += LineList[i].RountingID + ",";
                    str2 += strTemp + '\n';
                }
            }
            str1 += numLoose;
            File.WriteAllText(savepath, str);
            File.WriteAllText(savepath1, str1);
            File.WriteAllText(savepath2, str2);
            File.WriteAllText(savepath3, str3);
            return isnotcontain;
        }

    }


}
