using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace Crew_Planning
{
   
    public class Pairing
    {        
        public List<int> Route;
        public List<int> PassLines; //经过的运行线，即真正的交路内容
        public List<Line> TripList;
        public double Price; //< 等于是该条路径的reduced cost，即实际的总成本 - 拉格朗日价格

        private double cost_with_penalty = 0;        
        public double Cost_with_penalty {
            get { return cost_with_penalty; }
        } //CPLEX求解必须用这个价格
        private double cost_without_penalty = 0;
        public double Cost_without_penalty {
            get { return cost_without_penalty; }
        }

        private int totalLen = 0;
        public int TotalLen {
            get { return totalLen; }
        }
        //public void SetTotalLen

        public int[] CoverAaray;

        public Pairing() { }
        public Pairing(List<int> route, double price)
        {
            Route = new List<int>(route);
            Price = price;
            PassLines = new List<int>();
        }
        public void TransRouteToLinesAndCalCost_with_and_without_penalty(Dictionary<string, T_S_S_Arc> arcSet
            /*List<T_S_S_Node> tssnNodeList*/) 
        {
            //if (Route.Contains(2030)
            //        && Route.Contains(2155)
            //        && Route.Contains(2278)
            //        && Route.Contains(2415)
            //        && Route.Contains(2479)
            //        && Route.Contains(2916)) {
            //    int debug = 0;
            //}

            //方法1            
            int count = Route.Count;
            string key = "";
            for (int i = 0; i < count - 1; i++)
            {
                key = Route[i].ToString() + "," + Route[i + 1].ToString();
                if (arcSet.ContainsKey(key) && arcSet[key].ArcType == 1)
                {
                    PassLines.Add(arcSet[key].LineID);
                }
                var arc = arcSet[key];

                cost_with_penalty += arcSet[key].Price;
                

                //if (Cost_CG > 2000 
                //    && PassLines.Contains(30)
                //    && PassLines.Contains(143)
                //    && PassLines.Contains(64)
                //    && PassLines.Contains(157)                    
                //    ) {
                //    int deubg = 0;
                //}
                //debug                
            }

            if (Route.Count > 2) {
                string first = Route[0].ToString() + "," + Route[1].ToString();
                string last = Route[count - 2].ToString() + "," + Route[count - 1].ToString();

                int startTime = arcSet[first].EndTimeCode;
                int endTime = arcSet[last].StartTimeCode;
                totalLen = endTime - startTime;
            }
            cost_without_penalty = 1440 + totalLen;

            ////2th way
            //foreach (var node_id in Route)
            //{
            //    if (tssnNodeList[node_id].LineID > 0 && !PassLines.Contains(tssnNodeList[node_id].LineID))
            //    {
            //        PassLines.Add(tssnNodeList[node_id].LineID);
            //    }
            //}

        }
        public void GenerateCoverArray(int num_task)
        {
            CoverAaray = new int[num_task];
            for (int i = 0; i < num_task; i++)
            {
                CoverAaray[i] = 0;
            }

            foreach (var task_id in PassLines)
            {
                CoverAaray[task_id - 1] = 1;
            }
        }

        public static int cmp(Pairing p1, Pairing p2)
        {
            return p1.Price.CompareTo(p2.Price);
        }
        
        public bool equals(Pairing p)
        {
            if (this.PassLines.Count != p.PassLines.Count)
            {
                return false;
            }

            for (int i = 0; i < this.PassLines.Count; i++)
            {
                if (this.PassLines[i] != p.PassLines[i])
                {
                    return false;
                }
            }

            return true;
        }

        public void fillTripList(List<Line> allLineList) {
            TripList = new List<Line>();
            foreach (var lineID in PassLines) {
                TripList.Add(allLineList[lineID - 1]);
            }
        }
        public List<Line> GetTripList() {
            return TripList;
        }
    }

    public class PairingContentASC : IComparer<Pairing>
    {
        public static PairingContentASC PairingASC = new PairingContentASC();
        public int Compare(Pairing x, Pairing y) {
            if (x.PassLines.Count <= 2 && y.PassLines.Count <= 2) {
                return 0;
            }
            if (x.PassLines.Count <= 2) {
                return -1;
            }
            if (y.PassLines.Count <= 2) {
                return 1;
            }

            return x.PassLines.First() - y.PassLines.First();
                 
        }
    }

    public class Dynamic
    {
        private bool[] s;////表示该节点是否在队列出现过,是ture，不是false；
        public bool[] S
        {
            get { return s; }
            set { s = value; }
        }
        private double[] totalPrice;//最短路的值
        public double[] TotalPrice
        {
            get { return totalPrice; }
            set { totalPrice = value; }
        }
        private int[] perv;//前继节点编号
        public int[] Perv
        {
            get { return perv; }
            set { perv = value; }
        }
        public Dictionary<int, List<T_S_S_Arc>> timeArcs = new  Dictionary<int, List<T_S_S_Arc>>();
        public Dictionary<int, double> prev_cost;
        public List<Pairing> SortedPathSet;
        public int Np;

        public double Max = 999999999999999999;
        public ArrayList QueueNote = new ArrayList();
        public void Initialize(CreateNetWork net)
        {
            int max = 0, i = 0, leng = 0;
            for (i = 0; i < net.T_S_S_NodeList.Count; i++)
            {
                if (net.T_S_S_NodeList[i].ID > max)
                {
                    max = net.T_S_S_NodeList[i].ID;
                }
            }
            leng = max + 1;
            S = new bool[leng];//点的标号，true-此点已被便利，为永久标号；false-未被便利，临时标号
            TotalPrice = new double[leng];//起点到路网中任何一点的最短路  
            Perv = new int[leng];//前继节点代号 

            Np = net.CrewList.Count;
            prev_cost = new Dictionary<int, double>(Np);
            SortedPathSet = new List<Pairing>(Np);
          
        }
        public void Initialize(CreateNetWork_db net) {
            int max = 0, i = 0, leng = 0;
            for (i = 0; i < net.T_S_S_NodeList.Count; i++) {
                if (net.T_S_S_NodeList[i].ID > max) {
                    max = net.T_S_S_NodeList[i].ID;
                }
            }
            leng = max + 1;
            S = new bool[leng];//点的标号，true-此点已被便利，为永久标号；false-未被便利，临时标号
            TotalPrice = new double[leng];//起点到路网中任何一点的最短路  
            Perv = new int[leng];//前继节点代号 

            Np = net.CrewList.Count;
            prev_cost = new Dictionary<int, double>(Np);
            SortedPathSet = new List<Pairing>(Np);

        }
        public void LoadPrice(int o, int d, int crewtype, double outcost, CreateNetWork net, int methodtype)
        {
            int i = 0;
            //对偶问题求解时无需改变虚拟停驻弧的价格
            if (crewtype == 1 && methodtype == 2)//实设乘务组，启发式算法，任务未完成
            {
                #region
                foreach (T_S_S_Arc arc in net.T_S_S_ArcList)
                {
                    if (arc.ArcType == 33)
                    {
                        arc.HeurPrice = Max*Max;//任务未完成，需要安排乘务组出去值乘
                    }
                }
                #endregion
            }
            else if (crewtype == 1 && methodtype == 3)//实设乘务组，启发式算法，任务已完成
            {
                #region
                foreach (T_S_S_Arc arc in net.T_S_S_ArcList)
                {
                    if (arc.ArcType == 33)
                    {
                        arc.HeurPrice = 0;//任务已完成，安排乘务组在乘务基地休息
                    }
                }
                #endregion
            }
            //初始化FDP标号数组
            for (i = 0; i < TotalPrice.Length; i++)
            {
                TotalPrice[i] = Max;
                S[i] = false;
                Perv[i] = -1;//-1表示此点与起点不相连
            }
            //起点初始化
            TotalPrice[o] = 0;
            S[o] = true;
        }
        public void LoadPrice(int o, int d, int crewtype, double outcost, CreateNetWork_db net, int methodtype) {
            int i = 0;
            //对偶问题求解时无需改变虚拟停驻弧的价格
            if (crewtype == 1 && methodtype == 2)//实设乘务组，启发式算法，任务未完成
            {
                #region
                foreach (T_S_S_Arc arc in net.T_S_S_ArcList) {
                    if (arc.ArcType == 33) {
                        arc.HeurPrice = Max * Max;//任务未完成，需要安排乘务组出去值乘
                    }
                }
                #endregion
            }
            else if (crewtype == 1 && methodtype == 3)//实设乘务组，启发式算法，任务已完成
            {
                #region
                foreach (T_S_S_Arc arc in net.T_S_S_ArcList) {
                    if (arc.ArcType == 33) {
                        arc.HeurPrice = 0;//任务已完成，安排乘务组在乘务基地休息
                    }
                }
                #endregion
            }
            //初始化FDP标号数组
            for (i = 0; i < TotalPrice.Length; i++) {
                TotalPrice[i] = Max;
                S[i] = false;
                Perv[i] = -1;//-1表示此点与起点不相连
            }
            //起点初始化
            TotalPrice[o] = 0;
            S[o] = true;
        }

        public void Cal_Dynamic_Speedup(int o, int d, int crewtype, 
            double outcost, double remaincost, 
            ref CreateNetWork net, int methodtype,
            ref List<int> shortpathArr, ref double cost)
        {
            //methodtype:1——拉格朗日松弛算法调用，2——启发式算法调用
            LoadPrice(o, d, crewtype, outcost, net, methodtype);
            int j; int resent = o, v = o;//起点
            shortpathArr.Clear();//每次调用前清零
           
            //拉格朗日对偶问题调用
            int startTime;
            List<T_S_S_Arc> arcs;

            if (methodtype == 1)
            {
                foreach (KeyValuePair<int, List<T_S_S_Arc>> item in timeArcs)
                {
                    startTime = item.Key;
                    arcs = item.Value;
                    foreach (T_S_S_Arc arc in arcs)
                    {
                        resent = arc.StartPointID;
                        j = arc.EndPointID;
                        
                        //运行弧
                        if (arc.ArcType == 1 && TotalPrice[j] > arc.Price - arc.LagMultiplier + TotalPrice[resent])
                        {
                            TotalPrice[j] = arc.Price - arc.LagMultiplier + TotalPrice[resent];
                            Perv[j] = resent;
                        }
                        else if (arc.ArcType == 31 && TotalPrice[j] > arc.Price + outcost + TotalPrice[resent])
                        {
                            TotalPrice[j] = arc.Price + outcost + TotalPrice[resent];
                            Perv[j] = resent;                            
                        }
                        else if (arc.ArcType == 33 && TotalPrice[j] > arc.Price + remaincost + TotalPrice[resent])
                        {
                            TotalPrice[j] = arc.Price + remaincost + TotalPrice[resent];
                            Perv[j] = resent;
                            prev_cost.Add(resent, TotalPrice[j]);
                        }
                        //虚拟退乘弧  
                        else if (arc.ArcType == 32 && TotalPrice[j] > arc.LagPrice + TotalPrice[resent])
                        {
                            TotalPrice[j] = arc.LagPrice + TotalPrice[resent];
                            Perv[j] = resent;
                            prev_cost.Add(resent, TotalPrice[j]);
                        }
                        else if (arc.ArcType == 32 && TotalPrice[resent] < net.virtualRoutingCost) //TotalPrice[j] <= arc.LagPrice + TotalPrice[resent]
                        {
                            prev_cost.Add(resent, TotalPrice[resent] + arc.LagPrice); //记录次短路
                        }
                        //其他弧                            
                        else if (arc.ArcType != 32 && TotalPrice[j] > arc.Price + TotalPrice[resent])
                        {
                            TotalPrice[j] = arc.Price + TotalPrice[resent];
                            Perv[j] = resent;
                        }

                    }
                }
               
            }
            //拉格朗日启发式算法调用
            else if (methodtype == 2)
            {
                foreach (KeyValuePair<int, List<T_S_S_Arc>> item in timeArcs)
                {
                    startTime = item.Key;
                    arcs = item.Value;
                    foreach (T_S_S_Arc arc in arcs)
                    {
                        resent = arc.StartPointID;
                        j = arc.EndPointID;

                        //运行弧和每个乘务组的“会员折扣权重”
                        if (arc.ArcType == 1 && TotalPrice[j] > arc.HeurPrice + TotalPrice[resent])
                        {
                            TotalPrice[j] = arc.HeurPrice + TotalPrice[resent];
                            Perv[j] = resent;
                        }
                        //其他弧
                        else if (TotalPrice[j] > arc.HeurPrice + TotalPrice[resent])
                        {
                            TotalPrice[j] = arc.HeurPrice + TotalPrice[resent];
                            Perv[j] = resent;
                        }
                    }
                }
            }

            if (methodtype == 1)
            {
                prev_cost = prev_cost.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                findTopK(ref this.SortedPathSet, o, d, net.virtualRoutingCost);
                SortedPathSet.Sort(Pairing.cmp);

            }
            if (methodtype == 2 /*|| methodtype == 1*/)
            {
                //单条时空最短路径的路径值
                cost = TotalPrice[d];
                //回溯产生最短路径
                shortpathArr.Clear();
                j = d;//从终点回溯，找到整条路径
                do
                {
                    shortpathArr.Insert(0, j);
                    if (Perv[j] != -1)//防止孤岛的存在
                    {
                        j = Perv[j];
                    }
                    else
                    {
                        j = o;
                    }
                }
                while (j != o);
                shortpathArr.Insert(0, o);
            }
        }
        public void Cal_Dynamic_Speedup(int o, int d, int crewtype,
            double outcost, double remaincost,
            ref CreateNetWork_db net, int methodtype,
            ref List<int> shortpathArr, ref double cost) {
            //methodtype:1——拉格朗日松弛算法调用，2——启发式算法调用
            LoadPrice(o, d, crewtype, outcost, net, methodtype);
            int j; int resent = o, v = o;//起点
            shortpathArr.Clear();//每次调用前清零

            //拉格朗日对偶问题调用
            int startTime;
            List<T_S_S_Arc> arcs;

            if (methodtype == 1) {
                foreach (KeyValuePair<int, List<T_S_S_Arc>> item in timeArcs) {
                    startTime = item.Key;
                    arcs = item.Value;
                    foreach (T_S_S_Arc arc in arcs) {
                        resent = arc.StartPointID;
                        j = arc.EndPointID;

                        //运行弧
                        if (arc.ArcType == 1 && TotalPrice[j] > arc.Price - arc.LagMultiplier + TotalPrice[resent]) {
                            TotalPrice[j] = arc.Price - arc.LagMultiplier + TotalPrice[resent];
                            Perv[j] = resent;
                        }
                        else if (arc.ArcType == 31 && TotalPrice[j] > arc.Price + outcost + TotalPrice[resent]) {
                            TotalPrice[j] = arc.Price + outcost + TotalPrice[resent];
                            Perv[j] = resent;
                        }
                        else if (arc.ArcType == 33 && TotalPrice[j] > arc.Price + remaincost + TotalPrice[resent]) {
                            TotalPrice[j] = arc.Price + remaincost + TotalPrice[resent];
                            Perv[j] = resent;
                            prev_cost.Add(resent, TotalPrice[j]);
                        }
                        //虚拟退乘弧  
                        else if (arc.ArcType == 32 && TotalPrice[j] > arc.LagPrice + TotalPrice[resent]) {
                            TotalPrice[j] = arc.LagPrice + TotalPrice[resent];
                            Perv[j] = resent;
                            prev_cost.Add(resent, TotalPrice[j]);
                        }
                        else if (arc.ArcType == 32 && TotalPrice[resent] < net.virtualRoutingCost) //TotalPrice[j] <= arc.LagPrice + TotalPrice[resent]
                        {
                            prev_cost.Add(resent, TotalPrice[resent] + arc.LagPrice); //记录次短路
                        }
                        //其他弧                            
                        else if (arc.ArcType != 32 && TotalPrice[j] > arc.Price + TotalPrice[resent]) {
                            TotalPrice[j] = arc.Price + TotalPrice[resent];
                            Perv[j] = resent;
                        }

                    }
                }

            }
            //拉格朗日启发式算法调用
            else if (methodtype == 2) {
                foreach (KeyValuePair<int, List<T_S_S_Arc>> item in timeArcs) {
                    startTime = item.Key;
                    arcs = item.Value;
                    foreach (T_S_S_Arc arc in arcs) {
                        resent = arc.StartPointID;
                        j = arc.EndPointID;

                        //运行弧和每个乘务组的“会员折扣权重”
                        if (arc.ArcType == 1 && TotalPrice[j] > arc.HeurPrice + TotalPrice[resent]) {
                            TotalPrice[j] = arc.HeurPrice + TotalPrice[resent];
                            Perv[j] = resent;
                        }
                        //其他弧
                        else if (TotalPrice[j] > arc.HeurPrice + TotalPrice[resent]) {
                            TotalPrice[j] = arc.HeurPrice + TotalPrice[resent];
                            Perv[j] = resent;
                        }
                    }
                }
            }

            if (methodtype == 1) {
                prev_cost = prev_cost.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                findTopK(ref this.SortedPathSet, o, d, net.virtualRoutingCost);
                SortedPathSet.Sort(Pairing.cmp);

            }
            if (methodtype == 2 /*|| methodtype == 1*/) {
                //单条时空最短路径的路径值
                cost = TotalPrice[d];
                //回溯产生最短路径
                shortpathArr.Clear();
                j = d;//从终点回溯，找到整条路径
                do {
                    shortpathArr.Insert(0, j);
                    if (Perv[j] != -1)//防止孤岛的存在
                    {
                        j = Perv[j];
                    }
                    else {
                        j = o;
                    }
                }
                while (j != o);
                shortpathArr.Insert(0, o);
            }
        }

        void findTopK(ref List<Pairing> pathSet, int o, int d, int virRoutingCost)
        {
            int j;
            double cost;
            foreach(var prevNode_cost in this.prev_cost)
            {
                List<int> shortpathArr = new List<int>();
                shortpathArr.Add(d);
                //回溯产生最短路径                
                j = prevNode_cost.Key;//从终点前一个点回溯，找到整条路径               
                while (j != o)
                {
                    shortpathArr.Insert(0, j);
                    if (Perv[j] != -1)//防止孤岛的存在
                    {
                        j = Perv[j];
                    }
                    else
                    {
                        j = o;
                    }
                }                
                shortpathArr.Insert(0, o);

                cost = prevNode_cost.Value > virRoutingCost ? virRoutingCost : prevNode_cost.Value;
                Pairing newPath = new Pairing(shortpathArr, cost);
                pathSet.Add(newPath);
                if (pathSet.Count >= Np)
                {
                    break;
                }
            }
        }
        public void InitDynamic(ref CreateNetWork net)
        {
            Dictionary<int, List<T_S_S_Arc>> timeArcsTemp = new Dictionary<int, List<T_S_S_Arc>>();
            foreach (T_S_S_Arc arc in net.T_S_S_ArcList)
            {
                if(timeArcsTemp.ContainsKey(arc.StartTimeCode))
                    timeArcsTemp[arc.StartTimeCode].Add(arc);
                else
                {
                    List<T_S_S_Arc> list = new List<T_S_S_Arc>();
                    list.Add(arc);
                    timeArcsTemp.Add(arc.StartTimeCode,list);
                }
            }
            timeArcs = timeArcsTemp.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
        }
        public void InitDynamic(ref CreateNetWork_db net) {
            Dictionary<int, List<T_S_S_Arc>> timeArcsTemp = new Dictionary<int, List<T_S_S_Arc>>();
            foreach (T_S_S_Arc arc in net.T_S_S_ArcList) {
                if (timeArcsTemp.ContainsKey(arc.StartTimeCode))
                    timeArcsTemp[arc.StartTimeCode].Add(arc);
                else {
                    List<T_S_S_Arc> list = new List<T_S_S_Arc>();
                    list.Add(arc);
                    timeArcsTemp.Add(arc.StartTimeCode, list);
                }
            }
            timeArcs = timeArcsTemp.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
        }
    }
    public class Lagrange
    {
        public int k;//迭代次数
        public double BestLB;//最佳下界
        public double BestUB;//最佳上界
        public List<double> stepList;//运行线迭代步长
        public double virstep;//虚拟弧迭代步长
        public const double M = 999999.0;
        //public int Ns = 0;
        public int C = 299;
        public int cir;
        //public double Gap;
        private List<double> curLB;//第k次迭代的下界
        public List <double>CurLB
        {
            get { return curLB; }
            set { curLB = value; }
        }
        private List<double> curUB;//第k次迭代的上界
        public List<double> CurUB
        {
            get { return curUB; }
            set { curUB = value; }
        }
        private List<double> curBestLB;//第k次迭代的当前最优下界
        public List<double> CurBestLB
        {
            get { return curBestLB; }
            set { curBestLB = value; }
        }
        private List<double> curBestUB;//第k次迭代的当前最优上界
        public List<double> CurBestUB
        {
            get { return curBestUB; }
            set { curBestUB = value; }
        }
        public double BestT;//总的运行线时间之和，算便乘的
        public int BestNgo=100;//最佳出去的车
        public List<int> OCrew = new List<int>();//数组长度即为初始乘务组数，每一个元素代表时空状态网中车站代号
        public List<int> DCrew = new List<int>();
        public List<int> CrewType = new List<int>();//所对应的乘务组属性。1——实乘务组，0——虚乘务组
        public List<int> OCrewTemp = new List<int>();//数组长度即为初始乘务组数，每一个元素代表时空状态网中车站代号
        public List<int> DCrewTemp = new List<int>();
        public List<int> CrewTypeTemp = new List<int>();//所对应的乘务组属性。1——实乘务组，0——虚乘务组
        public List<double> CrewOutCost = new List<double>();//所对应的乘务组出动成本
        public List<double> CrewRemainCost = new List<double>();//所对应的乘务组停驻成本
        public int CrewCount = 0;
        public List<string> YLB = new List<string>();//第k次迭代的下界解
        public List<string> YUB = new List<string>();//第k次迭代上界的解(可行解) 
        public List<List<int>> YUB_iter = new List<List<int>>(); //迭代过程中的上界解内容        
        

        public List<string> templist = new List<string>();//临时存放解的集合 
        //public SPFA Spfa = new SPFA ();
        //public Dijkstr Dijk = new Dijkstr();
        public Dynamic dyna = new Dynamic();
        public List<int> shortpath = new List<int>();//临时存放最短路的集合

        //4-4
        public List<int> shortpath2 = new List<int>();
        public List<List<int>> OOCrew = new List<List<int>>();
        public List<List<int>> DDCrew = new List<List<int>>();

        //4-15
        public Dictionary<int, List<T_S_S_Arc>> type_arcPair = new Dictionary<int, List<T_S_S_Arc>>();

        //4-30
        public Dictionary<int, List<double>> task_LagMultiplier = new Dictionary<int, List<double>>();        
        public List<double> time_sp = new List<double>();
        Stopwatch spT = new Stopwatch();
        public int FixedCrewNum;

        public readonly double step_size;
        public List<double> stepSize_iter = new List<double>();        
        public List<Pairing> SortedPathSet;

        public List<Pairing> LB_PathSet;

        public List<Pairing> g_LB_PathSet = new List<Pairing>();
        public List<Pairing> g_UB_PathSet = new List<Pairing>();

        Dictionary<string, T_S_S_Arc> ArcSet = new Dictionary<string, T_S_S_Arc>();

        // db
        Dictionary<string, T_S_S_Arc> g_ArcSet = new Dictionary<string, T_S_S_Arc>();//只赋值一次
        Dictionary<int, Line> g_LineSet = new Dictionary<int, Line>();
        public double BestUB_with_penalty;
        public double BestLB_with_penalty;
        public double BestUB_without_penalty;
        public double BestLB_without_penalty;

        public List<double> CurLB_with_penalty = new List<double>();
        //public List<double> CurUB_with_penalty = new List<double>();
        public List<double> CurLB_without_penalty = new List<double>();
        //public List<double> CurUB_without_penalty = new List<double>();
        // end db


        public bool UBStay = false;//可行解是否改善
        public int nubstay = 0;//可行解不在改善的次数              

        public Lagrange()
        {
        }
        public Lagrange(CreateNetWork net, int stepSize)
        {
            Initialize(net);
            this.step_size = stepSize;
        }
        public Lagrange(CreateNetWork_db net, int stepSize) {
            Initialize(net);
            this.step_size = stepSize;
        }

        private void Initialize(CreateNetWork net)//初始化拉格朗日各变量
        {
            int i =0,j = 0;
            //dyna.InitDynamic(ref net);
            YLB.Clear();
            YUB.Clear();
            BestLB = 0-M*M;
            BestUB = M*M;
            FixedCrewNum = net.CrewList.Count;
            this.SortedPathSet = new List<Pairing>(FixedCrewNum);

            CurLB = new List<double>();
            CurUB = new List<double>();
            CurBestLB = new List<double>();
            CurBestUB = new List<double>();
            //录入乘务组数
            int couter0 = 0, couter1 = 0, couter2 = 0, couter3 = 0;
            for (j = 0; j < net.CrewBaseList[0].PhyCrewCapacity; j++)
            {
                if (net.CrewBaseList.Count > 0)
                {
                    if (couter0 < net.CrewBaseList[0].PhyCrewCapacity)
                    {
                        OCrew.Add(net.CrewBaseList[0].OIDinTSS);//起点集合
                        DCrew.Add(net.CrewBaseList[0].DIDinTSS);//终点集合
                        CrewType.Add(1);//乘务组类型集合
                        couter0++;
                    }
                }
                if (net.CrewBaseList.Count > 1)
                {
                    if (couter1 < net.CrewBaseList[1].PhyCrewCapacity)
                    {
                        OCrew.Add(net.CrewBaseList[1].OIDinTSS);//起点集合
                        DCrew.Add(net.CrewBaseList[1].DIDinTSS);//终点集合
                        CrewType.Add(1);//乘务组类型集合
                        couter1++;
                    }
                }
                if (net.CrewBaseList.Count > 2)
                {
                    if (couter2 < net.CrewBaseList[2].PhyCrewCapacity)
                    {
                        OCrew.Add(net.CrewBaseList[2].OIDinTSS);//起点集合
                        DCrew.Add(net.CrewBaseList[2].DIDinTSS);//终点集合
                        CrewType.Add(1);//乘务组类型集合
                        couter2++;
                    }
                }
                if (net.CrewBaseList.Count > 3)
                {
                    if (couter3 < net.CrewBaseList[3].PhyCrewCapacity)
                    {
                        OCrew.Add(net.CrewBaseList[3].OIDinTSS);//起点集合
                        DCrew.Add(net.CrewBaseList[3].DIDinTSS);//终点集合
                        CrewType.Add(1);//乘务组类型集合
                        couter3++;
                    }
                }
            }

            #region MyRegion
            int Obase, Dbase;
            CrewType.Clear();
            foreach (CrewBase crewbase in net.CrewBaseList)
            {
                OCrew = new List<int>();
                DCrew = new List<int>();
                Obase = crewbase.OIDinTSS;
                Dbase = crewbase.DIDinTSS;
                for (i = 0; i < net.CrewList.Count; i++)
                {
                    OCrew.Add(Obase);
                    DCrew.Add(Dbase);
                    CrewType.Add(1);
                }
                OOCrew.Add(OCrew);
                DDCrew.Add(DCrew);
            }

            int arc_type;
            for (i = 0; i < net.T_S_S_ArcList.Count; i++)
            {
                arc_type = net.T_S_S_ArcList[i].ArcType;
                if (!type_arcPair.ContainsKey(arc_type))
                {
                    List<T_S_S_Arc> list = new List<T_S_S_Arc>();
                    type_arcPair.Add(arc_type, list);
                } 
                type_arcPair[arc_type].Add(net.T_S_S_ArcList[i]);  
               
            }
            #endregion

           
            for (i=0;i<net.CrewList.Count;i++)
            {
                CrewOutCost.Add(net.CrewList[i].OutCost);
                CrewRemainCost.Add(net.CrewList[i].RemainCost);
            }
            stepList = new List<double>();
            foreach (Line line in net.LineList)
            {
                double time = (line.ArrTimeCode - line.DepTimeCode) * 1.0;
                stepList.Add(time);

            }
            //初始化前k次迭代的最大停留乘务组数，初始值为0
            CrewCount = OCrew.Count;
            //Ns = OCrewTemp.Count/2;
            for (i = 0; i < CrewOutCost.Count; i++)
            {
                CrewOutCost[i] = 0;
                CrewRemainCost[i] = 0;
            }

            ////4-30
            //for (i = 1; i <= net.LineList.Count; i++)
            //{
            //    task_LagMultiplier.Add(i, new List<double>());
            //    //task_HeuriMultiplier.Add(i, new List<double>());
            //}
        }
        private void Initialize(CreateNetWork_db net)//初始化拉格朗日各变量
        {
            string key = "";
            foreach (var arc in net.T_S_S_ArcList) {
                //arc.NumSelected = 0;
                key = arc.StartPointID.ToString() + "," + arc.EndPointID.ToString();
                g_ArcSet.Add(key, arc);
            }
            foreach (var line in net.LineList) {
                line.NumSelected = 0;                
                g_LineSet.Add(line.LineID, line);
            }


            int i = 0, j = 0;
            //dyna.InitDynamic(ref net);
            YLB.Clear();
            YUB.Clear();
            BestLB = 0 - M * M;
            BestUB = M * M;
            FixedCrewNum = net.CrewList.Count;
            this.SortedPathSet = new List<Pairing>(FixedCrewNum);

            CurLB = new List<double>();
            CurUB = new List<double>();
            CurBestLB = new List<double>();
            CurBestUB = new List<double>();
            //录入乘务组数
            int couter0 = 0, couter1 = 0, couter2 = 0, couter3 = 0;
            for (j = 0; j < net.CrewBaseList[0].PhyCrewCapacity; j++) {
                if (net.CrewBaseList.Count > 0) {
                    if (couter0 < net.CrewBaseList[0].PhyCrewCapacity) {
                        OCrew.Add(net.CrewBaseList[0].OIDinTSS);//起点集合
                        DCrew.Add(net.CrewBaseList[0].DIDinTSS);//终点集合
                        CrewType.Add(1);//乘务组类型集合
                        couter0++;
                    }
                }
                if (net.CrewBaseList.Count > 1) {
                    if (couter1 < net.CrewBaseList[1].PhyCrewCapacity) {
                        OCrew.Add(net.CrewBaseList[1].OIDinTSS);//起点集合
                        DCrew.Add(net.CrewBaseList[1].DIDinTSS);//终点集合
                        CrewType.Add(1);//乘务组类型集合
                        couter1++;
                    }
                }
                if (net.CrewBaseList.Count > 2) {
                    if (couter2 < net.CrewBaseList[2].PhyCrewCapacity) {
                        OCrew.Add(net.CrewBaseList[2].OIDinTSS);//起点集合
                        DCrew.Add(net.CrewBaseList[2].DIDinTSS);//终点集合
                        CrewType.Add(1);//乘务组类型集合
                        couter2++;
                    }
                }
                if (net.CrewBaseList.Count > 3) {
                    if (couter3 < net.CrewBaseList[3].PhyCrewCapacity) {
                        OCrew.Add(net.CrewBaseList[3].OIDinTSS);//起点集合
                        DCrew.Add(net.CrewBaseList[3].DIDinTSS);//终点集合
                        CrewType.Add(1);//乘务组类型集合
                        couter3++;
                    }
                }
            }

            #region MyRegion
            int Obase, Dbase;
            CrewType.Clear();
            foreach (CrewBase crewbase in net.CrewBaseList) {
                OCrew = new List<int>();
                DCrew = new List<int>();
                Obase = crewbase.OIDinTSS;
                Dbase = crewbase.DIDinTSS;
                for (i = 0; i < net.CrewList.Count; i++) {
                    OCrew.Add(Obase);
                    DCrew.Add(Dbase);
                    CrewType.Add(1);
                }
                OOCrew.Add(OCrew);
                DDCrew.Add(DCrew);
            }

            int arc_type;
            for (i = 0; i < net.T_S_S_ArcList.Count; i++) {
                arc_type = net.T_S_S_ArcList[i].ArcType;
                if (!type_arcPair.ContainsKey(arc_type)) {
                    List<T_S_S_Arc> list = new List<T_S_S_Arc>();
                    type_arcPair.Add(arc_type, list);
                }
                type_arcPair[arc_type].Add(net.T_S_S_ArcList[i]);

            }
            #endregion


            for (i = 0; i < net.CrewList.Count; i++) {
                CrewOutCost.Add(net.CrewList[i].OutCost);
                CrewRemainCost.Add(net.CrewList[i].RemainCost);
            }
            stepList = new List<double>();
            foreach (Line line in net.LineList) {
                double time = (line.ArrTimeCode - line.DepTimeCode) * 1.0;
                stepList.Add(time);

            }
            //初始化前k次迭代的最大停留乘务组数，初始值为0
            CrewCount = OCrew.Count;
            //Ns = OCrewTemp.Count/2;
            for (i = 0; i < CrewOutCost.Count; i++) {
                CrewOutCost[i] = 0;
                CrewRemainCost[i] = 0;
            }           
        }
        void ReSetArcSet(CreateNetWork net)
        {
            ArcSet.Clear();
            string key = "";
            for (int i = 0; i < net.T_S_S_ArcList.Count; i++)
            {
                net.T_S_S_ArcList[i].NumSelected = 0;
                key = net.T_S_S_ArcList[i].StartPointID.ToString() + "," + net.T_S_S_ArcList[i].EndPointID.ToString();
                ArcSet.Add(key, net.T_S_S_ArcList[i]);
            }
        }
        void ReSetArcSet(CreateNetWork_db net) {
            ArcSet.Clear();
            string key = "";
            for (int i = 0; i < net.T_S_S_ArcList.Count; i++) {
                net.T_S_S_ArcList[i].NumSelected = 0;
                key = net.T_S_S_ArcList[i].StartPointID.ToString() + "," + net.T_S_S_ArcList[i].EndPointID.ToString();
                ArcSet.Add(key, net.T_S_S_ArcList[i]);
            }
        }

        public void IterationGap(ref CreateNetWork net, double Gap)
        {
            //初始化
            Initialize(net);
            dyna.InitDynamic(ref net);
            Console.WriteLine("Gap:" + Gap);

            //迭代
            while ((BestUB - BestLB) / BestUB >= Gap)//清零
            {
                GeneratLB_SpeedUp(ref BestLB, k, ref  net);
                GeneratUB_SpeedUp(ref BestUB, k, ref net);
                Console.WriteLine("k{0}次迭代: 下界{1}\t上界{2}", k, BestLB, BestUB);
                
                k++;
            }
        }

        void recordMultiplier(CreateNetWork net) 
        {
            foreach (var line in net.LineList)
            {
                task_LagMultiplier[line.LineID].Add(line.LagMultiplier);
                //task_HeuriMultiplier[line.LineID].Add(line.p);
            }
        }
        public void IterationK(ref CreateNetWork net, int K)
        {
            //初始化
            //Initialize(net);
            dyna.InitDynamic(ref net);
            
            //stepSize_iter = new List<double>(K);            

            Stopwatch Time = new Stopwatch();
            
            //recordMultiplier(net);            
            //迭代
            while (k <= K)//清零//changed 4-27 :initial: k<=K
            {
                Time.Restart();
                //GeneratLB(ref BestLB,k, ref  net);
                GeneratLB_SpeedUp(ref BestLB, k, ref  net);

                g_LB_PathSet.InsertRange(g_LB_PathSet.Count , LB_PathSet);

                Time.Stop();
                Console.WriteLine("LB time:{0}", Time.Elapsed.TotalSeconds);

                //recordMultiplier(net); changed 20191026
                //if ((4 <= k && k <= 15) ||
                //    (45 <= k && k <= 65) ||
                //    (84 <= k && k <= K))

                if (k > K)
                {
                    Time.Restart();
                    //GeneratUB(ref BestUB, k, ref net);
                    GeneratUB_SpeedUp(ref BestUB, k, ref net);
                    Time.Stop();
                    Console.WriteLine("UB time:{0}", Time.Elapsed.TotalSeconds);
                    Console.WriteLine("k{0}次迭代: 下界{1}\t上界{2}, gap{3}", k, BestLB, BestUB, (BestUB - BestLB) / BestUB);
                }
                else
                {
                    Console.WriteLine("k{0}次迭代: 下界{1}", k, BestLB);
                }
                k++;
            }                        

            //transfer YUB
            double T = 0;
            double cost = 0;
            double UB_OBJ = 0;
            string sp = "";
            YUB.Clear();
            foreach (var route in YUB_iter)
            {
                sp = "";
                T = 0;
                cost = 0;
                TranslationCSP(route, net, ref sp, ref T, ref cost, 0, 0);
                YUB.Add(sp);
                UB_OBJ += cost;
            }

        }
        public void IterationK(ref CreateNetWork_db net, int K) {
            //初始化
            //Initialize(net);
            dyna.InitDynamic(ref net);

            //stepSize_iter = new List<double>(K);            

            Stopwatch Time = new Stopwatch();

            //recordMultiplier(net);            
            //迭代
            while (k <= K)//清零//changed 4-27 :initial: k<=K
            {
                Time.Restart();
                //GeneratLB(ref BestLB,k, ref  net);
                GeneratLB_SpeedUp(ref BestLB, k, ref net);

                g_LB_PathSet.InsertRange(g_LB_PathSet.Count, LB_PathSet);

                Time.Stop();
                Console.WriteLine("LB time:{0}", Time.Elapsed.TotalSeconds);
                //recordMultiplier(net); changed 20191026
                //if ((4 <= k && k <= 15) ||
                //    (45 <= k && k <= 65) ||
                //    (84 <= k && k <= K))

                if (k > K) {
                    Time.Restart();
                    //GeneratUB(ref BestUB, k, ref net);
                    GeneratUB_SpeedUp(ref BestUB, k, ref net);
                    Time.Stop();
                    Console.WriteLine("UB time:{0}", Time.Elapsed.TotalSeconds);
                    Console.WriteLine("k{0}次迭代: 下界{1}\t上界{2}, gap{3}", k, BestLB, BestUB, (BestUB - BestLB) / BestUB);
                }
                else {
                    Console.WriteLine("k{0}次迭代: 下界:{1}\t下界without_penalty:{2}",
                        k, BestLB, BestLB_without_penalty);
                }
                k++;
            }

            //transfer YUB
            //double T = 0;
            //double cost = 0;
            //double UB_OBJ = 0;
            //string sp = "";
            //YUB.Clear();
            //foreach (var route in YUB_iter) {
            //    sp = "";
            //    T = 0;
            //    cost = 0;
            //    TranslationCSP(route, net, ref sp, ref T, ref cost, 0, 0);
            //    YUB.Add(sp);
            //    UB_OBJ += cost;
            //}// changed 20191128 

        }
        public void IterationUBStay(ref CreateNetWork net, int N)
        {
           
            //初始化
            Initialize(net);
            //迭代
            while (UBStay==false)//清零
            {
                GeneratLB_SpeedUp(ref BestLB, k, ref  net);
                GeneratUB_SpeedUp(ref BestUB, k, ref net);
                if (OCrew.Count > CrewCount)
                {
                    while (OCrew.Count > CrewCount)
                    {
                        OCrew.RemoveAt(OCrew.Count - 1);
                        DCrew.RemoveAt(DCrew.Count - 1);
                        CrewType.RemoveAt(CrewType.Count - 1);
                        CrewOutCost.RemoveAt(CrewOutCost.Count - 1);
                        CrewRemainCost.RemoveAt(CrewRemainCost.Count - 1);
                    }
                }      
                k++;
                if(nubstay==N)
                {
                    UBStay = true;
                }
            }
        }

        //for CG
        public void GenerateLRSoln(ref CreateNetWork net, int numIter)
        {
            IterationK(ref net, numIter);
            ReSetArcSet(net);
            //TransForCG(ref LB_PathSet, ArcSet, net);
            //TransForCG(ref SortedPathSet, ArcSet, net); //changed 20191119
            TransForCG(ref g_LB_PathSet, ArcSet, net); //added 20191026
        }
        public void GenerateLRSoln(ref CreateNetWork_db net, int numIter) {
            int[] coveredLine = new int[net.LineList.Count + 1];
            //int sumWhenAllLinesCovered = 0;
            for (int i = 0; i < coveredLine.Length; i++) {
                coveredLine[i] = 0;
                //sumWhenAllLinesCovered += i;
            }

            IterationK(ref net, numIter);
            //ReSetArcSet(net);

            //TransForCG(ref LB_PathSet, ArcSet, net);
            //TransForCG(ref SortedPathSet, ArcSet, net); //changed 20191119
            //TransForCG(ref g_LB_PathSet, ArcSet, net); //added 20191026
            TransForCG(ref g_LB_PathSet, g_ArcSet, net); //added 20191128

            // 记录第一次迭代被覆盖的line
            foreach (var pairing in g_LB_PathSet) {
                if (pairing.PassLines.Count > 2) {
                    foreach (var line in pairing.PassLines) {
                        coveredLine[line] = 1;                        
                    }                    
                }
            }
            //List<int> uncoveredLines = new List<int>();
            //for (int i = 1; i < coveredLine.Length; i++) {
            //    if (coveredLine[i] == 0) {
            //        uncoveredLines.Add(i);
            //    }
            //}                 

            //int descIter = 1;
            //int curPathNum = g_LB_PathSet.Count;
            //int curCoveredNum = coveredLine.Sum();
            //while(net.LineList.Count > curCoveredNum) {

            //    int curIterNum = numIter * descIter + (int)(numIter * Math.Pow(0.95, descIter++));
            //    IterationK(ref net, curIterNum);
            //    TransForCG(ref g_LB_PathSet, g_ArcSet, net);

            //    for (int i = curPathNum; i < g_LB_PathSet.Count; i++) {
            //        var pairing = g_LB_PathSet[i];
            //        if (pairing.PassLines.Count > 2) {
            //            foreach (var line in pairing.PassLines) {
            //                coveredLine[line] = 1;
            //            }
            //        }
            //    }                
            //    curPathNum = g_LB_PathSet.Count;
            //    curCoveredNum = coveredLine.Sum();
            //}            
        }
        public void TransForCG(ref List<Pairing> sortedPathSet, 
            Dictionary<string, T_S_S_Arc> arcSet, 
            CreateNetWork net)
        {
            foreach (var path in sortedPathSet)
            {
                if (path.PassLines.Count == 0)
                {
                    path.TransRouteToLinesAndCalCost_with_and_without_penalty(arcSet);
                }
                path.GenerateCoverArray(net.LineList.Count);
            }
        }
        public void TransForCG(ref List<Pairing> sortedPathSet,
            Dictionary<string, T_S_S_Arc> arcSet,
            CreateNetWork_db net) {
            
            foreach (var path in sortedPathSet) {
                if (path.PassLines.Count == 0) {
                    path.TransRouteToLinesAndCalCost_with_and_without_penalty(arcSet);
                }
                path.GenerateCoverArray(net.LineList.Count);
                
            }

            //int debug = 0;
        }

        //end for CG

        void init_SortedPathSet()
        {
            SortedPathSet.Clear();
            for (int i = 0; i < FixedCrewNum; i++)
            {
                Pairing stayPath = new Pairing(new List<int> { OOCrew.Last()[0], DDCrew.Last()[0] }, 1000);
                this.SortedPathSet.Add(stayPath);
            }
        }
        public void GeneratLB_SpeedUp(ref double BestLB, int k, ref CreateNetWork net)
        {
            //定义及初始化变量
            int i = 0, j = 0, v = 0;//计数器
            double sum = 0.0, sumlag = 0.0, cost = 0.0, sumcost = 0.0;           
            
            ArrayList shortpathtemp = new ArrayList();
            Dictionary<string, T_S_S_Arc> arcs = new Dictionary<string, T_S_S_Arc>();
            Dictionary<int, Line> lines = new Dictionary<int, Line>();

            init_SortedPathSet();
            LB_PathSet = new List<Pairing>();
                       
            //初始化时空状态网络中弧被选中次数
            for (i = 0; i < net.T_S_S_ArcList.Count; i++)
            {
                net.T_S_S_ArcList[i].NumSelected = 0;
                string key = net.T_S_S_ArcList[i].StartPointID.ToString() + "," + net.T_S_S_ArcList[i].EndPointID.ToString();
                arcs.Add(key, net.T_S_S_ArcList[i]);
            }
            for (i = 0; i < net.LineList.Count; i++)
            {
                net.LineList[i].NumSelected = 0;
                int key = net.LineList[i].LineID;
                lines.Add(key, net.LineList[i]);
            }

            //step1.1  对于每一个乘务组迭代
            //改进：while所有task被覆盖完了就够了，不用再进行多余的循环
            #region find top Np shortest path, only number of crewbases iteration
            i = j = 0;
            for (int c = OOCrew.Count-1; c >= 0; c--)
            //for (int c = 0; c < OOCrew.Count; c++)
            {
                shortpath.Clear();
                cost = 0.0;
                dyna.Initialize(net);
                dyna.Cal_Dynamic_Speedup(OOCrew[c][0], DDCrew[c][0], CrewType[c],
                                        0, 0,
                                        ref net, 1, ref shortpath, ref cost);
                //to sort two sorted array
                Pairing temPath;
                j = 0;
                for (i = 0; i < dyna.SortedPathSet.Count; i++)
                { 
                    temPath = dyna.SortedPathSet[i];
                    if (temPath.Price >= net.virtualRoutingCost)
                    {
                        break;
                    }
                    
                    while (j < FixedCrewNum && temPath.Price > this.SortedPathSet[j].Price)
                    {
                        j++;
                    }
                    if (j < FixedCrewNum)
                    {
                        this.SortedPathSet.Insert(j, temPath);
                        j++; //下一次直接从当前插入点开始比较
                    }
                    else
                    {
                        break;
                    }
                }                
            }

            SortedPathSet.RemoveRange(FixedCrewNum, SortedPathSet.Count - FixedCrewNum);            
            foreach (var path in SortedPathSet)
            {
                LB_PathSet.Add(path);
            }

            //更新弧选择次数，并计算sumcost
            Pairing tempPath;
            i = 0;
            while (i < FixedCrewNum)
            {
                tempPath = SortedPathSet[i];                

                sumcost += tempPath.Price;
                for (v = 0; v < tempPath.Route.Count - 1; v++)
                {
                    string shortPathArcKey = tempPath.Route[v].ToString() + "," + tempPath.Route[v + 1].ToString();                    
                    if (arcs.ContainsKey(shortPathArcKey))
                    {
                        arcs[shortPathArcKey].NumSelected += 1;
                    }
                }

                i++;
                if (AllTaskCovered(net.T_S_S_ArcList, lines)) //若所有task已经被现有的路覆盖提前跳出
                {
                    //sumcost需要处理一下，剩余的乘务组均值乘虚拟停驻弧
                    for (; i < FixedCrewNum; i++)
                    {
                        sum += net.virtualRoutingCost;                        
                    }
                }                
            }
            #endregion            
            
            //step1.2  带入松弛对偶问题求出的解向量，求拉格朗日松弛对偶问题目标函数
            for (i = 0; i < net.LineList.Count; i++)
            {
                sumlag = sumlag + net.LineList[i].LagMultiplier;//计算所有运行线的拉格朗日乘子的和，第二项的值
            }
            //每次迭代的下界目标值
            sum = sumcost + sumlag;            
            //将每一步的迭代值加入到下届迭代曲线值数组中
            if (k >= 0)
            {
                CurLB.Add(sum);
            }
            //判断当前最优下界
            if (sum > BestLB)//若下界提高了
            {
                BestLB = sum;                
            }
            //将最优下届加入到最优下届迭代曲线值数组中  
            if (k >= 0)
            {
                CurBestLB.Add(BestLB);
            }
            //step1.3  次梯度法更新拉格朗日乘子
            double subgradient = 0.0;//, virsubgradient = 0.0;//运行弧次梯度和虚拟停驻弧次梯度
            //更新步长

            stepSize_iter.Add(step_size / (k + 1.0));
            //for (i = 0; i < stepList.Count; i++)
            //{                                
            //    stepList[i] = 400.0 / (k + 1.0);
            //}

            //4-16
            int lineId = 0;
            foreach (var taskarc in type_arcPair[1])
            {
                lineId = taskarc.LineID;
                if (lines.ContainsKey(lineId))
                {
                    lines[lineId].NumSelected += taskarc.NumSelected;
                }
            }
            //运行弧次梯度更新运行线乘子
            for (i = 0; i < net.LineList.Count; i++)
            {
                subgradient = 1 - net.LineList[i].NumSelected;

                if (net.LineList[i].LagMultiplier + stepSize_iter[k] * subgradient > 0)
                {
                    net.LineList[i].LagMultiplier = net.LineList[i].LagMultiplier + stepSize_iter[k] * subgradient;
                }
                else
                {
                    net.LineList[i].LagMultiplier = 0;
                }
            }
            //时空状态网络运行弧和虚拟停驻弧乘子更新
            foreach (var taskarc in type_arcPair[1])
            {
                lineId = taskarc.LineID;
                if (lines.ContainsKey(lineId))
                {
                    taskarc.LagMultiplier = lines[lineId].LagMultiplier;
                }
            }

        }

        public void GeneratLB_SpeedUp(ref double BestLB, int k, ref CreateNetWork_db net) {
            //定义及初始化变量
            int i = 0, j = 0, v = 0;//计数器
            
            ArrayList shortpathtemp = new ArrayList();
            //Dictionary<string, T_S_S_Arc> arcs = new Dictionary<string, T_S_S_Arc>();
            //Dictionary<int, Line> lines = new Dictionary<int, Line>();

            init_SortedPathSet();
            LB_PathSet = new List<Pairing>();

            //初始化时空状态网络中弧被选中次数
            for (i = 0; i < net.T_S_S_ArcList.Count; i++) {
                net.T_S_S_ArcList[i].NumSelected = 0;
                //string key = net.T_S_S_ArcList[i].StartPointID.ToString() + "," + net.T_S_S_ArcList[i].EndPointID.ToString();
                //arcs.Add(key, net.T_S_S_ArcList[i]);
            }
            for (i = 0; i < net.LineList.Count; i++) {
                net.LineList[i].NumSelected = 0;
                //int key = net.LineList[i].LineID;
                //lines.Add(key, net.LineList[i]);
            }

            //step1.1  对于每一个乘务组迭代
            //改进：while所有task被覆盖完了就够了，不用再进行多余的循环
            #region find top Np shortest path, only number of crewbases iteration
            i = j = 0;
            double cost = 0;
            for (int c = OOCrew.Count - 1; c >= 0; c--)
            //for (int c = 0; c < OOCrew.Count; c++)
            {
                shortpath.Clear();
                cost = 0.0;
                dyna.Initialize(net);
                dyna.Cal_Dynamic_Speedup(OOCrew[c][0], DDCrew[c][0], CrewType[c],
                                        0, 0,
                                        ref net, 1, ref shortpath, ref cost);
                //to sort two sorted array
                Pairing temPath;
                j = 0;
                for (i = 0; i < dyna.SortedPathSet.Count; i++) {
                    temPath = dyna.SortedPathSet[i];
                    if (temPath.Price >= net.virtualRoutingCost) {
                        break;
                    }

                    while (j < FixedCrewNum && temPath.Price > this.SortedPathSet[j].Price) {
                        j++;
                    }
                    if (j < FixedCrewNum) {
                        this.SortedPathSet.Insert(j, temPath);
                        j++; //下一次直接从当前插入点开始比较
                    }
                    else {
                        break;
                    }
                }
            }

            SortedPathSet.RemoveRange(FixedCrewNum, SortedPathSet.Count - FixedCrewNum);
            foreach (var path in SortedPathSet) {
                LB_PathSet.Add(path);
            }

            //更新弧选择次数，并计算sumcost
            Pairing tempPath;
            i = 0;
            double sum = 0.0, sumlag = 0.0, sumcost = 0.0;

            double temp_readonly_LB_without_penalty = 0;
            double sum_penalty = 0;
            int real_pairing_num = 0;

            while (i < FixedCrewNum) {
                tempPath = SortedPathSet[i];

                sumcost += tempPath.Price;                
                int count = tempPath.Route.Count;
                for (v = 0; v < count - 1; v++) {
                    string shortPathArcKey = tempPath.Route[v].ToString() + "," + tempPath.Route[v + 1].ToString();
                    //if (arcs.ContainsKey(shortPathArcKey)) {
                    //    arcs[shortPathArcKey].NumSelected += 1;
                    //}
                    if (g_ArcSet.ContainsKey(shortPathArcKey)) {
                        g_ArcSet[shortPathArcKey].NumSelected += 1;

                        sum_penalty += g_ArcSet[shortPathArcKey].PenaltyMealViolate;
                    }
                    else {
                        throw new KeyNotFoundException("LB_speed error, arc not found in g_ArcSet!!");
                    }
                }

                ////db
                //// 计算LB_without_penalty
                //if (count > 2) {
                //    ++real_pairing_num;
                //    string first = tempPath.Route[0].ToString() + "," + tempPath.Route[1].ToString();
                //    string last = tempPath.Route[count - 2].ToString() + "," + tempPath.Route[count - 1].ToString();
                //    temp_readonly_LB_without_penalty += 1440 + g_ArcSet[last].StartTimeCode - g_ArcSet[first].EndTimeCode;
                //}
                ////end db

                i++;
                if (AllTaskCovered(net.T_S_S_ArcList, g_LineSet)) //若所有task已经被现有的路覆盖提前跳出
                {
                    //sumcost需要处理一下，剩余的乘务组均值乘虚拟停驻弧
                    for (; i < FixedCrewNum; i++) {
                        sum += net.virtualRoutingCost;
                    }
                }
            }
            temp_readonly_LB_without_penalty = sumcost - sum_penalty;
            //temp_readonly_LB_without_penalty += (FixedCrewNum - real_pairing_num) * net.virtualRoutingCost;
            #endregion

            //step1.2  带入松弛对偶问题求出的解向量，求拉格朗日松弛对偶问题目标函数
            for (i = 0; i < net.LineList.Count; i++) {
                sumlag += net.LineList[i].LagMultiplier;//计算所有运行线的拉格朗日乘子的和，第二项的值
            }
            //每次迭代的下界目标值
            sum = sumcost + sumlag;

            temp_readonly_LB_without_penalty += sumlag;
            BestLB_without_penalty = temp_readonly_LB_without_penalty > BestLB_without_penalty ?
                temp_readonly_LB_without_penalty : BestLB_without_penalty;
            //将每一步的迭代值加入到下届迭代曲线值数组中
            if (k >= 0) {
                CurLB.Add(sum);
                CurLB_without_penalty.Add(temp_readonly_LB_without_penalty);
                CurLB_with_penalty.Add(sum);
            }
            //判断当前最优下界
            if (sum > BestLB)//若下界提高了
            {
                BestLB = sum;
                BestLB_with_penalty = sum;
            }
            //将最优下届加入到最优下届迭代曲线值数组中  
            if (k >= 0) {
                CurBestLB.Add(BestLB);                
            }
            //step1.3  次梯度法更新拉格朗日乘子
            double subgradient = 0.0;//, virsubgradient = 0.0;//运行弧次梯度和虚拟停驻弧次梯度
            //更新步长

            stepSize_iter.Add(step_size / (k + 1.0));
            //for (i = 0; i < stepList.Count; i++)
            //{                                
            //    stepList[i] = 400.0 / (k + 1.0);
            //}

            //4-16
            int lineId = 0;
            foreach (var taskarc in type_arcPair[1]) {
                lineId = taskarc.LineID;
                //if (lines.ContainsKey(lineId)) {
                //    lines[lineId].NumSelected += taskarc.NumSelected;
                //}
                if (g_LineSet.ContainsKey(lineId)) {
                    g_LineSet[lineId].NumSelected += taskarc.NumSelected;
                }
            }
            //运行弧次梯度更新运行线乘子
            for (i = 0; i < net.LineList.Count; i++) {
                subgradient = 1 - net.LineList[i].NumSelected;

                if (net.LineList[i].LagMultiplier + stepSize_iter[k] * subgradient > 0) {
                    net.LineList[i].LagMultiplier = net.LineList[i].LagMultiplier + stepSize_iter[k] * subgradient;
                }
                else {
                    net.LineList[i].LagMultiplier = 0;
                }
            }
            //时空状态网络运行弧和虚拟停驻弧乘子更新
            foreach (var taskarc in type_arcPair[1]) {
                lineId = taskarc.LineID;
                if (g_LineSet.ContainsKey(lineId)) {
                    taskarc.LagMultiplier = g_LineSet[lineId].LagMultiplier;
                }
            }

        }

        bool AllTaskCovered(List<T_S_S_Arc> tssn_ArcList, Dictionary<int, Line> lines) 
        {
            //4-16
            foreach (var taskArc in type_arcPair[1])
            {
                int lineId = taskArc.LineID;
                if (lines.ContainsKey(lineId) &&  taskArc.NumSelected == 0)
                {
                    return false;
                }
            }

            return true;
        }
        
        public void GeneratUB_SpeedUp(ref double BestUB, int k, ref CreateNetWork net)
        {
            //定义变量
            int i = 0, z = 0, ncover = 1;//计数器
            double sum = 0.0, sumT = 0.0, cost = 0.0;

            double cost2 = 0.0;

            bool coverfinish = false, recovered = false, isAdd = false;            
            Dictionary<string, T_S_S_Arc> arcs = new Dictionary<string, T_S_S_Arc>();
            Dictionary<int, Line> lines = new Dictionary<int, Line>();
            Dictionary<int, List<T_S_S_Arc>> lineIDArcs = new Dictionary<int, List<T_S_S_Arc>>();
            //int CurNs = 0;            
            templist.Clear();

            SortedPathSet.Clear();

            for (i = 0; i < net.T_S_S_ArcList.Count; i++)
            {
                string key = net.T_S_S_ArcList[i].StartPointID.ToString() + "," + net.T_S_S_ArcList[i].EndPointID.ToString();
                arcs.Add(key, net.T_S_S_ArcList[i]);

                int lineId = net.T_S_S_ArcList[i].LineID;
                if (lineIDArcs.ContainsKey(lineId))
                    lineIDArcs[lineId].Add(net.T_S_S_ArcList[i]);
                else
                {
                    List<T_S_S_Arc> list = new List<T_S_S_Arc>();
                    list.Add(net.T_S_S_ArcList[i]);
                    lineIDArcs.Add(lineId, list);
                }

            }
            for (i = 0; i < net.LineList.Count; i++)
            {
                int key = net.LineList[i].LineID;
                lines.Add(key, net.LineList[i]);
            }
            //初始化价格，根据拉格朗日松弛算法求得的下界解，给资源定价
            for (i = 0; i < net.T_S_S_ArcList.Count; i++)
            {

                int lineId = net.T_S_S_ArcList[i].LineID;
                if (lines.ContainsKey(lineId) && net.T_S_S_ArcList[i].ArcType == 1)
                {
                    net.T_S_S_ArcList[i].HeurPrice = -(lines[lineId].NumSelected + 1) * C;
                }
                else if (net.T_S_S_ArcList[i].ArcType != 1)
                {
                    net.T_S_S_ArcList[i].HeurPrice = net.T_S_S_ArcList[i].Price;
                }

            }
            //初始化个弧被选中次数为0
            for (i = 0; i < net.T_S_S_ArcList.Count; i++)
            {
                net.T_S_S_ArcList[i].NumSelected = 0;
            }
            for (i = 0; i < net.LineList.Count; i++)
            {
                net.LineList[i].NumSelected = 0;
            }

            /*step2.1启发是算法：对每一个乘务组搜索其最短路，
             * 注意两点：1，尽可能避免同一条运行线出现两次或以上的现象，即便乘（可行解中已经排除一列车先后执行俩天的同一条运行线），
             * 2.搜索完最短路后时时更新所经过的路径价格(只更新运行弧价格)，让其价格升高，避免后来的车重复走此路，
             * 所有的下界问题解的的弧被选完，则在降低所有运行弧的价格*/
            for (z = 0; z < OCrew.Count; z++)
            {
                shortpath.Clear();
                if (ncover == 0)//若所有弧都被覆盖，初始值为1
                {
                    coverfinish = true;
                    //剩余的乘务组均值乘虚拟停驻弧
                    for (; z < OCrew.Count; z++)
                    {                        
                        Pairing stayPath = new Pairing(new List<int> { OOCrew.Last()[0], DDCrew.Last()[0] }, 1000);
                        SortedPathSet.Add(stayPath);
                    }
                    break;
                }
                else
                {
                    coverfinish = false;
                }

                if (coverfinish == false)//还未覆盖完成,虚拟停驻弧定价为极大值
                {                    
                    cost = 0.0;
                    dyna.Initialize(net);
                    dyna.Cal_Dynamic_Speedup(OCrew[z], DCrew[z], CrewType[z], CrewOutCost[z], CrewRemainCost[z],
                                             ref net, 2, ref shortpath, ref cost);

                    #region 其他基地求最短
                    for (int c = 0; c < OOCrew.Count - 1; c++) //Ocrew,DCrew是最后一个基地。所以这里算最后一个之前的基地
                    {
                        shortpath2.Clear();
                        cost2 = 0.0;
                        dyna.Initialize(net);
                        dyna.Cal_Dynamic_Speedup(OOCrew[c][z], DDCrew[c][z], CrewType[z],
                                                 CrewOutCost[z], CrewRemainCost[z],
                                                 ref net, 2, ref shortpath2, ref cost2);
                        if (cost > cost2)
                        {
                            shortpath.Clear();
                            foreach (var node in shortpath2)
                            {
                                shortpath.Add(node);
                            }
                            cost = cost2;
                        }
                    }
                    #endregion

                    recovered = false;
                    Pairing newPath = new Pairing(shortpath, cost);
                    newPath.TransRouteToLinesAndCalCost_with_and_without_penalty(arcs);

                    foreach (var path in SortedPathSet)
                    {
                        if (path.PassLines.Count == 0)
                        {
                            path.TransRouteToLinesAndCalCost_with_and_without_penalty(arcs);
                        }
                        if (newPath.Route.Count > 2 && newPath.PassLines.SequenceEqual(path.PassLines))
                        {
                            recovered = true;
                            break;
                        }
                    }
                    if (recovered == false)
                    {
                        SortedPathSet.Add(newPath);
                    }                   
                }
                else
                {
                    cost = 0.0;
                    dyna.Initialize(net);                   
                    dyna.Cal_Dynamic_Speedup(OCrew[z], DCrew[z], CrewType[z], CrewOutCost[z], CrewRemainCost[z],
                                             ref net, 3, ref shortpath, ref cost);

                    #region 其他基地求最短
                    for (int c = 0; c < OOCrew.Count - 1; c++) //Ocrew,DCrew是最后一个基地。所以这里算最后一个之前的基地
                    {
                        shortpath2.Clear();
                        cost2 = 0.0;
                        dyna.Initialize(net);
                        dyna.Cal_Dynamic_Speedup(OOCrew[c][z], DDCrew[c][z], CrewType[z],
                                                 CrewOutCost[z], CrewRemainCost[z],
                                                 ref net, 3, ref shortpath2, ref cost2);
                        if (cost > cost2)
                        {
                            shortpath.Clear();
                            foreach (var node in shortpath2)
                            {
                                shortpath.Add(node);
                            }
                            cost = cost2;
                        }
                    }
                    #endregion

                    Pairing newPath = new Pairing(shortpath, cost);
                    SortedPathSet.Add(newPath);
                }
                //更新选中次数
                for (i = 0; i < shortpath.Count - 1; i++)
                {
                    string shortPathArcKey = shortpath[i].ToString() + "," + shortpath[i + 1].ToString();
                    if (arcs.ContainsKey(shortPathArcKey))
                    {
                        arcs[shortPathArcKey].NumSelected += 1;
                    }
                }
                ncover = 0;
                //计算每条运行线被选中的次数,并抬高被选中运行线的价格
                //4-16
                int lineId = 0;
                foreach (var taskarc in type_arcPair[1])
                {
                    lineId = taskarc.LineID;
                    if (lines.ContainsKey(lineId))
                    {
                        lines[lineId].NumSelected += taskarc.NumSelected;
                    }
                }

                isAdd = false;
                //根据运行线被选中的情况对运行弧定价
                foreach (KeyValuePair<int, List<T_S_S_Arc>> item in lineIDArcs)
                {
                    lineId = item.Key;
                    List<T_S_S_Arc> arcsOfLine = item.Value;
                    //未被选中过的：1.重复覆盖情况发生，令所有为被选中过的弧为—C*M，z--，代表此次迭代不算
                    if (lines.ContainsKey(lineId))
                    {
                        if (lines[lineId].NumSelected == 0)
                        {
                            ncover++;
                            //若重复覆盖，定价。未发生重复覆盖，直接隶属后跳过，改弧的价格还为以前的值。
                            if (recovered == true)
                            {
                                foreach (T_S_S_Arc arcItem in arcsOfLine)
                                {
                                    if (arcItem.ArcType == 1 && arcItem.NumSelected == 0)//arcItem.NumSelected == 0 此条件可以省略
                                    {
                                        arcItem.HeurPrice = -2.5 * M;//若此线已被选则抬高此条运行线所有状态运行弧的价格，尽可能避免便乘情况发生
                                    }
                                }
                                if (z > 0 && isAdd == false)
                                {                                   
                                    Pairing stayPath = new Pairing(new List<int> { OOCrew.Last()[0], DDCrew.Last()[0] }, 1000);
                                    SortedPathSet.Add(stayPath);

                                    isAdd = true;
                                }
                            }
                        }
                        else
                        {
                            foreach (T_S_S_Arc arcItem in arcsOfLine)
                            {
                                if (arcItem.ArcType == 1)
                                {
                                    arcItem.HeurPrice = M;//若此线已被选则抬高此条运行线所有状态运行弧的价格，尽可能避免便乘情况发生
                                }
                            }
                        }
                    }

                }
            }

            while(SortedPathSet.Count < FixedCrewNum)
            {
                Pairing stayPath = new Pairing(new List<int> { OOCrew.Last()[0], DDCrew.Last()[0] }, 1000);
                SortedPathSet.Add(stayPath);
            }

            sumT = 0;
            sum = 0;
            foreach (var path in SortedPathSet)
            {
                //求sumT and Ssumcost
                for (i = 0; i < path.Route.Count - 1; i++)
                {
                    string key = path.Route[i].ToString() + "," + path.Route[i + 1].ToString();
                    if (arcs.ContainsKey(key))
                    {
                        if (arcs[key].ArcType == 1)
                        {                            
                            sumT += arcs[key].Price;                                                                                    
                        }

                        sum += arcs[key].Price;                           
                    }             
                }
             
            }
            //step2.2  根据此次迭代所求出的可行上界解求原问题目标函数
            //每次迭代的上界目标值
            //判断当前最优上界
            if (k >= 0)
            {
                CurUB.Add(sum);
            }
            //更新最优Ns
            if (sum <= BestUB || (sum == BestUB && sumT <= BestT))//若上界降低或是便乘时间少了
            //if (BestNgo >= OCrew.Count - CurNs|| (BestNgo == OCrew.Count - CurNs && sumT <= BestT))//若上界降低或是便乘时间少了
            {
                BestUB = sum;
                BestT = sumT;
                //BestNgo = OCrew.Count - CurNs;
                YUB_iter.Clear();
                foreach (var path in SortedPathSet)
                {
                    List<int> route = new List<int>(path.Route);
                    YUB_iter.Add(route);
                }
              
                nubstay = 0;
            }
            else
            {
                nubstay++;
            }
            if (k >= 0)
            {
                CurBestUB.Add(BestUB);
            }
            //4-16            
            foreach (var stayarc in type_arcPair[33])
            {
                stayarc.BestNs = stayarc.NumSelected;
            }

            //Ns = OCrewTemp.Count - BestNgo;
        }
        public void GeneratUB_SpeedUp(ref double BestUB, int k, ref CreateNetWork_db net) {
            //定义变量
            int i = 0, z = 0, ncover = 1;//计数器
            double sum = 0.0, sumT = 0.0, cost = 0.0;

            double cost2 = 0.0;

            bool coverfinish = false, recovered = false, isAdd = false;
            Dictionary<string, T_S_S_Arc> arcs = new Dictionary<string, T_S_S_Arc>();
            Dictionary<int, Line> lines = new Dictionary<int, Line>();
            Dictionary<int, List<T_S_S_Arc>> lineIDArcs = new Dictionary<int, List<T_S_S_Arc>>();
            //int CurNs = 0;            
            templist.Clear();

            SortedPathSet.Clear();

            for (i = 0; i < net.T_S_S_ArcList.Count; i++) {
                string key = net.T_S_S_ArcList[i].StartPointID.ToString() + "," + net.T_S_S_ArcList[i].EndPointID.ToString();
                arcs.Add(key, net.T_S_S_ArcList[i]);

                int lineId = net.T_S_S_ArcList[i].LineID;
                if (lineIDArcs.ContainsKey(lineId))
                    lineIDArcs[lineId].Add(net.T_S_S_ArcList[i]);
                else {
                    List<T_S_S_Arc> list = new List<T_S_S_Arc>();
                    list.Add(net.T_S_S_ArcList[i]);
                    lineIDArcs.Add(lineId, list);
                }

            }
            for (i = 0; i < net.LineList.Count; i++) {
                int key = net.LineList[i].LineID;
                lines.Add(key, net.LineList[i]);
            }
            //初始化价格，根据拉格朗日松弛算法求得的下界解，给资源定价
            for (i = 0; i < net.T_S_S_ArcList.Count; i++) {

                int lineId = net.T_S_S_ArcList[i].LineID;
                if (lines.ContainsKey(lineId) && net.T_S_S_ArcList[i].ArcType == 1) {
                    net.T_S_S_ArcList[i].HeurPrice = -(lines[lineId].NumSelected + 1) * C;
                }
                else if (net.T_S_S_ArcList[i].ArcType != 1) {
                    net.T_S_S_ArcList[i].HeurPrice = net.T_S_S_ArcList[i].Price;
                }

            }
            //初始化个弧被选中次数为0
            for (i = 0; i < net.T_S_S_ArcList.Count; i++) {
                net.T_S_S_ArcList[i].NumSelected = 0;
            }
            for (i = 0; i < net.LineList.Count; i++) {
                net.LineList[i].NumSelected = 0;
            }

            /*step2.1启发是算法：对每一个乘务组搜索其最短路，
             * 注意两点：1，尽可能避免同一条运行线出现两次或以上的现象，即便乘（可行解中已经排除一列车先后执行俩天的同一条运行线），
             * 2.搜索完最短路后时时更新所经过的路径价格(只更新运行弧价格)，让其价格升高，避免后来的车重复走此路，
             * 所有的下界问题解的的弧被选完，则在降低所有运行弧的价格*/
            for (z = 0; z < OCrew.Count; z++) {
                shortpath.Clear();
                if (ncover == 0)//若所有弧都被覆盖，初始值为1
                {
                    coverfinish = true;
                    //剩余的乘务组均值乘虚拟停驻弧
                    for (; z < OCrew.Count; z++) {
                        Pairing stayPath = new Pairing(new List<int> { OOCrew.Last()[0], DDCrew.Last()[0] }, 1000);
                        SortedPathSet.Add(stayPath);
                    }
                    break;
                }
                else {
                    coverfinish = false;
                }

                if (coverfinish == false)//还未覆盖完成,虚拟停驻弧定价为极大值
                {
                    cost = 0.0;
                    dyna.Initialize(net);
                    dyna.Cal_Dynamic_Speedup(OCrew[z], DCrew[z], CrewType[z], CrewOutCost[z], CrewRemainCost[z],
                                             ref net, 2, ref shortpath, ref cost);

                    #region 其他基地求最短
                    for (int c = 0; c < OOCrew.Count - 1; c++) //Ocrew,DCrew是最后一个基地。所以这里算最后一个之前的基地
                    {
                        shortpath2.Clear();
                        cost2 = 0.0;
                        dyna.Initialize(net);
                        dyna.Cal_Dynamic_Speedup(OOCrew[c][z], DDCrew[c][z], CrewType[z],
                                                 CrewOutCost[z], CrewRemainCost[z],
                                                 ref net, 2, ref shortpath2, ref cost2);
                        if (cost > cost2) {
                            shortpath.Clear();
                            foreach (var node in shortpath2) {
                                shortpath.Add(node);
                            }
                            cost = cost2;
                        }
                    }
                    #endregion

                    recovered = false;
                    Pairing newPath = new Pairing(shortpath, cost);
                    newPath.TransRouteToLinesAndCalCost_with_and_without_penalty(arcs);

                    foreach (var path in SortedPathSet) {
                        if (path.PassLines.Count == 0) {
                            path.TransRouteToLinesAndCalCost_with_and_without_penalty(arcs);
                        }
                        if (newPath.Route.Count > 2 && newPath.PassLines.SequenceEqual(path.PassLines)) {
                            recovered = true;
                            break;
                        }
                    }
                    if (recovered == false) {
                        SortedPathSet.Add(newPath);
                    }
                }
                else {
                    cost = 0.0;
                    dyna.Initialize(net);
                    dyna.Cal_Dynamic_Speedup(OCrew[z], DCrew[z], CrewType[z], CrewOutCost[z], CrewRemainCost[z],
                                             ref net, 3, ref shortpath, ref cost);

                    #region 其他基地求最短
                    for (int c = 0; c < OOCrew.Count - 1; c++) //Ocrew,DCrew是最后一个基地。所以这里算最后一个之前的基地
                    {
                        shortpath2.Clear();
                        cost2 = 0.0;
                        dyna.Initialize(net);
                        dyna.Cal_Dynamic_Speedup(OOCrew[c][z], DDCrew[c][z], CrewType[z],
                                                 CrewOutCost[z], CrewRemainCost[z],
                                                 ref net, 3, ref shortpath2, ref cost2);
                        if (cost > cost2) {
                            shortpath.Clear();
                            foreach (var node in shortpath2) {
                                shortpath.Add(node);
                            }
                            cost = cost2;
                        }
                    }
                    #endregion

                    Pairing newPath = new Pairing(shortpath, cost);
                    SortedPathSet.Add(newPath);
                }
                //更新选中次数
                for (i = 0; i < shortpath.Count - 1; i++) {
                    string shortPathArcKey = shortpath[i].ToString() + "," + shortpath[i + 1].ToString();
                    if (arcs.ContainsKey(shortPathArcKey)) {
                        arcs[shortPathArcKey].NumSelected += 1;
                    }
                }
                ncover = 0;
                //计算每条运行线被选中的次数,并抬高被选中运行线的价格
                //4-16
                int lineId = 0;
                foreach (var taskarc in type_arcPair[1]) {
                    lineId = taskarc.LineID;
                    if (lines.ContainsKey(lineId)) {
                        lines[lineId].NumSelected += taskarc.NumSelected;
                    }
                }

                isAdd = false;
                //根据运行线被选中的情况对运行弧定价
                foreach (KeyValuePair<int, List<T_S_S_Arc>> item in lineIDArcs) {
                    lineId = item.Key;
                    List<T_S_S_Arc> arcsOfLine = item.Value;
                    //未被选中过的：1.重复覆盖情况发生，令所有为被选中过的弧为—C*M，z--，代表此次迭代不算
                    if (lines.ContainsKey(lineId)) {
                        if (lines[lineId].NumSelected == 0) {
                            ncover++;
                            //若重复覆盖，定价。未发生重复覆盖，直接隶属后跳过，改弧的价格还为以前的值。
                            if (recovered == true) {
                                foreach (T_S_S_Arc arcItem in arcsOfLine) {
                                    if (arcItem.ArcType == 1 && arcItem.NumSelected == 0)//arcItem.NumSelected == 0 此条件可以省略
                                    {
                                        arcItem.HeurPrice = -2.5 * M;//若此线已被选则抬高此条运行线所有状态运行弧的价格，尽可能避免便乘情况发生
                                    }
                                }
                                if (z > 0 && isAdd == false) {
                                    Pairing stayPath = new Pairing(new List<int> { OOCrew.Last()[0], DDCrew.Last()[0] }, 1000);
                                    SortedPathSet.Add(stayPath);

                                    isAdd = true;
                                }
                            }
                        }
                        else {
                            foreach (T_S_S_Arc arcItem in arcsOfLine) {
                                if (arcItem.ArcType == 1) {
                                    arcItem.HeurPrice = M;//若此线已被选则抬高此条运行线所有状态运行弧的价格，尽可能避免便乘情况发生
                                }
                            }
                        }
                    }

                }
            }

            while (SortedPathSet.Count < FixedCrewNum) {
                Pairing stayPath = new Pairing(new List<int> { OOCrew.Last()[0], DDCrew.Last()[0] }, 1000);
                SortedPathSet.Add(stayPath);
            }

            sumT = 0;
            sum = 0;
            foreach (var path in SortedPathSet) {
                //求sumT and Ssumcost
                for (i = 0; i < path.Route.Count - 1; i++) {
                    string key = path.Route[i].ToString() + "," + path.Route[i + 1].ToString();
                    if (arcs.ContainsKey(key)) {
                        if (arcs[key].ArcType == 1) {
                            sumT += arcs[key].Price;
                        }

                        sum += arcs[key].Price;
                    }
                }

            }
            //step2.2  根据此次迭代所求出的可行上界解求原问题目标函数
            //每次迭代的上界目标值
            //判断当前最优上界
            if (k >= 0) {
                CurUB.Add(sum);
            }
            //更新最优Ns
            if (sum <= BestUB || (sum == BestUB && sumT <= BestT))//若上界降低或是便乘时间少了
            //if (BestNgo >= OCrew.Count - CurNs|| (BestNgo == OCrew.Count - CurNs && sumT <= BestT))//若上界降低或是便乘时间少了
            {
                BestUB = sum;
                BestT = sumT;
                //BestNgo = OCrew.Count - CurNs;
                YUB_iter.Clear();
                foreach (var path in SortedPathSet) {
                    List<int> route = new List<int>(path.Route);
                    YUB_iter.Add(route);
                }

                nubstay = 0;
            }
            else {
                nubstay++;
            }
            if (k >= 0) {
                CurBestUB.Add(BestUB);
            }
            //4-16            
            foreach (var stayarc in type_arcPair[33]) {
                stayarc.BestNs = stayarc.NumSelected;
            }

            //Ns = OCrewTemp.Count - BestNgo;
        }
        public void TranslationCSP(List<int> path, 
            CreateNetWork net, 
            ref string str,
            ref double T,
            ref double dis,
            double outcost,double remaincost)
        {
            string strtemp = "";
            bool isnight = false, isday = false, islunch = false, isdinner = false;
            Dictionary<string, T_S_S_Arc> arcs = new Dictionary<string, T_S_S_Arc>();
            dis = 0;
            int v = 0, j = 0, starttime = 0, endtime = 0;//, crewtime = 0;
            //初始化时空状态网络中弧被选中次数
            for (int i = 0; i < net.T_S_S_ArcList.Count; i++)
            {
                string key = net.T_S_S_ArcList[i].StartPointID.ToString() + "," + net.T_S_S_ArcList[i].EndPointID.ToString();
                arcs.Add(key, net.T_S_S_ArcList[i]);
            }
            if (path.Count>2)
            {
                foreach(CrewBase crewbase in net.CrewBaseList)
                {
                    if(path[0]==crewbase.OIDinTSS)
                    {
                        str += crewbase.StaCode.ToString() + "站";
                        break; //changed 20191119
                    }
                }
                for (j = 0; j < net.T_S_S_ArcList.Count; j++)
                {
                    if (path[0] == net.T_S_S_ArcList[j].StartPointID && path[1] == net.T_S_S_ArcList[j].EndPointID)
                    {
                        int hour = 0, min =0;
                        hour = net.T_S_S_ArcList[j].EndTimeCode/60;
                        min = net.T_S_S_ArcList[j].EndTimeCode%60;
                        starttime = net.T_S_S_ArcList[j].EndTimeCode - 60;
                        str += (hour-1).ToString() + ":" + min.ToString() + "开始出乘到";
                        break; //changed 20191119
                    }
                }

                int pathLength = path.Count;
                foreach (CrewBase crewbase in net.CrewBaseList)
                {
                    if (path[pathLength-1] == crewbase.DIDinTSS)
                    {
                        str += crewbase.StaCode.ToString() + "站";
                        break; //changed 20191119
                    }
                }
                for (j = 0; j < net.T_S_S_ArcList.Count; j++)
                {
                    if (path[pathLength - 3] == net.T_S_S_ArcList[j].StartPointID 
                        && path[pathLength - 2] == net.T_S_S_ArcList[j].EndPointID)
                    {
                        int hour = 0, min = 0;
                        hour = net.T_S_S_ArcList[j].EndTimeCode/ 60;
                        min = net.T_S_S_ArcList[j].EndTimeCode % 60;
                        endtime = net.T_S_S_ArcList[j].EndTimeCode + 60;
                        str += (hour+1).ToString() + ":" + min.ToString() + "退乘结束";
                        break; //changed 20191119
                    }
                }
                str += "内容如下：";
                //crewtime = endtime - starttime;
                //str += "交路长度为:" + crewtime.ToString() + "分钟内容如下：";
            }
            for (v = 0; v < path.Count - 1; v++)
            {
                string key = path[v].ToString() + "," + path[v + 1].ToString();
                if (arcs.ContainsKey(key))
                {
                    if (arcs[key].ArcType == 1)
                    {
                        str += arcs[key].TrainCode.ToString() + "→";
                        T += arcs[key].Price;
                        isday = false;
                        isnight = false;                        
                        dis += arcs[key].Price;
                    }
                    else if (arcs[key].ArcType != 1)
                    {
                        dis += arcs[key].Price;
                    }
                    if (arcs[key].ArcType == 22 && isday == false)
                    {
                        str += "间休" + "→";
                        strtemp = str.Substring(str.Length - 3, 2);
                        if (strtemp == "间休")
                        {
                            isday = true;
                        }

                    }
                    if (arcs[key].ArcType == 23 && isnight == false)
                    {
                        str += "外段驻班" + "→";
                        isnight = true;
                    }
                    if (arcs[key].EndCumuLunch == 2 && islunch == false)
                    {
                        str += "午餐" + "→";
                        strtemp = str.Substring(str.Length - 3, 2);
                        if (strtemp == "午餐")
                        {
                            islunch = true;
                        }
                    }
                    if (arcs[key].EndCumuDinner == 2 && isdinner == false)
                    {
                        str += "晚餐" + "→";
                        strtemp = str.Substring(str.Length - 3, 2);
                        if (strtemp == "晚餐")
                        {
                            isdinner = true;
                        }
                    }
                }
            }

            if (str.Length > 0)
            {
                str = str.Substring(0, str.Length - 1);
            }
            else
            {
                str = "基地停驻";
            }
            str += " 交路长度为:" + (dis + 0).ToString();
        }

        public void TranslationCSP(List<int> path,
           CreateNetWork_db net,
           ref string str,
           ref double T,
           ref double dis,
           double outcost, double remaincost) {
            string strtemp = "";
            bool isnight = false, isday = false, islunch = false, isdinner = false;
            Dictionary<string, T_S_S_Arc> arcs = new Dictionary<string, T_S_S_Arc>();
            dis = 0;
            int v = 0, j = 0, starttime = 0, endtime = 0;//, crewtime = 0;
            //初始化时空状态网络中弧被选中次数
            for (int i = 0; i < net.T_S_S_ArcList.Count; i++) {
                string key = net.T_S_S_ArcList[i].StartPointID.ToString() + "," + net.T_S_S_ArcList[i].EndPointID.ToString();
                arcs.Add(key, net.T_S_S_ArcList[i]);
            }
            if (path.Count > 2) {
                foreach (CrewBase crewbase in net.CrewBaseList) {
                    if (path[0] == crewbase.OIDinTSS) {
                        str += crewbase.StaCode.ToString() + "站";
                        break; //changed 20191119
                    }
                }
                for (j = 0; j < net.T_S_S_ArcList.Count; j++) {
                    if (path[0] == net.T_S_S_ArcList[j].StartPointID && path[1] == net.T_S_S_ArcList[j].EndPointID) {
                        int hour = 0, min = 0;
                        hour = net.T_S_S_ArcList[j].EndTimeCode / 60;
                        min = net.T_S_S_ArcList[j].EndTimeCode % 60;
                        starttime = net.T_S_S_ArcList[j].EndTimeCode - 60;
                        str += (hour - 1).ToString() + ":" + min.ToString() + "开始出乘到";
                        break; //changed 20191119
                    }
                }

                int pathLength = path.Count;
                foreach (CrewBase crewbase in net.CrewBaseList) {
                    if (path[pathLength - 1] == crewbase.DIDinTSS) {
                        str += crewbase.StaCode.ToString() + "站";
                        break; //changed 20191119
                    }
                }
                for (j = 0; j < net.T_S_S_ArcList.Count; j++) {
                    if (path[pathLength - 3] == net.T_S_S_ArcList[j].StartPointID
                        && path[pathLength - 2] == net.T_S_S_ArcList[j].EndPointID) {
                        int hour = 0, min = 0;
                        hour = net.T_S_S_ArcList[j].EndTimeCode / 60;
                        min = net.T_S_S_ArcList[j].EndTimeCode % 60;
                        endtime = net.T_S_S_ArcList[j].EndTimeCode + 60;
                        str += (hour + 1).ToString() + ":" + min.ToString() + "退乘结束";
                        break; //changed 20191119
                    }
                }
                str += "内容如下：";
                //crewtime = endtime - starttime;
                //str += "交路长度为:" + crewtime.ToString() + "分钟内容如下：";
            }
            for (v = 0; v < path.Count - 1; v++) {
                string key = path[v].ToString() + "," + path[v + 1].ToString();
                if (arcs.ContainsKey(key)) {
                    if (arcs[key].ArcType == 1) {
                        str += arcs[key].TrainCode.ToString() + "→";
                        T += arcs[key].Price;
                        isday = false;
                        isnight = false;
                        dis += arcs[key].Price;
                    }
                    else if (arcs[key].ArcType != 1) {
                        dis += arcs[key].Price;
                    }
                    if (arcs[key].ArcType == 22 && isday == false) {
                        str += "间休" + "→";
                        strtemp = str.Substring(str.Length - 3, 2);
                        if (strtemp == "间休") {
                            isday = true;
                        }

                    }
                    if (arcs[key].ArcType == 23 && isnight == false) {
                        str += "外段驻班" + "→";
                        isnight = true;
                    }
                    if (arcs[key].EndCumuLunch == 2 && islunch == false) {
                        str += "午餐" + "→";
                        strtemp = str.Substring(str.Length - 3, 2);
                        if (strtemp == "午餐") {
                            islunch = true;
                        }
                    }
                    if (arcs[key].EndCumuDinner == 2 && isdinner == false) {
                        str += "晚餐" + "→";
                        strtemp = str.Substring(str.Length - 3, 2);
                        if (strtemp == "晚餐") {
                            isdinner = true;
                        }
                    }
                }
            }

            if (str.Length > 0) {
                str = str.Substring(0, str.Length - 1);
            }
            else {
                str = "基地停驻";
            }
            str += " 交路长度为:" + (dis + 0).ToString();
        }

        public void ShowProcessK(int K,RichTextBox rtbox ,ProgressBar probar,double BestLB,double BestUB)
        {
            double gap =(BestUB - BestLB) / BestUB;
            probar.Maximum = K;
            probar.Value = k;
            rtbox.Text = rtbox.Text + "第" + k.ToString() + "次迭代：最优上界为" + BestUB.ToString() +"；最优下界为" + BestLB.ToString() + "；Gap为" + gap.ToString() +"\r\n";
        }
    }
}
