using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using InfoScreenAdminDAL;
using InfoScreenAdminDAL.Entities;

namespace InfoScreenAdminBusiness
{
    public class IPHandler : DBHandler
    {
        public static string CheckIP()
        {
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
                throw new Exception("No network with an IPv4 address in the system.");
            }
            throw new Exception("No connection.");
        }

        public bool UpdateIP(string ip)
        {
            IpAddress address = Model.IpAddresses.FirstOrDefault();
            address.Ip = ip;
            DbAccess.SetActiveIP(address);
            return true;
        }
        public bool CreateNewIp(string ip)
        {
            DbAccess.CreateNewIP(ip);
            return true;
        }

        public IpAddress GetIp()
        {
            return Model.IpAddresses.FirstOrDefault();
        }

        public bool HasIp()
        {
            if(Model.IpAddresses != null && Model.IpAddresses.Count > 0)
            {
                return true;
            } return false;
        }
    }
}
