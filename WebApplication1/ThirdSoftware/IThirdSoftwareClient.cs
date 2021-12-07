using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiWithTcpIpClient
{
    public interface IThirdSoftwareClient: IDisposable
    {
        Task ConnectAsync(CancellationToken cancellationToken);
        Task EnsureConnectedAsync(CancellationToken cancellationToken);
        Task<byte[]> ReceiveAsync(CancellationToken cancellationToken);
        Task SendAsync(byte[] data, CancellationToken cancellationToken);
    }
}