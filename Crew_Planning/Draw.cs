using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Collections;

namespace Crew_Planning
{
    public class Draw
    {
        public void DrawCrewGap(Graphics g,int panelwidth,int panelheight,List<double> CurLB, List<double> CurUB,double Time) //
        {
            //颜色
            Color clb = Color.SkyBlue;
            Color cub = Color.Red;
            Color c = Color.Black;//书写文字数字、画坐标轴需要的颜色
            //填充色
            Brush blb = new SolidBrush(clb);
            Brush bub = new SolidBrush(cub);
            Brush b = new SolidBrush(c);
            //画笔
            Pen penlb = new Pen(clb, 2);
            penlb.DashStyle = DashStyle.Dash;//下界虚线
            Pen penub = new Pen(cub, 2);
            penub.DashStyle = DashStyle.Solid;//上界实线
            Pen pencoor = new Pen(c, 1);
            pencoor.EndCap = LineCap.ArrowAnchor;//坐标轴的箭头线
            Pen penstring = new Pen(c, 2);
            //字体
            Font fontstr = new Font("黑体", 10);
            Font fontnum = new Font("宋体", 8);
            //画坐标轴
            int xlength = panelwidth - 100;
            int ylength = panelheight - 30;
            PointF ohoriandver = new PointF (100,ylength);//原点
            PointF dhori = new PointF (xlength,ylength);//横轴终点
            PointF dver = new PointF (100,30);//纵轴重点
            g.DrawLine(pencoor, ohoriandver, dhori);
            g.DrawLine(pencoor, ohoriandver, dver);
            //写横轴数字、文字
            //确定迭代次数
            int kx = CurLB.Count;
            //int kx = 239;
            //横轴单位标注长度 //横轴长horilength-100,-50为了美观
            float xstep = (xlength - 100 - 50) / kx;
            int ix = 0, nx =20;//想标注数字的最大个数（0除外）
            int xnumstep = (Convert.ToInt32(kx / nx / 10+1))*10;//标注数值间的间隔，10代表间隔数量级
            while (ix <= kx/xnumstep)
            {
                g.DrawString((ix * xnumstep).ToString(), fontstr, b, 100 + (xstep * xnumstep * ix), ylength + 2);
                ix++;
            }
            g.DrawString("迭代次数/k", fontstr, b, xlength+20, ylength + 2);
            //写纵轴数字、文字
            double max = -100000000000000;//, min = 100000000000000.0
            //double max = 762.56, min = 1356.32;
            int i = 0;
            for (i = 0; i < CurLB.Count; i++)
            {
                if (CurLB[i]> max) { max = CurLB[i]; }
            }
            for (i = 0; i < CurUB.Count; i++)
            {
                if (CurUB[i] > max) { max = CurUB[i]; }
            }
            float ky =(float)max/1000;
            float ystep = (ylength - 30 - 50) / ky;//单位标注长度，即一个乘务组所代表的长度
            int iy = 1, ny = 10;//想标注数字的最大个数（0除外）
            int ynumstep = (Convert.ToInt32(ky/ny/1+1))*1;//1代表相邻数字间的差值
            while (iy <= (ky / (ynumstep)))
            {
                g.DrawString((iy * ynumstep).ToString(), fontstr, b, 40, ylength-(ystep*ynumstep*iy));
                iy++;
            }
            g.DrawString("(目标值/1440)/乘务组数", fontstr, b, 30, 10);
            g.DrawString("求解时间为：" + Time.ToString("f2")+"s", fontstr, b, xlength-50, 10);
            PointF[] PLB = new PointF[CurLB.Count];
            PointF[] PUB = new PointF[CurUB.Count];
            float yvalue = 0f;
            for(i=0;i<CurLB.Count;i++)
            {
                yvalue = (float)(ylength - ystep * (CurLB[i]/1440));
                PLB[i] = new PointF(i * xstep+100, yvalue);
            }
            for (i = 0; i < CurUB.Count; i++)
            {
                yvalue = (float)(ylength - ystep * (CurUB[i]/1440));
                PUB[i] = new PointF(i * xstep+100, yvalue);
            }
            g.DrawLines(penlb, PLB);
            g.DrawLines(penub, PUB);
            //g.Dispose();
        }
    }
    public class WriteIn
    {
        public DataTable Dt = new DataTable();
        public DataRow row; 
        public DataTable SaveInDtCurBest(List<double> CurBestLB, List<double> CurBestUB)
        {
            DataTable Dt = new DataTable();
            DataRow row;
            int i=0;
            Dt.Columns.Add(new DataColumn("CurBestLB", typeof(double)));
            Dt.Columns.Add(new DataColumn("CurBestUB", typeof(double)));
            for(i=0;i<CurBestLB.Count;i++)
            {
                row = Dt.NewRow();
                row["CurBestLB"] = CurBestLB[i];
                row["CurBestUB"] = CurBestUB[i];
                Dt.Rows.Add(row);
            }
            return Dt;
        }
        public DataTable SaveInDtCur(List<double> CurLB, List<double> CurUB)
        {
            DataTable Dt = new DataTable();
            DataRow row;
            int i = 0;
            Dt.Columns.Add(new DataColumn("CurLB", typeof(double)));
            Dt.Columns.Add(new DataColumn("CurUB", typeof(double)));
            for (i = 0; i < CurLB.Count; i++)
            {
                row = Dt.NewRow();
                row["CurLB"] = CurLB[i];
                row["CurUB"] = CurUB[i];
                Dt.Rows.Add(row);
            }
            return Dt;
        }
        public void WtiteInExcelCurBest(List<double> LB, List<double> UB, string fullPath)
        {
            DataTable dt = new DataTable();
            dt = SaveInDtCurBest(LB, UB);
            FileInfo fi = new FileInfo(fullPath);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            FileStream fs = new FileStream(fullPath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            //StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            string data = "";
            //写出列名称
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                data += dt.Columns[i].ColumnName.ToString();
                if (i < dt.Columns.Count - 1)
                {
                    data += ",";
                }
            }
            sw.WriteLine(data);
            //写出各行数据
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                data = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string str = dt.Rows[i][j].ToString();
                    str = str.Replace("\"", "\"\"");//替换英文冒号 英文冒号需要换成两个冒号
                    if (str.Contains(',') || str.Contains('"')
                       || str.Contains('\r') || str.Contains('\n')) //含逗号 冒号 换行符的需要放到引号中
                    {
                        str = string.Format("\"{0}\"", str);
                    }

                    data += str;
                    if (j < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
            }
            sw.Close();
            fs.Close();
        }
        public void WtiteInExcelCur(List<double> LB, List<double> UB, string fullPath)
        {
            DataTable dt = new DataTable();
            dt = SaveInDtCur(LB, UB);
            FileInfo fi = new FileInfo(fullPath);
            if (!fi.Directory.Exists)
            {
                fi.Directory.Create();
            }
            FileStream fs = new FileStream(fullPath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            //StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            string data = "";
            //写出列名称
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                data += dt.Columns[i].ColumnName.ToString();
                if (i < dt.Columns.Count - 1)
                {
                    data += ",";
                }
            }
            sw.WriteLine(data);
            //写出各行数据
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                data = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string str = dt.Rows[i][j].ToString();
                    str = str.Replace("\"", "\"\"");//替换英文冒号 英文冒号需要换成两个冒号
                    if (str.Contains(',') || str.Contains('"')
                       || str.Contains('\r') || str.Contains('\n')) //含逗号 冒号 换行符的需要放到引号中
                    {
                        str = string.Format("\"{0}\"", str);
                    }

                    data += str;
                    if (j < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
            }
            sw.Close();
            fs.Close();
        }
    }
}
