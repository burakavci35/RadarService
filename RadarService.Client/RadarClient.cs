using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarService.Client
{
    public class RadarClient : IRadarClient
    {
        private readonly HttpClient httpClient;
        private readonly RadarConfig _radarConfiguration;

        public RadarClient(RadarConfig radarConfiguration)
        {
            _radarConfiguration = radarConfiguration;
             httpClient = new HttpClient() { BaseAddress = new Uri(_radarConfiguration.BaseAddress) };
        }

     
        public async Task<bool> GetStatus()
        {
            var loginIsSuccess = await Login();

            return loginIsSuccess;
        }

        private async Task<bool> Login()
        {
            var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("UserName", _radarConfiguration.LoginConfig.UserName),
                    new KeyValuePair<string, string>("Password", _radarConfiguration.LoginConfig.Password)
                });
            var response = await httpClient.PostAsync(_radarConfiguration.LoginConfig.Url, formContent);
            var result = await response.Content.ReadAsStringAsync();

            return result.Equals(_radarConfiguration.LoginConfig.SuccessCondition);
        }

        public async Task SendCommand(bool command)
        {
            throw new NotImplementedException();
        }

     
    }
}
