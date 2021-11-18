using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1
{
    public class ThirdSoftwareService : IThirdSoftwareService, IDisposable
    {
        private readonly IThirdSoftwareTcpClient _client;

        public ThirdSoftwareService(IThirdSoftwareTcpClient client)
        {
            this._client = client;
        }

        public async Task<string> GreetService()
        {
            await _client.ConnectAsync();

            var message = "Hello";
            var data = System.Text.Encoding.UTF8.GetBytes(message);
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
