using System;
using System.Net;

namespace expense.web.eventstore.StoreConnection
{
    public static class IPEndPointExtensions
    {
        public static Uri ToESTcpUri(this IPEndPoint ipEndPoint)
        {
            return new Uri(string.Format("tcp://{0}:{1}", ipEndPoint.Address, ipEndPoint.Port));
        }

        public static Uri ToESTcpUri(this IPEndPoint ipEndPoint, string username, string password)
        {
            return new Uri(string.Format("tcp://{0}:{1}@{2}:{3}", username, password, ipEndPoint.Address, ipEndPoint.Port));
        }
    }
}