/* ==============================================================================
 * 功能描述：CG
 * 创 建 者：Administrator
 * 创建日期：2019/5/22 星期三 下午 21:50:52
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ILOG.Concert;
using ILOG.CPLEX;



namespace Crew_Planning
{
    public class CGandLR
    {
        //part of CG
        public Cplex RMP;
        //public IObjective Obj;
        public List<INumVar> DvarSet;
        public List<int[]> CoverMatrix;

        IRange[] constraints;
        public double[] DualSet;
        public double RMPObjValue;
        //end part of CG
        
        //part of LR
        public Lagrange LR;


        //end part of CG

        public double GAP = 0.01;
        public List<Pairing> ColumnPool;

        private List<Pairing> OptSoln = new List<Pairing>();

        private int num_task;

        public void InitCG(CreateNetWork net)
        {
            num_task = net.LineList.Count;
            RMP = new Cplex();           
            DvarSet = new List<INumVar>(net.CrewList.Count);
            CoverMatrix = new List<int[]>();
            DualSet = new double[num_task];
            constraints = new IRange[num_task];
            ColumnPool = new List<Pairing>();
        }
        public void InitCG(CreateNetWork_db net) {
            num_task = net.LineList.Count;
            RMP = new Cplex();
            DvarSet = new List<INumVar>(net.CrewList.Count);
            CoverMatrix = new List<int[]>();
            DualSet = new double[num_task];
            constraints = new IRange[num_task];
            ColumnPool = new List<Pairing>();
        }

        public void InitLR(CreateNetWork net, int stepSize)
        {
            LR = new Lagrange(net, stepSize);
        }
        public void InitLR(CreateNetWork_db net, int stepSize) {
            LR = new Lagrange(net, stepSize);
        }

        double GetRMPObjValue(List<Pairing> pathSet)
        {
            double stayPath_Value = 0;
            for (int i = pathSet.Count - 1; i >= 0; i--)
            {
                if (pathSet[i].Route.Count <= 2)
                {
                    stayPath_Value += pathSet[i].Cost_with_penalty;
                }
            }

            RMPObjValue = RMP.GetObjValue() + stayPath_Value;
            return RMPObjValue;
        }

        public void BuildRMP(List<Pairing> initialPathSet)
        {
            //GetCoverMatrix()

            INumExpr obj_expr = RMP.NumExpr();
            foreach (Pairing path in initialPathSet)
            {
                if (path.Route.Count <= 2)
                {
                    continue;
                }

                ColumnPool.Add(path);
                //create obj function
                INumVar x = RMP.NumVar(0, 1, NumVarType.Float);
                DvarSet.Add(x);
                obj_expr = RMP.Sum(obj_expr, RMP.Prod(path.Cost_with_penalty, x));

                CoverMatrix.Add(path.CoverAaray);
                
            }
            RMP.AddObjective(ObjectiveSense.Minimize, obj_expr);
            //s.t
            for (int i = 0; i < num_task; i++)
            {                
                INumExpr ct = RMP.NumExpr();
                for (int j = 0; j < DvarSet.Count; j++)
                {
                    ct = RMP.Sum(ct,
                        RMP.Prod(CoverMatrix[j][i], DvarSet[j]));
                }
                constraints[i] = RMP.AddGe(ct, 1);
            }

        }

        public void LR_and_CG(ref CreateNetWork net, int lr_iter_num)
        {
            //1.initial solution            
            LR.GenerateLRSoln(ref net, lr_iter_num);
            double lr_lb = LR.BestLB;
            double ub = 0;
            //2.RMP
            //BuildRMP(LR.SortedPathSet);
            //AddColumnsToRMP(LR.LB_PathSet);
            BuildRMP(LR.g_LB_PathSet);
            
            //double real_LB = 0;
            //double real_UB = 0;

            #region //CG progress changed 20191026
            
            //for (; ; )
            //{
            //    try
            //    {
            //        RMP.Solve();
            //        //Console.WriteLine(GetRMPObjValue(LR.SortedPathSet));
            //        RMPObjValue = RMP.GetObjValue();
            //        Console.WriteLine("RMPOBJ:" + RMPObjValue);
            //        //3.get dual:v
            //        GetDuals();
            //    }
            //    catch (ILOG.Concert.Exception iloex)
            //    {
            //        Console.WriteLine(iloex.Message);
            //    }
            //    //4.renew LR multipliers u //2 way:1) u = a*v + u_best of last LR solve process
            //    double weight = 0.05;// 0.05;
            //    List<Line> lineList = net.LineList;
            //    List<T_S_S_Arc> taskArcSet = LR.type_arcPair[1];
            //    RenewLRMultipliers(ref lineList, ref taskArcSet, weight);

            //    real_LB = LR.BestLB;
            //    real_UB = LR.BestUB;
            //    foreach (var path in LR.LB_PathSet)
            //    {
            //        if (path.Route.Count <= 2)
            //        {
            //            real_LB -= path.Price;
            //        }
            //    }
            //    foreach (var path in LR.SortedPathSet)
            //    {
            //        if (path.Route.Count <= 2)
            //        {
            //            real_UB -= path.Price;
            //        }
            //    }

            //    Console.WriteLine("real_LB: {0} \t real_UB: {1}\n", real_LB, real_UB);
            //    //5.gap
            //    if (CheckTwoGAP(real_LB, real_UB, RMPObjValue) || num_iter >= 100)
            //    {
            //        //got best solution

            //        break;
            //    }
            //    //6.LR loop,either for some short path,not all Np numbers, or all Np short path                         
            //    //7.found no path with negetive reduced cost,then
            //    //renew LR multipliers by renew weight
            //    num_iter += 20;
            //    LR.GenerateLRSoln(ref net, num_iter);
            //    while (LR.SortedPathSet[0].Price >= 0 && weight < 1)
            //    {
            //        Console.WriteLine("!!!!");
            //        weight += 0.1;
            //        RenewLRMultipliers(ref lineList, ref taskArcSet, weight);
            //        num_iter += 20;
            //        LR.GenerateLRSoln(ref net, num_iter);
            //    }
            //    if (weight >= 1)
            //    {
            //        //got best solution
            //        break;
            //    }
            //    //8.if found path with negetive reduced cost,then
            //    //add to RMP
            //    AddColumnsToRMP(LR.SortedPathSet);
            //    AddColumnsToRMP(LR.LB_PathSet);
            //}

            #endregion
            
            //求整数解
            IConversion mip = RMP.Conversion(DvarSet.ToArray(), NumVarType.Int);
            RMP.Add(mip);
            RMP.Solve();
            Console.WriteLine("MIP_OBJ:" + RMP.GetObjValue());

            int real_crew_num = 0;
            //ub = calLRUB_with_and_without_penalry(net.virtualRoutingCost, ref real_crew_num);
            Console.WriteLine("ub:" + ub);
            Console.WriteLine("real crew num:" + real_crew_num);


            //******stop when met stop-condition
            LR.BestUB = ub;
            setOptSoln(net.LineList);
            sortOptSolnByASC();
            getOptSolnContent(net);

        }
        public void LR_and_CG(ref CreateNetWork_db net, int lr_iter_num) {
            //1.initial solution            
            LR.GenerateLRSoln(ref net, lr_iter_num);
            double lr_lb = LR.BestLB;
            double ub_with = 0;
            double ub_without = 0;
            //2.RMP
            //BuildRMP(LR.SortedPathSet);
            //AddColumnsToRMP(LR.LB_PathSet);
            BuildRMP(LR.g_LB_PathSet);

            
            //求整数解
            IConversion mip = RMP.Conversion(DvarSet.ToArray(), NumVarType.Int);
            RMP.Add(mip);            

            RMP.Solve();
            Console.WriteLine("MIP_OBJ:" + RMP.GetObjValue());

            int real_crew_num = 0;
            calLRUB_with_and_without_penalry(net.virtualRoutingCost, ref real_crew_num,
                ref ub_with, ref ub_without);
            Console.WriteLine("ub_with:{0}, ub_without:{1}", ub_with, ub_without);
            Console.WriteLine("real crew num:" + real_crew_num);


            //******stop when met stop-condition
            //LR.BestUB = ub;
            LR.BestUB_with_penalty = ub_with;
            LR.BestUB_without_penalty = ub_without;
            setOptSoln(net.LineList);
            sortOptSolnByASC();
            getOptSolnContent(net);

        }
        private void calLRUB_with_and_without_penalry(int virRoutingCost, ref int realCrewNum,
            ref double ub_with_penalty, ref double ub_without_penalty) {
            
            int count = 0;
            double[] x_value = RMP.GetValues(DvarSet.ToArray());
            //List<Pairing> cur_feasible_soln = new List<Pairing>();

            for (int i = 0; i < x_value.Length; i++) {
                if (x_value[i] > 0) {
                    ub_with_penalty += ColumnPool[i].Cost_with_penalty;
                    ub_without_penalty += ColumnPool[i].Cost_without_penalty;
                    ++count;
                }
            }
            realCrewNum = count;
            for (; count < LR.FixedCrewNum; ++count) {
                ub_with_penalty += virRoutingCost;
                ub_without_penalty += virRoutingCost;
            }            
        }

        void GetDuals()
        {
            DualSet = RMP.GetDuals(constraints);
        }
        void RenewLRMultipliers(ref List<Line> LineList, ref List<T_S_S_Arc> taskArcSet, double weight)
        {
            for (int i = 0; i < LineList.Count; i++)
            {
                LineList[i].LagMultiplier = weight * DualSet[i] + (1 - weight) * LineList[i].LagMultiplier;
                //LineList[i].LagMultiplier = DualSet[i];
            }

            foreach (var taskarc in taskArcSet)
            {
                int lineId = taskarc.LineID;
                
                taskarc.LagMultiplier = LineList[lineId-1].LagMultiplier;                
            }


        }
        bool CheckTwoGAP(double LB,double UB, double ObjValue_RMP)
        {
            //double ratio = Math.Min((UB - LB) / UB, (ObjValue_RMP - LB) / ObjValue_RMP);
            double ratio = (UB - LB) / UB;
            return ratio <= GAP;
        }

        void AddColumnsToRMP(List<Pairing> newPaths)
        {            
            foreach (var path in newPaths)
            {
                if (path.Price > 0 && checkExsitColumn(ColumnPool, path))
                {
                    continue;
                }

                ColumnPool.Add(path);

                INumVar new_col = RMP.NumVar(0, 1, NumVarType.Float);
                //renew obj
                RMP.GetObjective().Expr = RMP.Sum(RMP.GetObjective().Expr,
                                                  RMP.Prod(path.Cost_with_penalty, new_col)); 
                //renew ct
                for (int i = 0; i < num_task; i++)
                {
                    constraints[i].Expr = RMP.Sum(constraints[i].Expr,
                        RMP.Prod(path.CoverAaray[i], new_col));
                }

                DvarSet.Add(new_col);

            }
        }
        bool checkExsitColumn(List<Pairing> columnPool, Pairing path)
        {
            bool isExsit = false;
            for (int i = columnPool.Count - 1; i >= 0; i--)
            {
                if (path.equals(columnPool[i]))
                {
                    isExsit = true;
                    break;
                }
            }
            return isExsit;
        }
       
        private void setOptSoln(List<Line> allLineSet) {
            double[] x_value = RMP.GetValues(DvarSet.ToArray());
            for (int i = 0; i < x_value.Length; i++) {
                if (x_value[i] > 0) {
                    ColumnPool[i].fillTripList(allLineSet);
                    OptSoln.Add(ColumnPool[i]);
                }
            }
        }
        private void sortOptSolnByASC() {
            OptSoln.Sort(PairingContentASC.PairingASC);
        }
        private void getOptSolnContent(CreateNetWork net) {
            foreach (var pairing in OptSoln) {
                string str = "";
                double T = 0, dis = 0;
                LR.TranslationCSP(pairing.Route, net, ref str, ref T, ref dis, 0, 0);
                LR.YUB.Add(str);
            }
        }
        private void getOptSolnContent(CreateNetWork_db net) {
            foreach (var pairing in OptSoln) {
                string str = "";
                double T = 0, dis = 0;
                LR.TranslationCSP(pairing.Route, net, ref str, ref T, ref dis, 0, 0);
                LR.YUB.Add(str);
            }
        }
        public List<Pairing> GetOptPairingSet() {
            return OptSoln;
        }

    }
}
