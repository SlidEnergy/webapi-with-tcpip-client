using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiWithTcpIpClient
{
    public interface IThirdSoftwareClient: IDisposable
    {
        Task ConnectAsync(CancellationToken cancellationToken = default);
        Task EnsureConnectedAsync(CancellationToken cancellationToken = default);
        Task<byte[]> ReceiveAsync(CancellationToken cancellationToken = default);
        Task SendAsync(byte[] data, CancellationToken cancellationToken = default);
    }
}