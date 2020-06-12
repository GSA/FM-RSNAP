using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSNAP.Models
{
    public class AppConfigurtaionServices
    {
        public static IConfiguration Configuration { get; set; }
        static AppConfigurtaionServices()
        {
                      
            Configuration = new ConfigurationBuilder()
            .Add(new JsonConfigurationSource { Path = "RSNAP_appsettings.json", ReloadOnChange = true })
            .Build();
        }
    }
}
