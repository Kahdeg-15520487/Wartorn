using Newtonsoft.Json;
using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    /// <summary>
    /// Singleton Player
    /// </summary>
    public class Player : IDisposable
    {
        private static Player instance;
        /// <summary>
        /// Use to singleton in multithread
        /// </summary>
        private static object syncRoot = new Object();
        #region Event
        public event EventHandler<string> update;

        public event EventHandler entered_succeed;

        public event EventHandler<int> created_room;

        public event EventHandler another_left;

        public event EventHandler<string> received_chat;
     
        private SimpleTCP.SimpleTcpClient simpleClient;
        #endregion

        private int roomNumber;
        /// <summary>
        /// Index in room : 0 or 1
        /// </summary>
        private int index;

        /// <summary>
        /// Room number of client
        /// </summary>
        public int RoomNumber
        {
            get
            {
                return roomNumber;
            }

            set
            {
                roomNumber = value;
            }
        }

        /// <summary>
        /// Instance use to connect to server
        /// </summary>
        public SimpleTcpClient SimpleClient
        {
            get
            {
                return simpleClient;
            }

            set
            {
                simpleClient = value;
            }
        }

        /// <summary>
        /// Instance of class
        /// </summary>
        public static Player Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        instance = new Player();
                    }
                }
                return instance;
            }

        }

        private Player()
        {

        }
        /// <summary>
        /// Connect to server with ip of server
        /// </summary>
        /// <param name="IP"></param>
        public void ConnectToServer(string IP)
        {

            try
            {
                SimpleClient = new SimpleTcpClient().Connect(IP, 9000);
                SimpleClient.DataReceived += SimpleClient_DataReceived;
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);

            }

        }

        private void SimpleClient_DataReceived(object sender, SimpleTCP.Message e)
        {
            try
            {
                var temp = e.MessageString.Split('|');
                string message = temp[0];

                switch (message)
                {
                    case "your room is":

                        RoomNumber = System.Convert.ToInt32(temp[1]);
                        index = System.Convert.ToInt32(temp[2]);
                        if (created_room != null)
                        {
                            created_room(null, RoomNumber);
                        }

                        break;
                    case "update":                      
                        if (update != null)
                        {
                            update(null,temp[1]);
                        }
                        break;
                    case "enter succeed":

                        RoomNumber = Convert.ToInt32(temp[1]);
                        index = Convert.ToInt32(temp[2]);
                        if (entered_succeed != null)
                        {
                            entered_succeed(null, EventArgs.Empty);
                        }
                        break;
                    case "Another player leave room":
                        index = 0;
                        if (another_left != null)
                        {
                            another_left(null, EventArgs.Empty);
                        }
                        break;
                    case "you have message":

                        if (received_chat != null)
                        {
                            received_chat(null, temp[1]);
                        }
                        break;
                }
            }
            catch (Exception er) 
            {

                //Wartorn.Utility.HelperFunction.Log(er); 
            }

        }

        /// <summary>
        /// Send update state to another phayer
        /// </summary>
        /// <param name="obj"></param>
        public void Update(Object obj)
        {
            try
            {
                string send_message = JsonConvert.SerializeObject(obj);
                StringBuilder temp = new StringBuilder();
                temp.AppendFormat("update|{0}|{1}|{2}", send_message, RoomNumber, index);
                SimpleClient.WriteLine(temp.ToString());
            }
            catch (Exception e)
            {

                //Wartorn.Utility.HelperFunction.Log(e);
            }

        }
        /// <summary>
        /// Chat with another phayer in same room
        /// </summary>
        /// <param name="message"></param>
        public void ChatWithAnother(string message)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("chat with another player|{0}|{1}|{2}", RoomNumber, index, message);
            SimpleClient.WriteLine(stringBuilder.ToString());
        }
        /// <summary>
        /// Create new room
        /// </summary>
        public void CreateRoom()
        {
            SimpleClient.WriteLine("create room|");
        }
        /// <summary>
        /// Go to room
        /// </summary>
        /// <param name="roomNumber"></param>
        public void GotoRoom(int roomNumber)
        {
            SimpleClient.WriteLine("go to room|" + roomNumber.ToString());
        }
        /// <summary>
        /// Leave room
        /// </summary>
        public void LeaveRoom()
        {
            StringBuilder temp = new StringBuilder();
            temp.AppendFormat("leave room|{0}|{1}", RoomNumber, index);
            SimpleClient.WriteLine(temp.ToString());
        }

        /// <summary>
        /// Disconnect to server
        /// </summary>
        public void Dispose()
        {
            SimpleClient.Disconnect();
        }
    }
}
