using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Terminal_Manager
{
    public class ServerBroker
    {
        public string server_name { get; set; }
        public string server_ip { get; set; }
    }

    public class Plan
    {
        public string name { get; set; }
        public double price { get; set; }
        public double min_lot { get; set; }
        public double max_lot { get; set; }
        public double max_daily_profit { get; set; }
        public double max_daily_loss { get; set; }
        public bool daily_loss_fix { get; set; }
        public double max_total_profit { get; set; }
        public double max_total_loss { get; set; }
        public bool total_loss_fix { get; set; }
        public int max_orders { get; set; }
        public string comment { get; set; }
        public string[] currency_pair { get; set; }
        public bool use_pre_approval { get; set; }
        public double usd_for_pre_approval { get; set; }
        public int validity_period { get; set; }
        public bool outside_control_flag { get; set; }
        public string approval_status { get; set; }
    }

    public class MTAccount
    {
        public string id { get; set; }
        public string account_number { get; set; }
        public string account_password { get; set; }
        public string platform { get; set; }
        public ServerBroker broker { get; set; }
        public Plan plan { get; set; }
        public string server_id { get; set; }
        public string create_date { get; set; }
        public string status { get; set; }

    }

    public class NJ4XServer
    {
        public string id { get; set; }
        public string name { get; set; }
        public string server_ip { get; set; }
        public int server_port { get; set; }
        public int max_terminals { get; set; }
        public int time_sync { get; set; }
        public string status { get; set; }
    }

    public class LoginPacket
    {
        public string email { get; set; }
        public string password { get; set; }
    }
    public class LoginReplyPacket
    {
        public string status { get; set; }
        public string message { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public List<MTAccount> accounts { get; set; }
    }

    class Socket_Manager
    {
        public event LoginReplyEvent LoginReplied;
        
        public static Socket_Manager Instance = new Socket_Manager();
        public Socket m_Socket;
        public Socket_Manager()
        {
        }

        public void Initialize()
        {
            m_Socket = IO.Socket("http://localhost:3333");
            m_Socket.On(Socket.EVENT_CONNECT, () =>
            {
                Console.WriteLine("*** Connection is Established! ***\n");
            });

            m_Socket.On(Socket.EVENT_DISCONNECT, (msg) =>
            {
                Console.WriteLine("*** Disconnected! ***\n");

            });

            m_Socket.On(Socket.EVENT_ERROR, (msg) =>
            {
                Console.WriteLine("*** Error! ***\n" + msg + '\n');
            });
            m_Socket.On("RequestLoginReply", (msg) =>
            {
                Console.WriteLine("*** RequestLoginReply! ***\n" + msg + '\n');
                LoginReplyPacket loginReplyPacket = JsonConvert.DeserializeObject<LoginReplyPacket>(msg.ToString());
                LoginReplied(loginReplyPacket);

            });
        }
        public void SendLoginPacket(string strEmail, string strPassword)
        {
            LoginPacket loginPacket = new LoginPacket();
            loginPacket.email = strEmail;
            loginPacket.password = strPassword;
            string strSendData = JsonConvert.SerializeObject(loginPacket);
            m_Socket.Emit("RequestLogin", strSendData);
        }
    }
}
