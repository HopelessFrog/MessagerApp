using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Messager.Models;
using Messager.Services.Interfaces;
using Newtonsoft.Json;

// 94.19.228.225:6666

namespace Messager.Services
{
    public class LoginService : ILoginService
    {
        private string ip = "94.19.228.225";
        private int port = 6666;
        public Task<ConnectedUser> Login(UserInfo userData)
        {
            try
            {
                if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
                {
                    var userInfo = new UserInfo();
                    var client = new TcpClient();
                    client.Connect(ip,port);

                    var sw = new StreamWriter(client.GetStream());
                    sw.AutoFlush = true;

                    sw.WriteLine(JsonConvert.SerializeObject(userData));
                }

                return new Task<ConnectedUser>(new Func<ConnectedUser>(delegate { return new ConnectedUser();}));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
          
        }
    }
}
