using Newtonsoft.Json;
using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Player : IDisposable
    {
        #region Event
        public event EventHandler<string> received_message;

        public event EventHandler entered_succeed;

        public event EventHandler left_room;

        public event EventHandler<int> created_room;

        public event EventHandler another_left;

        public event EventHandler<string> received_chat;
     
        private SimpleTCP.SimpleTcpClient simpleClient;
        #endregion

        private int room;
        /// <summary>
        /// Số thứ thự trong phòng
        /// </summary>
        private int index;

        /// <summary>
        /// Số phòng của Client
        /// </summary>
        public int Room
        {
            get
            {
                return room;
            }

            set
            {
                room = value;
            }
        }

        /// <summary>
        /// Đối tượng SimpleTcpClient kết nối với server
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

        public Player(string IP)
        {

            try
            {
                SimpleClient = new SimpleTcpClient().Connect(IP, 9000);
                SimpleClient.DataReceived += SimpleClient_DataReceived;
            }
            catch (Exception)
            {


            }

        }

        private void SimpleClient_DataReceived(object sender, Message e)
        {
            try
            {
                var temp = e.MessageString.Split('|');
                string message = temp[0];

                switch (message)
                {
                    case "your room is":

                        Room = System.Convert.ToInt32(temp[1]);
                        index = System.Convert.ToInt32(temp[2]);
                        if (created_room != null)
                        {
                            created_room(null, Room);
                        }

                        break;
                    case "you have message":

                        string recieved = temp[1];
                        if (received_message != null)
                        {
                            received_message(null, recieved);
                        }


                        break;
                    case "enter succeed":

                        Room = Convert.ToInt32(temp[1]);
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
                    case "another player give you message":

                        if (received_chat != null)
                        {
                            received_chat(null, temp[1]);
                        }

                        break;
                }
            }
            catch (Exception)
            {


            }

        }



       

        /// <summary>
        /// Send update state to another phayer
        /// </summary>
        /// <param name="obj"></param>
        public void SendOjectToAnother(Object obj)
        {
            try
            {
                string send_message = JsonConvert.SerializeObject(obj);
                StringBuilder temp = new StringBuilder();
                temp.AppendFormat("update|{0}|{1}|{2}", send_message, Room, index);
                SimpleClient.WriteLine(temp.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        /// <summary>
        /// Chat with another phayer in same room
        /// </summary>
        /// <param name="message"></param>
        public void ChatWithAnother(string message)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("chat with another player|{0}|{1}|{2}", Room, index, message);
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
            temp.AppendFormat("leave room|{0}|{1}", Room, index);
            SimpleClient.WriteLine(temp.ToString());
        }


        public void Dispose()
        {
            SimpleClient.Disconnect();
        }
    }
}
