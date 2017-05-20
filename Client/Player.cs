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

        public event EventHandler<string> received_message;

        public event EventHandler entered_succeed;

        public event EventHandler left_room;

        public event EventHandler<int> created_room;

        public event EventHandler another_left;

        public event EventHandler<string> received_chat;
        /// <summary>
        /// Đối tượng SimpleTcpClient kết nối với server
        /// </summary>
        private SimpleTCP.SimpleTcpClient simpleClient;
        /// <summary>
        /// Số phòng của Client
        /// </summary>
        private int room;
        /// <summary>
        /// Số thứ thự trong phòng
        /// </summary>
        private int index;
        public SimpleTcpClient SimpleClient { get => simpleClient; set => simpleClient = value; }

        public int Room { get => room; set => room = value; }
        public Player(string IP)
        {

            try
            {
                simpleClient = new SimpleTcpClient().Connect(IP, 9000);
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

                        room = System.Convert.ToInt32(temp[1]);
                        index = System.Convert.ToInt32(temp[2]);
                        if (created_room != null)
                        {
                            created_room(null, room);
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

                        room = Convert.ToInt32(temp[1]);
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



        public void SendToAnother(string message)
        {
            StringBuilder temp = new StringBuilder();
            temp.Append(message);

            temp.AppendFormat("update|{0}|{1}", room, index);
            simpleClient.WriteLine(temp.ToString());
        }

        public void SendOjectToAnother(Object obj)
        {
            try
            {
                string send_message = JsonConvert.SerializeObject(obj);
                StringBuilder temp = new StringBuilder();
                temp.AppendFormat("send message|{0}|{1}|{2}", send_message, room, index);
                simpleClient.WriteLine(temp.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
        public void ChatWithAnother(string message)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("chat with another player|{0}|{1}|{2}", room, index, message);
            simpleClient.WriteLine(stringBuilder.ToString());
        }
        public void CreateRoom()
        {
            simpleClient.WriteLine("create room|");
        }
        public void GotoRoom(int roomNumber)
        {
            simpleClient.WriteLine("go to room|" + roomNumber.ToString());
        }
        public void LeaveRoom()
        {
            StringBuilder temp = new StringBuilder();
            temp.AppendFormat("leave room|{0}|{1}", room, index);
            SimpleClient.WriteLine(temp.ToString());
        }


        public void Dispose()
        {
            SimpleClient.Disconnect();
        }
    }
}
