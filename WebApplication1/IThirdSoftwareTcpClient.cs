using System;
using System.Threading.Tasks;

namespace WebApiWithTcpIpClient
{
    public interface IThirdSoftwareTcpClient: IDisposable
    {
        Task ConnectAsync();
        Task<byte[]> ReceiveAsync();
        Task SendAsync(byte[] data);
    }
}