using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
namespace ServerWartorn
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
            
            LoadServer();
            this.Load += Form1_Load;
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Server.InstanceOfServer.Start();
            Server.InstanceOfServer.gotoRoom += InstanceOfServer_gotoRoom;
            Server.InstanceOfServer.createRoom += InstanceOfServer_createRoom;
            Server.InstanceOfServer.leaveRoom += InstanceOfServer_leaveRoom;
        }

        private void InstanceOfServer_leaveRoom(object sender, Room e)
        {
            listView1.PerformSafely(() =>
            {
                foreach (ListViewItem item in listView1.Items)
                {
                    if (item.Name == e.roomNumber.ToString())
                    {
                        if (e.roomNumber == 1)
                        {
                            listView1.Items.Remove(item);
                        }
                        else
                        {
                            item.Text = "Phòng số :" + e.roomNumber.ToString() + "Số lượng người: 1";
                        }
                        return;
                    }
                }
            });
        }

        private void InstanceOfServer_createRoom(object sender, Room e)
        {
            ListViewItem item = new ListViewItem("Phòng số :" + e.roomNumber.ToString() + "Số lượng người: 1") { Name = e.roomNumber.ToString() };
            listView1.PerformSafely(()=>listView1.Items.Add(item));
        }

        private void InstanceOfServer_gotoRoom(object sender, Room e)
        {
            listView1.PerformSafely(() =>
            {
                foreach (ListViewItem item in listView1.Items)
                {
                    if (e.roomNumber.ToString() == item.Name)
                    {
                        item.Text = "Phòng số :" + e.roomNumber.ToString() + "Số lượng người: 2";
                        return;
                    }
                }
            });
           
        }

        private void LoadServer()
        {            
            lbIP.Text = Server.InstanceOfServer.GetIPAdress();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lbIP.Text = Server.InstanceOfServer.GetIPAdress();
        }
    }
}
