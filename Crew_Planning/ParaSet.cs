using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Crew_Planning
{
    public partial class ParaSet : Form
    {
        CreateNetWork net = new CreateNetWork();
        
        DataSet Ds = new DataSet();
        Lagrange lag; 
        
        ShowGap_Cur showgap_cur = new ShowGap_Cur();
        ShowGap_CurBest showgap_curbest = new ShowGap_CurBest();
        WriteIn WI = new WriteIn();
        public double time = 0.0;
        public double time_Net = 0.0;
        public DateTime StartTime = new DateTime();
        public DateTime EndTime = new DateTime();
        public DateTime EndTime_Net = new DateTime();
        public string savepath;
        public string str;
        public ParaSet()
        {
            InitializeComponent();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void radioButtonk_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButtonk.Checked==true)
            {
                labelend.Text = "请输入跌代次数k值";
            }
        }

        private void radioButtonGap_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButtonGap.Checked==true)
            {
                labelend.Text = "请输入上下界之差Gap值";
            }
        }

        private void buttonok_Click(object sender, EventArgs e)
        {   
            int maxdrivetime=Convert.ToInt32(textBoxmaxdrive.Text);
            int mindrivetime=Convert.ToInt32(textBoxmindrive.Text);
            int maxdaycrewtime = Convert.ToInt32(textBoxmaxdaycrew.Text);
            int mindaycrewtime = Convert.ToInt32(textBoxmindaycrew.Text);
            int maxconntime=Convert.ToInt32(textBoxconn.Text);
            int mintranslation=Convert.ToInt32(textBoxtran.Text);
            int minrelaxtime=Convert.ToInt32(textBoxminrelax.Text);
            int maxrelaxtime=Convert.ToInt32(textBoxmaxrelax.Text);
            int minoutrelaxtime=Convert.ToInt32(textBoxminoutrelax.Text);
            int maxoutrelaxtime = Convert.ToInt32(textBoxmaxoutrelax.Text);
            int days = Convert.ToInt32(textBoxdays.Text);
            int step = Convert.ToInt32(textBoxstep.Text);

            
            List<int[]> windows = new List<int[]>();
            //STLunch = 660; ETLunch = 780; STDinner = 1020; ETDinner = 1140; //2h
            //int STLunch = 630, ETLunch = 810, STDinner = 990, ETDinner = 1170; //3h
            windows.Add(new int[4] { 660, 780, 1020, 1140 });//2h
            windows.Add(new int[4] { 630, 810, 990, 1170 });//3h
            windows.Add(new int[4] { 600, 840, 960, 1200 }); //4h
            //windows.Add(new int[4] {480,   840,   900,   1260}); //不考虑 windows})
            windows.Add(new int[4] { 4000, 8400, 9600, 14400 });
            //windows.Add(new int[4] { 3000, 7200, 7210, 28800 });//INF

            string[] Cur_case = { "case21\\", "case22\\", "case33\\", "case34\\" };
            

            for (int j = 0; j < windows.Count - 3 ; j++)
            {
                #region MyRegion
                Stopwatch realTime = new Stopwatch();
                realTime.Restart();

                str = "";
                
                CGandLR LR_CG = new CGandLR();                

                StartTime = DateTime.Now;
                int sh = StartTime.Hour;
                int sm = StartTime.Minute;
                int ss = StartTime.Second;
                int sms = StartTime.Millisecond;
                int sa = (sh * 60 + sm) * 60 + ss;
                double starttime = sa + 0.001 * sms;
                //Ds = net.ConnDataBase();
                //net.LoadData(Ds,days);
                net.LoadData_csv(days,
                    "..\\DATA\\沪杭\\Timetable.csv",
                    "..\\DATA\\沪杭\\CrewBase.csv",
                    "..\\DATA\\沪杭\\Crew.csv",
                    "..\\DATA\\沪杭\\Station.csv");
                net.CreateT_S_NetWork();

                net.SetMealWindows(windows[j][0], windows[j][1], windows[j][2], windows[j][3]);

                //net.CreateT_S_S_NetWork(mindrivetime,maxdrivetime,maxconntime,mintranslation,minrelaxtime, maxrelaxtime,minoutrelaxtime,maxoutrelaxtime, mindaycrewtime, maxdaycrewtime);//daycrewtime-120出退乘
                //net.CreateT_S_S_NetWork(180, 250, 40, 15, 3312, 3330, 3400, 3900, 180, 250);//京津城际实例
                net.CreateT_S_S_NetWork(240, 540, 90, 13, 3312, 3300, 3400, 3900, 240, 540);//沪宁杭
                //net.CreateT_S_S_NetWork(180, 720, 200, 6, 3312, 3330, 600, 2800, 180, 720);//bigscale
                //net.CreateT_S_S_NetWork(120, 780, 400, 6, 3312, 3330, 540, 2880, 120, 780);//bigscale2

                //lag = new Lagrange(net, 350);
                LR_CG.InitLR(net, 350);
                lag = LR_CG.LR;
                LR_CG.InitCG(net);

                string case_dir = System.Environment.CurrentDirectory + 
                    "\\12算例结果\\new_test\\沪杭\\" + Cur_case[j];
                
                StreamWriter file_1 = new StreamWriter(case_dir + 
                    "交路内容与求解信息" + lag.step_size + "_M3_(1000)_in==0_nosppedLB.txt", false, Encoding.Default);
                StreamWriter file_2 = new StreamWriter(case_dir +
                    "iterationBound" + lag.step_size + "_M3_(1000)_in==0_nosppedLB.csv", false, Encoding.Default);
                StreamWriter file_3 = new StreamWriter(case_dir +
                    "convergeBound" + lag.step_size + "_M3_(1000)_in==0_nosppedLB.csv", false, Encoding.Default);

                Console.WriteLine("solve case: " + Cur_case[j]);
                Console.WriteLine("state arc number: " + net.T_S_S_ArcList.Count);
                Console.WriteLine("state node number: " + net.T_S_S_NodeList.Count);

                
                EndTime_Net = DateTime.Now;
                int eh_Net = EndTime_Net.Hour;
                int em_Net = EndTime_Net.Minute;
                int es_Net = EndTime_Net.Second;
                int ems_Net = EndTime_Net.Millisecond;
                int ea_Net = (eh_Net * 60 + em_Net) * 60 + es_Net;
                double endtime_Net = ea_Net + 0.001 * ems_Net;
                time_Net = endtime_Net - starttime;
                Console.WriteLine("create net spend time: {0} s", time_Net);

                bool isnotcontain = true;
                isnotcontain = net.AllLineContain();
                if (isnotcontain == false)//|| isnotcontain)
                {
                    //if (radioButtonk.Checked == true)
                    //{
                    //int K = 100;// Convert.ToInt32(textBoxend.Text);
                    //lag.k = 0;
                    //lag.IterationK(ref net, K);
                    //}
                    LR_CG.LR_and_CG(ref net, 10);

                    if (radioButtonGap.Checked == true)
                    {
                        double Gap = Convert.ToDouble(textBoxend.Text);

                        //lag.IterationGap(ref net, Gap);
                    }
                    if (radioButtonUB.Checked == true)
                    {
                        int N = Convert.ToInt32(textBoxend.Text);
                        //lag.IterationUBStay(ref net, N);
                    }

                    EndTime = DateTime.Now;
                    int eh = EndTime.Hour;
                    int em = EndTime.Minute;
                    int es = EndTime.Second;
                    int ems = EndTime.Millisecond;
                    int ea = (eh * 60 + em) * 60 + es;
                    double endtime = ea + 0.001 * ems;
                    time = endtime - starttime;

                    realTime.Stop();

                    Console.WriteLine("{0} spend time: {1} s", Cur_case[j], realTime.Elapsed.TotalSeconds);
                    Console.WriteLine("calculate time(except net construction time): {0} s"
                        , realTime.Elapsed.TotalSeconds - time_Net);

                    if (checkCur.Checked == true && checkCurBest.Checked == false)
                    {
                        showgap_cur.Draw(lag.CurLB, lag.CurUB, time);
                        showgap_cur.Show();
                    }
                    else if (checkCur.Checked == false && checkCurBest.Checked == true)
                    {
                        showgap_curbest.Draw(lag.CurBestLB, lag.CurBestUB, time);
                        showgap_curbest.Show();
                    }
                    else if (checkCur.Checked == true && checkCurBest.Checked == true)
                    {
                        showgap_cur.Draw(lag.CurLB, lag.CurUB, time);
                        showgap_curbest.Draw(lag.CurBestLB, lag.CurBestUB, time);
                        showgap_curbest.Show();
                        showgap_cur.Show();
                    }

                    //保存文件
                    int i = 0;
                    for (i = 0; i < lag.YUB.Count; i++)
                    {
                        str += "乘务交路" + (i + 1).ToString() + "为：" + lag.YUB[i] + "\r\n";
                    }
                    double gap = (lag.BestUB - lag.BestLB) / lag.BestUB;
                    str += "BestLB为：" + lag.BestLB.ToString("f2") + "\r\n";
                    str += "BestUB为：" + lag.BestUB.ToString("f2") + "\r\n";
                    str += "迭代次数K为：" + lag.k.ToString() + "\r\n";
                    str += "Gap为：" + gap.ToString("f4") + "\r\n";
                    str += "建网时间为：" + time_Net.ToString("f2") + "s" + "\r\n";
                    str += "state arc number: " + net.T_S_S_ArcList.Count + "\r\n";
                    str += "state node number: " + net.T_S_S_NodeList.Count + "\r\n";
                    str += "求解时间为：" + time.ToString("f2") + "s" + "\r\n";
                    str += "real求解时间为：" + realTime.Elapsed.TotalSeconds.ToString("f2") + "s" + "\r\n";
                    str += "决策变量个数为：" + net.T_S_S_ArcList.Count * lag.CrewCount;

                    file_1.WriteLine(str);
                    file_1.Close();

                    string LB_UB = "";
                    file_2.WriteLine("CurLB,CurUB");
                    for (i = 0; i < lag.CurLB.Count; i++)
                    {
                        LB_UB = "";
                        LB_UB = i < lag.CurUB.Count ? 
                            Convert.ToString(lag.CurLB[i]) + "," + Convert.ToString(lag.CurUB[i]) 
                            : Convert.ToString(lag.CurLB[i]); 
                        file_2.WriteLine(LB_UB);// + "," + lag.CurUB[i]);
                    }
                    file_2.Close();

                    file_3.WriteLine("CurBestLB,CurBestUB");
                    for (i = 0; i < lag.CurBestLB.Count; i++)
                    {
                        LB_UB = "";
                        LB_UB = i < lag.CurBestUB.Count ?
                            Convert.ToString(lag.CurBestLB[i]) + "," + Convert.ToString(lag.CurBestUB[i])
                            : Convert.ToString(lag.CurBestLB[i]);
                        file_3.WriteLine(LB_UB);// + "," + lag.CurUB[i]);
                    }
                    file_3.Close();

                }

                Console.WriteLine("{0} 求解完成 ", Cur_case[j]);
                #endregion
            }
           
            MessageBox.Show("保存成功！", "please close",
                MessageBoxButtons.OK,
                MessageBoxIcon.Stop,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.ServiceNotification);
        }

        private void textBoxend_TextChanged(object sender, EventArgs e)
        {
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonUB.Checked == true)
            {
                labelend.Text = "请输入次数n值";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBoxconn_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxmindrive_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxmindaycrew_TextChanged(object sender, EventArgs e)
        {

        }

        private void ParaSet_Load(object sender, EventArgs e)
        {

        }
    }
}