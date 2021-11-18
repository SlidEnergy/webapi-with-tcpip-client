using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiWithTcpIpClient
{
    public class ThirdSoftwareService : IThirdSoftwareService, IDisposable
    {
        private readonly IThirdSoftwareTcpClient _client;

        public ThirdSoftwareService(IThirdSoftwareTcpClient client)
        {
            this._client = client;
        }

        public async Task<string> SendData(byte[] data)
        {
            await _client.ConnectAsync();

            await _client.SendAsync(data);

            var response = await _client.ReceiveAsync();
            var responseMessage = System.Text.Encoding.UTF8.GetString(response);

            return responseMessage;
        }

        public void Dispose()
        { 
            _client?.Dispose();
        }
    }
}
