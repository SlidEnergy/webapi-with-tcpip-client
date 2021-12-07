using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiWithTcpIpClient
{
    public class ItemModel
    {
        public byte[] Data { get; set; }

        public byte[] Serialize()
        {
            return Data;
        }
    }
}
