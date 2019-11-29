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
   
    public partial class ShowGap_Cur : Form
    {
        public int width;
        public int height;
        public Graphics g;
        public Draw draw=new Draw ();
        List<double>CurLB=new List<double> ();
        List<double> CurUB=new List<double> ();
        public double Time = 0.0;
        public ShowGap_Cur()
        {
            InitializeComponent();
        }
        public void Draw(List<double>LB,List<double>UB,double time)
        {

            CurLB = LB;
            CurUB = UB;
            Time = time;
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            g = e.Graphics;
            width = panel1.Width;
            height = panel1.Height;
            draw.DrawCrewGap(g, width, height, CurLB, CurUB,Time);
        }
        private void ShowGap_Load(object sender, EventArgs e)
        {

        }       
    }
}
