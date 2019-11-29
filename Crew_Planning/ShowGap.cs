using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crew_Planning
{
   
    public partial class ShowGap : Form
    {
        //Draw draw = new Draw();
        public int width;
        public int height;
        public Graphics g;
        public Draw draw=new Draw ();
        List<double>CurLB=new List<double> ();
        List<double> CurUB=new List<double> ();
        public ShowGap()
        {
            InitializeComponent();
        }
        public void Draw(List<double>LB,List<double>UB)
        {

            CurLB = LB;
            CurUB = UB;
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            g=e.Graphics;
            draw.DrawGap(g, width, height, CurLB, CurUB);
            //Graphics g = e.Graphics;//Graphics是e的属性而不是方法。
            //List<double> CurLB  =new List<double>();
            //List<double> CurUB = new List<double>();
            //int i = 0;
            //for (i=0;i<70;i++)
            //{
            //    CurLB.Add( 1000 + 10 * i);
            //}
            //for (i =70; i < 108; i++)
            //{
            //    CurLB.Add( 1800);
            //}
            //for (i = 0; i < 70; i++)
            //{
            //    CurUB.Add(3000 - 15 * i);
            //}
            //for (i = 70; i < 108; i++)
            //{
            //    CurUB.Add(1850);
            //}

            //int panelwidth = this.panel1.Width;
            //int panelheight = this.panel1.Height;
            //draw.DrawGap(g, panelwidth, panelheight,CurLB,CurUB);
        }

        private void ShowGap_Load(object sender, EventArgs e)
        {

        }
        public void Createg()
        {
            g = panel1.CreateGraphics();
            width = panel1.Width;
            height = panel1.Height;
        }
    }
}
