using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;


namespace Crew_Planning
{

    static class TestMain
    {
        static void Main() {

            Test test = new Test();
            test.RunTest();

        }
    }

    class Test {

        public string[] TestInstances = { "京津", "沪杭", "BigScale", "BigScale2" };
        /// <summary>
        /// 注意，每个时间窗对应得最佳迭代次数也不一样
        /// </summary>
        public Dictionary<string, int> InstanceLRIteration = new Dictionary<string, int>()
        {
            {"京津", 60},
            {"沪杭", 60},
            {"BigScale", 60},
            {"BigScale2", 60},

            { "SmallCaseSet\\small01", 10}
        };
        public Dictionary<string, int> InstanceStepSize = new Dictionary<string, int>()
        {
            {"京津", 500}, // 200
            {"沪杭", 500}, // 350
            {"BigScale", 600}, //400
            {"BigScale2", 700}, //400

            { "SmallCaseSet\\small01", 100}
        };
        public Dictionary<string, int> InstanceVirRoutingCostMap = new Dictionary<string, int>() {
            {"京津", 1000},
            {"沪杭", 1000},
            {"BigScale", 1000},
            {"BigScale2", 1000}
        };

        public Dictionary<string, FixedMealWindow> FixedMealWindowsDict = new Dictionary<string, FixedMealWindow>()
        {
            { "2h", new FixedMealWindow(660, 780, 1020, 1140) }, //2h
            { "3h", new FixedMealWindow(630, 810, 990, 1170) }, //3h
            { "4h", new FixedMealWindow(600, 840, 960, 1200) }, //4h
            //{ "INF_h", new FixedMealWindow(4000, 8400, 9600, 14400) }//INF
            { "INF_h", new FixedMealWindow(0, 8400, 0, 14400) }//INF
            //new FixedMealWindows(3000, 7200, 7210, 28800)            
        };

        public Dictionary<string, int[]> InstanceMealSpan = new Dictionary<string, int[]>() {
            {"京津", new int[2]{25, 40} },
            {"沪杭", new int[2]{30, 40} },
            {"BigScale", new int[2]{10, 15}},//{2,15}
            {"BigScale2", new int[2]{10, 15}},//{ 6,15}

            { "SmallCaseSet\\small01", new int[2]{30, 40} }
        };

        
        private List<int[]> net_params = new List<int[]>() {
            new int[10]{180, 250, 40, 15, 3312, 3330, 3400, 3900, 180, 250 }, //京津城际实例
            new int[10]{240, 540, 90, 13, 3312, 3300, 3400, 3900, 240, 540 }, //沪宁杭
            new int[10]{180, 720, 200, 6, 3312, 3330, 600, 2800, 180, 720 }, //bigscale
            new int[10]{120, 780, 400, 6, 3312, 3330, 540, 2880, 120, 780 }, //bigscale2

            new int[10]{20, 450, 400, 15, 3312, 3330, 3400, 3900, 20, 450 } //smallcase01
        };
        private List<int[]> crewRules = new List<int[]>() {
            new int[6]{180, 250, 15, 40, 180, 250 },
            new int[6]{240, 540, 13, 90, 240, 540 },
            new int[6]{180, 720, 6, 200, 180, 720 },
            new int[6]{120, 780, 6, 400, 120, 780 }
        };

        public void RunTest() {
            for (int caseIndex = 1; caseIndex < 2/*TestInstances.Length*/; caseIndex++) {
                string caseName = TestInstances[caseIndex];

                //caseName = "SmallCaseSet\\small01";

                string input_dir = @".\DATA\" + caseName + "\\";                 
                                
                for (int windowIndex = 0; windowIndex < 1/*FixedMealWindowsDict.Count*/; windowIndex++) {
                    KeyValuePair<string, FixedMealWindow> windowNameToParam =
                        FixedMealWindowsDict.ElementAt(windowIndex);
                    string meal_window = windowNameToParam.Key;
                    FixedMealWindow curMealWindow = windowNameToParam.Value;
                    string output_dir = @".\12算例结果\" + caseName + "\\" + meal_window + "\\";
                    
                    Console.WriteLine("**********START TEST CASE [{0}] CONSIDERING MEAL TIME WINDOW [{1}]**********", 
                        caseName, meal_window);

                    Stopwatch solveTimer = new Stopwatch();
                    solveTimer.Start();
                    Stopwatch netTimer = new Stopwatch();
                    netTimer.Start();
                    
                    //CreateNetWork net = new CreateNetWork();
                    CreateNetWork_db net = new CreateNetWork_db();

                    net.LoadData_csv(1,
                        input_dir + "Timetable.csv",
                        input_dir + "CrewBase.csv",
                        input_dir + "Crew.csv",
                        input_dir + "Station.csv");

                    net.SetBasicTimeRules(crewRules[caseIndex][0], crewRules[caseIndex][1],
                        crewRules[caseIndex][2], crewRules[caseIndex][3],
                        crewRules[caseIndex][4], crewRules[caseIndex][5]);
                    net.ShowBasicTimeRules();
                    //net.CreateT_S_NetWork();
                    net.CreateT_S_NetWork_db();

                    net.SetMealWindows(curMealWindow.lunch_start, curMealWindow.lunch_end,
                        curMealWindow.supper_start, curMealWindow.supper_end);

                    //net.SetMealTime(30, 40);
                    net.SetMealTime(InstanceMealSpan[caseName][0], 40);
                    if (meal_window == "INF_h") { //不考虑时间窗，则用餐时间设为0
                        net.minMealTime = 0;
                    }
                    net.SetVirRoutingCost(1000);//(InstanceVirRoutingCostMap[caseName]);

                    //net.CreateT_S_S_NetWork(net_params[caseIndex][0], net_params[caseIndex][1],
                    //    net_params[caseIndex][2], net_params[caseIndex][3],
                    //    net_params[caseIndex][4], net_params[caseIndex][5],
                    //    net_params[caseIndex][6], net_params[caseIndex][7],
                    //    net_params[caseIndex][8], net_params[caseIndex][9]);

                    //net.SetBasicTimeRules(20, 450, 15, 400, 20, 450); //smallcase01

                    //net.SetArcPenaltyMealViolate(crewRules[caseIndex][5]);
                    net.SetArcPenaltyMealViolate(1400);

                    net.CreateT_S_S_Network_db();

                    netTimer.Stop();

                    //StreamWriter logFile = new StreamWriter(output_dir + "stateNodeSet_db.csv");
                    //logFile.WriteLine("ID,NodeType,OStation,LineID,TrainCode,Station,TimeCode,SuperPointID,SuperPointType,PrevNodeID," +
                    //    "DriveTime,DayCrewTime,Lunch,Dinner," +
                    //    "PassLines,PassStateNodes");
                    //foreach (var node in net.T_S_S_NodeList) {
                    //    logFile.Write(Logger.stateNodeInfoToStr(node, OutputMode.file));
                    //    logFile.Write(",");
                    //    logFile.WriteLine(Logger.stateNodePartialPathToStr(node, OutputMode.file));
                    //    //Console.WriteLine(Logger.stateNodePartialPathToStr(node, OutputMode.console));
                    //}
                    //logFile.Close();

                    Console.WriteLine("state arc num: {0}", net.T_S_S_ArcList.Count);
                    Console.WriteLine("state node num: {0}", net.T_S_S_NodeList.Count);


                    bool isnotcontain = true;
                    isnotcontain = net.AllLineContain();
                    if (isnotcontain == false) {
                        CGandLR LR_CG = new CGandLR();
                        LR_CG.InitLR(net, InstanceStepSize[caseName]);
                        //LR_CG.InitLR(net, 100);
                        Lagrange lag = LR_CG.LR;

                        LR_CG.InitCG(net);

                        LR_CG.LR_and_CG(ref net, InstanceLRIteration[caseName]);

                        solveTimer.Stop();

                        Summery summery = new Summery(lag, net, netTimer.Elapsed.TotalSeconds, solveTimer.Elapsed.TotalSeconds);
                        saveSoln(output_dir, summery, lag, 1000/*InstanceVirRoutingCostMap[caseName]*/);

                        Logger.GetScheduleForVisualize(LR_CG.GetOptPairingSet(), output_dir,caseName,meal_window);
                    }
                    else {
                        throw new Exception("*************current network is not covered all lines, there might be some errors!!!*************\n");
                    }

                    Console.WriteLine("total time spended in solve this case is {0} s", solveTimer.Elapsed.TotalSeconds);
                    Console.WriteLine("**********END TEST CASE [{0}] CONSIDERING MEAL TIME WINDOW [{1}]", caseName, windowIndex);
                }
            }
        }

        public void saveSoln(string caseDir, Summery summery, Lagrange lag, int vitualRoutingValue) {
            //保存文件
            StreamWriter file_schedule = new StreamWriter(caseDir
                + "交路内容与求解信息_" + lag.step_size + "_virRoutingVal_" + vitualRoutingValue + ".txt");
            StreamWriter file_iteration = new StreamWriter(caseDir
                + "iterationBound" + lag.step_size + "_virRoutingVal_" + vitualRoutingValue + ".csv");

            file_schedule.WriteLine(summery.outputStr());
            file_schedule.Close();

            string LB_UB = "";
            file_iteration.WriteLine("CurLB_with,CurLB_without,CurUB_with");
            for (int i = 0; i < lag.CurLB.Count; i++) {
                LB_UB = "";
                LB_UB = i < lag.CurUB.Count ?
                    Convert.ToString(lag.CurLB[i]) + "," + Convert.ToString(lag.CurLB_without_penalty[i])
                        + "," + Convert.ToString(lag.CurUB[i])
                    : Convert.ToString(lag.CurLB[i]) + "," + Convert.ToString(lag.CurLB_without_penalty[i]);

                file_iteration.WriteLine(LB_UB);// + "," + lag.CurUB[i]);
            }
            file_iteration.Close();            
        }        
    }

  
}
