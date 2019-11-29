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
    public partial class ShowGap_CurBest : Form
    {
        public int width;
        public int height;
        public Graphics g;
        public Draw draw = new Draw();
        List<double> CurBestLB = new List<double>();
        List<double> CurBestUB = new List<double>();
        public double Time = 0.0;
        public ShowGap_CurBest()
        {
            InitializeComponent();
        }
        public void Draw(List<double> LB, List<double> UB, double time)
        {

            CurBestLB = LB;
            CurBestUB = UB;
            Time = time;
        }
        private void ShowGap_CurBest_Load(object sender, EventArgs e)
        {

        }

        private void ShowGap_CurBest_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            g = e.Graphics;
            width = panel1.Width;
            height = panel1.Height;
            draw.DrawCrewGap(g, width, height, CurBestLB, CurBestUB, Time);
        }
    }
}
