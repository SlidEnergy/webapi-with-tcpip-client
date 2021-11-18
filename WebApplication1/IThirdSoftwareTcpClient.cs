using System;
using System.Threading.Tasks;

namespace WebApplication1
{
    public interface IThirdSoftwareTcpClient: IDisposable
    {
        Task ConnectAsync();
        Task<byte[]> ReceiveAsync();
        Task SendAsync(byte[] data);
    }
}