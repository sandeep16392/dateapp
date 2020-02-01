using Date.API.Configs;
using Microsoft.Extensions.Configuration;

namespace DateApp.API.Configs
{
    public class CommonConfigurations: ICommonConfigurations
    {
        private readonly IConfiguration _configuration;
        public CommonConfigurations(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Token => _configuration["Token"];
        public string ConnectionString => _configuration["DatingAppDatabase"];
    }
}
