using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Crew_Planning
{
    using System.Diagnostics;

    public struct FixedMealWindow
    {
        public FixedMealWindow(int lunchStart, int lunchEnd, int supperStart, int supperEnd) {
            lunch_start = lunchStart;
            lunch_end = lunchEnd;
            supper_start = supperStart;
            supper_end = supperEnd;
        }
        public int lunch_start;
        public int lunch_end;
        public int supper_start;
        public int supper_end;
    }
    class Summery
    {

        public string crew_schedule_content = "";
        public double gap_with = 0;
        public double gap_without = 0;

        public double opt_lb_with = 0;
        public double opt_ub_with = 0;
        public double opt_lb_without = 0;
        public double opt_ub_without = 0;

        public int iteration_num = 0;
        public int state_node_num = 0;
        public int state_arc_num = 0;
        public int dvar_num = 0;

        public double net_construct_time = 0;
        public double solve_time = 0;


        public Summery(Lagrange lag, CreateNetWork net, double netConstructTime, double solveTime) {
            for (int i = 0; i < lag.YUB.Count; i++) {
                crew_schedule_content += "乘务交路" + (i + 1).ToString() + "为：" + lag.YUB[i] + "\n";
            }

            //opt_lb_with = lag.BestLB;
            //opt_ub_with = lag.BestUB;
            opt_lb_with = lag.BestLB_with_penalty;
            opt_ub_with = lag.BestUB_with_penalty;
            opt_lb_without = lag.BestLB_without_penalty;
            opt_ub_without = lag.BestUB_without_penalty;

            gap_with = (opt_ub_with - opt_lb_with) / opt_ub_with;
            gap_without = (opt_ub_without - opt_lb_without) / opt_ub_without;

            iteration_num = lag.k;
            state_node_num = net.T_S_S_NodeList.Count;
            state_arc_num = net.T_S_S_ArcList.Count;
            dvar_num = state_arc_num * lag.CrewCount;
            net_construct_time = netConstructTime;
            solve_time = solveTime;

        }
        public Summery(Lagrange lag, CreateNetWork_db net, double netConstructTime, double solveTime) {
            for (int i = 0; i < lag.YUB.Count; i++) {
                crew_schedule_content += "乘务交路" + (i + 1).ToString() + "为：" + lag.YUB[i] + "\n";
            }

            //opt_lb_with = lag.BestLB;
            //opt_ub_with = lag.BestUB;
            opt_lb_with = lag.BestLB_with_penalty;
            opt_ub_with = lag.BestUB_with_penalty;
            opt_lb_without = lag.BestLB_without_penalty;
            opt_ub_without = lag.BestUB_without_penalty;

            gap_with = (opt_ub_with - opt_lb_with) / opt_ub_with;
            gap_without = (opt_ub_without - opt_lb_without) / opt_ub_without;

            iteration_num = lag.k;
            state_node_num = net.T_S_S_NodeList.Count;
            state_arc_num = net.T_S_S_ArcList.Count;
            dvar_num = state_arc_num * lag.CrewCount;
            net_construct_time = netConstructTime;
            solve_time = solveTime;

        }
        public string outputStr() {
            string str = "";
            str += crew_schedule_content;
            str += "OptLB_with_penalty为：" + opt_lb_with.ToString("f2") + "\n";
            str += "OptUB_with_penalty为：" + opt_ub_with.ToString("f2") + "\n";
            str += "OptLB_without_penalty为：" + opt_lb_without.ToString("f2") + "\n";
            str += "OptUB_without_penalty为：" + opt_ub_without.ToString("f2") + "\n";
            str += "迭代次数K为：" + iteration_num.ToString() + "\n";
            str += "Gap_with为：" + gap_with.ToString("f4") + "\n";
            str += "Gap_without为：" + gap_without.ToString("f4") + "\n";
            str += "建网时间为：" + net_construct_time.ToString("f2") + "s" + "\n";
            str += "state arc number: " + state_arc_num + "\n";
            str += "state node number: " + state_node_num + "\n";
            str += "求解时间为：" + solve_time.ToString("f2") + "s" + "\n";
            str += "决策变量个数为：" + dvar_num + "\n";

            return str;
        }

    }
    enum OutputMode
    {
        console = 0,
        file = 1
    }
    class Logger {

        string logPath = "";
        public string LogPath {
            get { return logPath; }
        }
        public void SetLogPath(string path) {
            logPath = path;
        }
        public void ResetLogPath() {
            logPath = "";
        }
        public static void GetScheduleForVisualize(List<Pairing> soln, string caseDir, 
            string caseName, string mealWindow) {
            StringBuilder fileName = new StringBuilder();
            fileName.AppendFormat("scheduleForVisualize_{0}_{1}_{2}_LR.csv", caseName, mealWindow, soln.Count);
            StreamWriter scheduleFile = new StreamWriter(caseDir + fileName.ToString(),
                false, Encoding.UTF8);
            scheduleFile.WriteLine("交路编号,编号,车次,出发时刻,到达时刻,出发车站,到达车站");

            int pathID = 0;
            string str = "";
            foreach (var pairing in soln) {
                str = "";
                ++pathID;
                foreach (var trip in pairing.TripList) {
                    str += pathID + "," + tripToStr(trip);
                }
                scheduleFile.Write(str);
            }

            scheduleFile.Close();
        }

        public static string tripToStr(Line trip) {
            StringBuilder str = new StringBuilder();

            str.AppendFormat("{0},{1},{2},{3},{4},{5}\n",
                trip.LineID,
                trip.TrainCode,
                trip.DepTimeCode,
                trip.ArrTimeCode,
                trip.DepStaCode,
                trip.ArrStaCode);

            return str.ToString();
        }

        public static string stateNodeInfoToStr(T_S_S_Node curStateNode, OutputMode mode) {
            StringBuilder strBuilder = new StringBuilder();
            if (mode == OutputMode.console) {
                strBuilder.AppendLine("----curStateNode's basic info");
                strBuilder.AppendFormat("ID[{0}],NodeType[{1}],OStation[{2}],LineID[{3}],Train[{4}]," +
                    "Station[{5}],Time[{6}],SuperPointID[{7}],SuperPointType[{8}],PrevNodeID[{9}]\n",
                    curStateNode.ID, curStateNode.PointType, curStateNode.OStation,
                    curStateNode.LineID, curStateNode.TrainCode,
                    curStateNode.StaCode, curStateNode.TimeCode,
                    curStateNode.SuperPointID, curStateNode.SuperPoint.PointType,
                    curStateNode.PrevID);
                strBuilder.AppendLine("----curStateNode's state info");
                var resource = curStateNode.Resources;

                strBuilder.AppendFormat("DriveTime[{0}],DayCrewTime[{1}],Lunch[{2}],Dinner[{3}]\n",
                    resource.DriveTime_accumu, resource.DayCrewTime_accumu,
                    resource.LunchStatus, resource.DinnerStatus);


            }
            else if (mode == OutputMode.file) {                
                strBuilder.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},",
                    curStateNode.ID, curStateNode.PointType, curStateNode.OStation,
                    curStateNode.LineID, curStateNode.TrainCode,
                    curStateNode.StaCode, curStateNode.TimeCode,
                    curStateNode.SuperPointID, curStateNode.SuperPoint.PointType,
                    curStateNode.PrevID);                
                var resource = curStateNode.Resources;

                strBuilder.AppendFormat("{0},{1},{2},{3}",
                resource.DriveTime_accumu, resource.DayCrewTime_accumu,
                resource.LunchStatus, resource.DinnerStatus);

            }

            return strBuilder.ToString();
        }

        public static string stateNodePartialPathToStr(T_S_S_Node curStateNode, OutputMode mode) {
            StringBuilder strBuilder = new StringBuilder();
            if (mode == OutputMode.console) {
                strBuilder.AppendLine("----curStateNode's partial path info[Line]:");
            }            

            if (curStateNode.PassLine.Count == 0) {
                strBuilder.Append("[Empty],");
            }
            else {
                strBuilder.Append("[");
                for (int i = 0; i < curStateNode.PassLine.Count - 1; i++) {
                    strBuilder.AppendFormat("{0}-", curStateNode.PassLine[i]);
                }
                strBuilder.AppendFormat("{0}],", curStateNode.PassLine.Last());
            }

            if (mode == OutputMode.console) {
                strBuilder.AppendLine("----curStateNode's partial path info[stateNode]:");
            }
            
            Stack<int> nodePahStack = new Stack<int>();
            var preNode = curStateNode;
            while (preNode != null) {
                nodePahStack.Push(preNode.ID);
                preNode = preNode.PrevNode;

                if (nodePahStack.Count > 10000) {
                    return strBuilder.ToString();
                    throw new StackOverflowException("stateNodePartialPathToStr()死循环");
                }                
            }
            if (nodePahStack.Count == 0) {
                strBuilder.AppendLine("[Empty]");
            }
            else {
                strBuilder.Append("[");
                while (nodePahStack.Count > 1) {
                    strBuilder.AppendFormat("{0}-", nodePahStack.Pop());
                }
                strBuilder.AppendFormat("{0}]", nodePahStack.Pop());
            }            

            return strBuilder.ToString();
        }

    }

}
