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
    public partial class Form1 : Form
    {
        CreateNetWork cnw = new CreateNetWork();
        DataSet Ds = new DataSet();
        DataTable Dt = new DataTable();
        Control control = new Control();
        public Form1()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Ds = cnw.ConnDataBase();
            //cnw.LoadData(Ds, 2);
            //cnw.LoadData_csv(1);
            cnw.CreateT_S_NetWork();
            //cnw.CreateT_S_S_NetWork();
            //cnw.CreateT_S_S_NetWork(int mindrivetime,int maxdrivetime,int maxconntime,int mintranslation,int minrelaxtime,int maxrelaxtime,int minoutrelaxtime,int maxoutrelaxtime,int mindaycrewtime,int maxdaycrewtime);
            cnw.CreateT_S_S_NetWork(70, 300, 40, 15, 3312, 3330, 3400, 3900, 150, 300);
            //(int maxdrivetime,int mindrivetime,int maxconntime,int mintranslation,int minrelaxtime,int maxrelaxtime,int minoutrelaxtime,int maxoutrelaxtime,int maxdaycrewtime,int mindaycrewtime)
            int k = cnw.T_S_S_NodeList.Count;
            Dt = cnw.ShowResult1();
            dataGridView1.DataSource = Dt;
        }
    }
}
