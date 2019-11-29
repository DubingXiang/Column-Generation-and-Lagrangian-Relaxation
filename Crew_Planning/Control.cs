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

namespace Crew_Planning
{
    public class Control
    {
        CreateNetWork net = new CreateNetWork();
        
        DataSet Ds = new DataSet();
        Lagrange lag = new Lagrange();
        public void TSSNetWork()
        {
            //rtboc.Text += "系统正在自行建立时空状态网络，请稍候..." + "\r\n";
            Ds = net.ConnDataBase();
            //net.LoadData(Ds,2);
            //net.LoadData_csv(1);
            net.CreateT_S_NetWork();
            //net.CreateT_S_S_NetWork();
            //return net;
           // rtboc.Text += "时空状态网络已经建成，开始计算..." + "\r\n";
            lag.IterationGap(ref net, 0.0005);
        }
             
    }
}
