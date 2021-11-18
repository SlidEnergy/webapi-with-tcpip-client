using System.Threading.Tasks;

namespace WebApiWithTcpIpClient
{
    public interface IThirdSoftwareService
    {
        Task<string> SendData(byte[] data);
    }
}