using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiWithTcpIpClient
{
    public class SomeRepository
    {
        public ItemModel GetData()
        {
            return new ItemModel() { Data = Encoding.UTF8.GetBytes("Hello " + Guid.NewGuid().ToString()) };
        }
    }
}
