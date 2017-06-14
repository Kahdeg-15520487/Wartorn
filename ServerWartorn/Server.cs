using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleTCP;
using System.Net.Sockets;
using ConvertByteAndOject;
using System.Threading;


namespace ServerWartorn
{
    /// <summary>
    /// Room class save roomNumber and player number
    /// </summary>
    public class Room : EventArgs
    {
        public int roomNumber;
        public int index;

        public Room(int _roomNumber,int _index)
        {
            roomNumber = _roomNumber;
            index = _index;
        }
    }
   
    /// <summary>
    /// Singleton server
    /// </summary>    
    public class Server
    {
        #region Event
        public event EventHandler<Room> gotoRoom;

        public event EventHandler<Room> createRoom;

        public event EventHandler<Room> leaveRoom;

        public event EventHandler<TcpClient> clientConnect;

        public event EventHandler<TcpClient> clientDisConnect;

        private static Server instanceOfServer;
        #endregion

  
        private SimpleTcpServer server;
        /// <summary>
        /// Client connect through this post
        /// </summary>
        private const int PORT = 9000;

        /// <summary>
        /// List of the rooms
        /// </summary>
        private Dictionary<int, List<TcpClient>> rooms;

        /// <summary>
        /// Max number player in one room
        /// </summary>
        private const int MAX_NUM_PLAYER = 2;

        /// <summary>
        /// Save room number that had deleted before
        /// </summary>
        private int saveDel_room = 0;
        /// <summary>
        ///Max rooms
        /// </summary>
        private const int MAX_ROOMS = 100;
        /// <summary>
        /// Instance of singleton
        /// </summary>
        public static Server InstanceOfServer
        {
            get 
            {
                if (instanceOfServer == null)
                {
                    instanceOfServer = new Server();
                }
                return instanceOfServer;
            }
        }

        public Dictionary<int, List<TcpClient>> Rooms
        {
            get
            {
                return rooms;
            }

            set
            {
                rooms = value;
            }
        }

        /// <summary>
        /// Get IP Address
        /// </summary>
        /// <returns></returns>
        public string GetIPAdress()
        {
            string temp = null;
            try
            {
                temp = server.GetIPAddresses().ToArray()[0].ToString();
            }
            catch (Exception)
            {

                
            }
            
            return temp;
        }

        private Server()
        {

        }
        public void Start()
        {
            if (InstanceOfServer != null)
            {

                server = new SimpleTcpServer().Start(PORT);

                server.Delimiter = 0x13;

                Rooms = new Dictionary<int, List<TcpClient>>();

                server.DelimiterDataReceived += Server_DelimiterDataReceived;

                server.ClientConnected += Server_ClientConnected;

                server.ClientDisconnected += Server_ClientDisconnected;
               
            }
            else
            {
               instanceOfServer = new Server();
                server = new SimpleTcpServer().Start(PORT);

                server.Delimiter = 0x13;

                Rooms = new Dictionary<int, List<TcpClient>>();

                server.DelimiterDataReceived += Server_DelimiterDataReceived;
            }
        
        }

        /// <summary>
        /// Raise when one client disconnect, and then send to another player know
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Server_ClientDisconnected(object sender, TcpClient e)
        {
            byte[] send = server.StringEncoder.GetBytes("Another player leave room");
            foreach (KeyValuePair<int,List<TcpClient>> item in rooms)
            {
                for (int i = 0; i < item.Value.Count; i++)
                {
                    if (item.Value[i] == e)
                    {
                        LeaveRoom(item.Value[i], item.Key);                      
                    }
                }
              
            }
            if (clientDisConnect != null)
            {
                clientDisConnect(null, e);
            }
        }

        /// <summary>
        /// Raise when one client connect to server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Server_ClientConnected(object sender, TcpClient e)
        {
            byte[] send = server.StringEncoder.GetBytes("connect succeed");

            e.GetStream().Write(send, 0, send.Length);

            if (clientConnect != null)
            {
                clientConnect(null, e);
            }
          
        }



        /// <summary>
        /// Xử lý đi vào phòng của client
        /// </summary>
        /// <param name="client"></param>
        /// <param name="info"></param>
        private void GoToRoom(TcpClient client, int roomNumber,string name)
        {
            try
            {
                if (Rooms.ContainsKey(roomNumber))
                {
                    if (Rooms[roomNumber].Count == 2)
                    {
                        //Reply to full room
                        Reply(client, "full room");
                    }
                    else
                    {
                        Rooms[roomNumber].Add(client);

                        //Reply to enter succeed
                        StringBuilder stringBuilder = new StringBuilder();
                        stringBuilder.AppendFormat("enter succeed|{0}|{1}", roomNumber, 1);
                        Reply(client, stringBuilder.ToString());

                        //Reply to host that another phayer have gone to room 
                        stringBuilder.Clear();
                        stringBuilder.AppendFormat("another player go room|{0}", name);
                        SendMessage(client, roomNumber, 1, stringBuilder.ToString());

                        //Raise gotoRoom event to custom UI of server
                        if (gotoRoom != null)
                        {
                            gotoRoom(null, new Room(roomNumber, 1));
                        }
                    }
                }
                else
                {
                    //Reply to phayer room not exists
                    Reply(client, "room not exists");
                }
            }
            catch (Exception)
            {
                //Reply to send wrong format
                Reply(client, "send wrong format");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="info"></param>
        private void CreateRoom(Message e)
        {
            if (Rooms.Count == 100)
            {
                e.Reply("Server is bussy");
            }
            else if (saveDel_room == 0)
            {
                Rooms.Add(Rooms.Count + 1, new List<TcpClient>() { e.TcpClient });
                StringBuilder temp = new StringBuilder();
                temp.AppendFormat("your room is|{0}|{1}", Rooms.Count, 0);
                if (createRoom != null)
                {
                    createRoom(null, new Room(Rooms.Count,1));
                }
                e.Reply(temp.ToString());

            }
            else
            {

                Rooms.Add(saveDel_room, new List<TcpClient>() { e.TcpClient });
                StringBuilder temp = new StringBuilder();
                temp.AppendFormat("your room is|{0}|{1}", saveDel_room, 0);
                if (createRoom != null)
                {
                    createRoom(null, new Room(saveDel_room,1));
                }
                e.Reply(temp.ToString());
                
            }
        }

        private void Reply(TcpClient client,string message)
        {
            byte[] message_to_bytes = server.StringEncoder.GetBytes(message);
            client.GetStream().Write(message_to_bytes, 0, message_to_bytes.Length);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LeaveRoom(TcpClient client, int roomNumber)
        {
            try
            {
                if (Rooms[roomNumber].Count == 2)
                {
                    Rooms[roomNumber].Remove(client);

                    //Thông báo cho người chơi còn lại mình đã thoát            
                    Reply(Rooms[roomNumber][0], "Another player leave room");
  
                    if (leaveRoom != null)
                    {
                        leaveRoom(null, new Room(roomNumber,1));
                    }                  
                }
                else
                {
                    //Room has only one player, delete room and save room to use to create room the next time
                    Rooms.Remove(roomNumber);
                    saveDel_room = roomNumber;
                    if (leaveRoom != null)
                    {
                        leaveRoom(null, new Room(roomNumber,0));
                    }
                }
            }
            catch (Exception er)
            {
                Reply(client, "send wrong format");
            }
        }
        /// <summary>
        /// Player has finished his turn, an another player must be update
        /// </summary>
        /// <param name="client"></param>
        /// <param name="info"></param>
        private void UpDate(TcpClient client, int roomNumber, int index, string update_command)
        {
            try
            {
                StringBuilder temp = new StringBuilder();

                temp.AppendFormat("update|{0}", update_command);

                byte[] send = server.StringEncoder.GetBytes(temp.ToString());

                if (index == 0)
                {
                    Rooms[roomNumber][index + 1].GetStream().Write(send, 0, send.Length);
                }
                else
                {
                    Rooms[roomNumber][index - 1].GetStream().Write(send, 0, send.Length);
                }
            }
            catch (Exception)
            {
                Reply(client, "send wrong format");

            }
        }
        /// <summary>
        /// Player send message to another phayer
        /// </summary>
        /// <param name="client"></param>
        /// <param name="info"></param>
        private void SendMessage(TcpClient client, int roomNumber, int index, string send_message)
        {
            try
            {
                StringBuilder temp = new StringBuilder();

                temp.AppendFormat("you have message|{0}", send_message);
                byte[] send = server.StringEncoder.GetBytes(temp.ToString());
                if (index == 0)
                {

                    Rooms[roomNumber][index + 1].GetStream().Write(send, 0, send.Length);
                }
                else
                {
                    Rooms[roomNumber][index - 1].GetStream().Write(send, 0, send.Length);
                }

            }
            catch (Exception)
            {
                //e.Reply("send wrong format");
                Reply(client, "send wrong format");

            }
        }

        private void Server_DelimiterDataReceived(object sender, SimpleTCP.Message e)
        {

            try
            {
                string[] info = e.MessageString.Split('|');
                string message = info[0];


                switch (message)
                {
                    case "go to room":
                        GoToRoom(e.TcpClient, Convert.ToInt32(info[1]),info[2]);

                        break;
                    case "create room":
                        CreateRoom(e);

                        break;
                    case "leave room":
                        LeaveRoom(e.TcpClient, Convert.ToInt32(info[1]));

                        break;
                    case "send message":
                        SendMessage(e.TcpClient, Convert.ToInt32(info[1]), Convert.ToInt32(info[2]), info[3]);
                        break;
                    case "update":
                        UpDate(e.TcpClient, Convert.ToInt32(info[1]), Convert.ToInt32(info[2]), info[3]);
                        break;
                    default:
                        e.Reply("send wrong format");
                        break;

                }
            }
            catch (Exception er)
            {
                
            }
           
        }
    }
}